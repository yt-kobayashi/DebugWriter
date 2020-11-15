using System;

namespace DebugWriter
{
    public enum Mode
    {
        Debug,
        Error,
        Status,
        Release
    }

    public interface Writer
    {
        void OutputMessage();
    }

    public class DebugWriter : Writer
    {
        public void OutputMessage()
        {
        }
    }

    public class ErrorWriter : Writer
    {
        public void OutputMessage()
        {
        }
    }

    public class StatusWriter : Writer
    {
        public void OutputMessage()
        {
        }
    }

    public class ReleaseWriter : Writer
    {
        public void OutputMessage()
        {
        }
    }
}
