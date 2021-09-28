using System;

namespace LegacyInstaller.Patcher
{
    internal class ProcessNotFoundException : Exception { }
    internal class ModuleNotFoundException : Exception { }
    internal class StringNotFoundException : Exception { }
    internal class PatternNotFoundException : Exception { }
    internal class PatchAlreadyAppliedException : Exception { }

    internal class ProcessOpenException : Exception
    {
        public readonly int ErrorCode;

        public ProcessOpenException(int code)
        {
            ErrorCode = code;
        }
    }

    internal class MemoryReadException : Exception
    {
        public readonly int ErrorCode;

        public MemoryReadException(int code)
        {
            ErrorCode = code;
        }
    }

    internal class MemoryWriteException : Exception
    {
        public readonly int ErrorCode;

        public MemoryWriteException(int code)
        {
            ErrorCode = code;
        }
    }
}
