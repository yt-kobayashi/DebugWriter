using System;
using System.Runtime.CompilerServices;

namespace DebuggerLib
{
    public enum Mode
    {
        Debug,
        Error,
        Status,
        Trace,
        Release,
        Enter
    }

    public interface Writer
    {
        void Write(in string message = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0);
    }

    public class Debugger
    {
        public Writer Debug { get; set; }
        public Writer Error { get; set; }
        public Writer Status { get; set; }
        public Writer Enter { get; set; }

        private bool EnableCaller { get; set; }
        private bool EnableLineNumber { get; set; }

        public Debugger() { }
        public Debugger(in Mode mode, in bool enableCaller = true, in bool enableLineNumber = true)
        {
            EnableCaller = enableCaller;
            EnableLineNumber = enableLineNumber;
            Initialize(mode);
        }
        public Debugger(in bool enableDebug, in bool enableError, in bool enableStatus, in bool enableEnter, in bool enableCaller = true, in bool enableLineNumber = true)
        {
            EnableCaller = enableCaller;
            EnableLineNumber = enableLineNumber;
            SetWriters(enableDebug, enableError, enableStatus, enableEnter);
        }

        public bool Initialize(in Mode mode)
        {
            switch (mode)
            {
                case Mode.Debug:
                    SetWriters(true, true, true, true);
                    break;
                case Mode.Error:
                    SetWriters(false, true, false, false);
                    break;
                case Mode.Status:
                    SetWriters(false, false, true, false);
                    break;
                case Mode.Trace:
                    SetWriters(false, true, true, true);
                    break;
                case Mode.Release:
                    SetWriters(false, false, false, false);
                    break;
                case Mode.Enter:
                    SetWriters(false, false, false, true);
                    break;
                default:
                    SetWriters(false, false, false, false);
                    break;
            }

            return true;
        }

        public bool SetWriters(in bool enableDebug, in bool enableError, in bool enableStatus, in bool enableEnter)
        {
            Debug = SetWriter(enableDebug, Mode.Debug);
            Error = SetWriter(enableError, Mode.Error);
            Status = SetWriter(enableStatus, Mode.Status);
            Enter = SetWriter(enableEnter, Mode.Enter);

            return true;
        }

        public Writer SetWriter(in bool enable, in Mode mode)
        {
            Writer debugWriter = new DebugWriter(mode, EnableCaller, EnableLineNumber);

            if (false == enable)
            {
                debugWriter = new DebugWriterEmpty();
            }

            return debugWriter;
        }

    }

    public class DebugWriter : Writer
    {
        private string WriterMode { get; set; }
        private string EnterMessage { get; set; }
        private bool EnableCaller { get; set; }
        private bool EnableLineNumber { get; set; }

        public DebugWriter(in Mode mode, in bool enableCaller = true, in bool enableLineNumber = true)
        {
            EnterMessage = "";
            EnableCaller = enableCaller;
            EnableLineNumber = enableLineNumber;

            switch (mode)
            {
                case Mode.Debug:
                    WriterMode = "[Debug ]";
                    break;
                case Mode.Error:
                    WriterMode = "[Error ]";
                    break;
                case Mode.Status:
                    WriterMode = "[Status]";
                    break;
                case Mode.Enter:
                    WriterMode = "[Enter ]";
                    EnterMessage = "Enter";
                    break;
                default:
                    break;
            }
        }

        public void Write(in string message = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            string caller = "";
            if ("" != callerName && true == EnableCaller)
            {
                caller = "[" + callerName + "]";
            }
            if (true == EnableLineNumber)
            {
                caller += "[Line " + lineNumber + "]";
            }


            string date;
            DateTime dateTime = DateTime.Now;
            date = "[" + dateTime + "." + dateTime.Millisecond + "]";

            Console.WriteLine(date + WriterMode + caller + " : " + EnterMessage + message);
        }
    }

    public class DebugWriterEmpty : Writer
    {
        public void Write(in string message = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0) { }
    }
}
