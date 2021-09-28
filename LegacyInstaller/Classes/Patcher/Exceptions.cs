using System;

namespace LegacyInstaller.Patcher
{
    internal class ProcessNotFoundException : Exception { }
    internal class ModuleNotFoundException : Exception { }
    internal class StringNotFoundException : Exception { }
    internal class PatternNotFoundException : Exception { }
    internal class PatchAlreadyAppliedException : Exception { }

    internal interface NativeError
    {
        int ErrorCode { get; }
    }

    internal class ProcessOpenException : Exception, NativeError
    {
        public int ErrorCode { get; private set; }

        public ProcessOpenException(int code)
        {
            ErrorCode = code;
        }
    }

    internal class MemoryReadException : Exception, NativeError
    {
        public int ErrorCode { get; private set; }

        public MemoryReadException(int code)
        {
            ErrorCode = code;
        }
    }

    internal class MemoryWriteException : Exception, NativeError
    {
        public int ErrorCode { get; private set; }

        public MemoryWriteException(int code)
        {
            ErrorCode = code;
        }
    }
}
