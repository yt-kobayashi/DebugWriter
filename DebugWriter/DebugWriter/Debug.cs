using System;

namespace DebuggerLib
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
        void Write(in string message);
    }

    public class Debugger
    {
        public Writer Debug { get; set; }
        public Writer Error { get; set; }
        public Writer Status { get; set; }
        public Writer Release { get; set; }

        public Debugger() { }
        public Debugger(in Mode mode)
        {
            Initialize(mode);
        }

        public bool Initialize(in Mode mode)
        {
            switch (mode)
            {
                case Mode.Debug:
                    SetWriters(true, true, true, true);
                    break;
                case Mode.Error:
                    SetWriters(false, true, true, true);
                    break;
                case Mode.Status:
                    SetWriters(false, false, true, true);
                    break;
                case Mode.Release:
                    SetWriters(false, false, false, true);
                    break;
                default:
                    SetWriters(false, false, false, false);
                    break;
            }

            return true;
        }

        public bool SetWriters(in bool enableDebug, in bool enableError, in bool enableStatus, in bool enableRelease)
        {
            Debug = SetWriter(enableDebug, Mode.Debug);
            Error = SetWriter(enableError, Mode.Error);
            Status = SetWriter(enableStatus, Mode.Status);
            Release = SetWriter(enableRelease, Mode.Release);

            return true;
        }

        public Writer SetWriter(in bool enable, in Mode mode)
        {
            Writer debugWriter;

            if (enable)
            {
                debugWriter = new DebugWriter(mode);
            }
            else
            {
                debugWriter = new DebugWriterEmpty();
            }

            return debugWriter;
        }

        public class DebugWriter : Writer
        {
            private string WriterMode { get; set; }

            public DebugWriter(in Mode mode)
            {
                switch (mode)
                {
                    case Mode.Debug:
                        WriterMode = "[Debug  ] : ";
                        break;
                    case Mode.Error:
                        WriterMode = "[Error  ] : ";
                        break;
                    case Mode.Status:
                        WriterMode = "[Status ] : ";
                        break;
                    case Mode.Release:
                        WriterMode = "[Release] : ";
                        break;
                    default:
                        break;
                }
            }

            public void Write(in string message)
            {
                string date;
                DateTime dateTime = DateTime.Now;
                date = "[" + dateTime + "." + dateTime.Millisecond + "]";

                Console.WriteLine(date + WriterMode + message);
            }
        }

        public class DebugWriterEmpty : Writer
        {
            public void Write(in string message) { }
        }
    }
}
