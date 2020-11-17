using System;
using System.Collections.Generic;
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

    public class MessageFormat
    {
        public const byte Date = 1;
        public const byte Mode = 2;
        public const byte Caller = 4;
        public const byte Number = 8;
        public const byte EnterMessage = 16;
        public const byte Message = 32;
        public const byte DEFAULT = (byte)(Date | Mode | Caller | Message);
        public const byte ENTER = (byte)(Date | Mode | Caller | EnterMessage | Message);
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

        private Dictionary<Mode, bool[]> ModeParam { get; set; }

        public Debugger(in Mode mode, in byte format = MessageFormat.DEFAULT)
        {
            ModeParam = new Dictionary<Mode, bool[]>();
            ModeParam.Add(Mode.Debug, new bool[] { true, true, true, true});
            ModeParam.Add(Mode.Error, new bool[] { false, true, false, false});
            ModeParam.Add(Mode.Trace, new bool[] { false, false, true, true});
            ModeParam.Add(Mode.Status, new bool[] { false, false, true, false});
            ModeParam.Add(Mode.Enter, new bool[] { false, false, false, true});
            ModeParam.Add(Mode.Release, new bool[] { false, false, false, false});

            Initialize(mode, format);
        }

        public void Initialize(in Mode mode, in byte format)
        {
            bool[] param = ModeParam[mode];

            Debug = SetWriter(param[0], Mode.Debug, format);
            Error = SetWriter(param[1], Mode.Error, format);
            Status = SetWriter(param[2], Mode.Status, format);
            Enter = SetWriter(param[3], Mode.Enter, format);
        }

        public Writer SetWriter(in bool enable, in Mode mode, in byte format)
        {
            Writer debugWriter = new DebugWriter(mode, format);

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
        private string EnterMessage { get; set; } = "";
        private string Format { get; set; }

        public DebugWriter(in Mode mode, in byte format = MessageFormat.DEFAULT)
        {
            byte messageFormat = format;
            if (Mode.Enter == mode)
            {
                messageFormat = (byte)(messageFormat | (byte)MessageFormat.EnterMessage);
                EnterMessage = " - Enter";
            }

            WriterMode = mode.ToString();
            byte mask;

            for(int count = 0; count < 8; count++)
            {
                mask = (byte)(1 << count);

                if(0 == (messageFormat & mask))
                {
                    continue;
                }
                else if(MessageFormat.Message == mask)
                {
                    Format += " : {" + count.ToString() + "}";
                }
                else if(MessageFormat.EnterMessage == mask)
                {
                    Format += "{" + count.ToString() + "}";
                }
                else
                {
                    Format += "[{" + count.ToString() + "}]";
                }
            }
        }

        public void Write(in string message = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            List<string> messageList = new List<string>();
            string debugMessage = "";
            string number = "Line " + lineNumber.ToString();
            string date = string.Format("{0}.{1}", DateTime.Now, DateTime.Now.Millisecond);

            debugMessage = string.Format(Format, date, WriterMode, callerName, number, EnterMessage, message);

            Console.WriteLine(debugMessage);
        }
    }

    public class DebugWriterEmpty : Writer
    {
        public void Write(in string message = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0) { }
    }
}
