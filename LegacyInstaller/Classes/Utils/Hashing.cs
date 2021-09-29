using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LegacyInstaller.Utils
{
    internal static class Hashing
    {
        public static string Hash(byte[] bytes)
        {
            using (var sha = SHA1.Create())
            {
                byte[] hash = sha.ComputeHash(bytes);
                return ToHex(hash);
            }
        }

        public static string Hash(string text)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(text);
            return Hash(utf8);
        }

        public static string FastHashDirectory(string directoryName, params string[] ignoreFileNames)
        {
            using (var sha = SHA1.Create())
            {
                var files = IO.EnumerateDirectory(directoryName, relative: true);
                var buffer = new byte[0];

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    if (ignoreFileNames.Contains(fileName)) continue;

                    var utf8 = Encoding.UTF8.GetBytes(file);
                    var hash = sha.ComputeHash(utf8);
                    buffer = buffer.Concat(hash).ToArray();
                }

                var finalHash = sha.ComputeHash(buffer);
                return ToHex(finalHash);
            }
        }

        public static string HashDirectory(string directoryName, params string[] ignoreFileNames)
        {
            using (var sha = SHA1.Create())
            {
                var files = IO.EnumerateDirectory(directoryName, relative: false);
                var buffer = new byte[0];

                foreach (var file in files)
                {
                    if (!File.Exists(file)) continue;

                    var fileName = Path.GetFileName(file);
                    if (ignoreFileNames.Contains(fileName)) continue;

                    using (var fs = new FileStream(file, FileMode.Open))
                    using (var buf = new BufferedStream(fs))
                    {
                        var hash = sha.ComputeHash(buf);
                        buffer = buffer.Concat(hash).ToArray();
                    }
                }

                var finalHash = sha.ComputeHash(buffer);
                return ToHex(finalHash);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                _ = sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
