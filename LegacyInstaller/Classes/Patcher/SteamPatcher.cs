using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using static LegacyInstaller.Patcher.Native;

namespace LegacyInstaller.Patcher
{
    internal static class SteamPatcher
    {
        private static readonly string _processName = "steam";
        private static readonly string _moduleName = "steamclient.dll";
        private static readonly byte[] _targetBytes = Encoding.UTF8.GetBytes("Depot download failed : Manifest not available");

        public static void ApplyPatch()
        {
            var process = Process.GetProcessesByName(_processName).FirstOrDefault();
            if (process == null)
            {
                throw new ProcessNotFoundException();
            }

            var module = process.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == _moduleName);
            if (module == null)
            {
                throw new ModuleNotFoundException();
            }

            var accessFlags = ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryOperation | ProcessAccessFlags.VirtualMemoryRead | ProcessAccessFlags.VirtualMemoryWrite;
            var processHandle = OpenProcess(accessFlags, false, process.Id);
            if (processHandle == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                throw new ProcessOpenException(error);
            }

            IntPtr moduleHandle = module.BaseAddress;
            Debug.WriteLine($"Process handle: 0x{processHandle:x}");
            Debug.WriteLine($"Module handle: 0x{moduleHandle:x}");

            var buffer = new byte[module.ModuleMemorySize];
            if (ReadProcessMemory(processHandle, moduleHandle, buffer, buffer.Length, out int bytesRead) == false)
            {
                int error = Marshal.GetLastWin32Error();
                throw new MemoryReadException(error);
            }

            int stringIdx = Search(buffer, _targetBytes);
            if (stringIdx == -1)
            {
                throw new StringNotFoundException();
            }

            int stringAddr = (int)moduleHandle + stringIdx;
            var pushBytes = BitConverter.GetBytes(stringAddr).Prepend<byte>(0x68).ToArray();

            Debug.WriteLine($"String address: 0x{stringAddr:x}");
            // Debug.WriteLine(BitConverter.ToString(pushBytes).Replace('-', ' '));

            var pushIdx = Search(buffer, pushBytes);
            if (pushIdx == -1)
            {
                throw new PatternNotFoundException();
            }

            Debug.WriteLine($"Pattern index: 0x{pushIdx:x}");

            // 0x0F 0x85 is JNZ instruction (jump if true)
            var index = pushIdx;
            while (buffer[index] != 0x0F && buffer[index + 1] != 0x85)
            {
                index -= 1;
            }

            // Either the patch has already been applied or the JNZ isn't there
            // We want to prevent writing over code which isn't actually part
            // of what we want to patch.
            if (index < pushIdx - 10)
            {
                throw new PatchAlreadyAppliedException();
            }

            // Replace 2-byte jnz with nop, jmp
            buffer[index] = 0x90;
            buffer[index + 1] = 0xE9;

            var patchAddr = moduleHandle + index;
            var memory = buffer.Skip(index).Take(2).ToArray();
            if (WriteProcessMemory(processHandle, patchAddr, memory, memory.Length, out int bytesWritten) == false)
            {
                int error = Marshal.GetLastWin32Error();
                throw new MemoryWriteException(error);
            }

            Debug.WriteLine("Wrote patch to memory.");
        }

        private static int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                // compare only first byte
                if (src[i] != pattern[0]) continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }

            return -1;
        }
    }
}
