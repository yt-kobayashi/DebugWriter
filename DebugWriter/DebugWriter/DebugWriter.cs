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
        void OutputDebugWindow();
    }

    public class DebugWriter : Writer
    {
        public void OutputDebugWindow()
        {

        }
    }
}
