using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DebuggerLib
{
    /// <summary>
    /// デバッグモード
    /// </summary>
    public enum Mode : int
    {
        Release = 0x00,
        Debug = 0x01,
        Error = 0x02,
        Status = 0x04,
        Enter = 0x08,
        Exit = 0x10,
        All = 0xFF,
        Trace = Error | Status | Enter | Exit,
        Access = Enter | Exit,
        Message = Debug | Error | Status
    }

    /// <summary>
    /// 表示を行う要素
    /// </summary>
    public enum Format : int
    {
        Date = 0x01,
        Mode = 0x02,
        CallerName = 0x04,
        CallerLine = 0x08,
        EnterMessage = 0x10,
        ExitMessage = 0x20,
        Message = 0x40
    }

    /// <summary>
    /// 本ライブラリで使用する定数
    /// </summary>
    public class Param
    {
        public const Format FORMAT_DEFAULT = Format.Date | Format.Mode | Format.CallerName | Format.Message;
        public const Format FORMAT_ENTER = Format.Date | Format.Mode | Format.CallerName | Format.EnterMessage | Format.Message;
        public const Format FORMAT_EXIT = Format.Date | Format.Mode | Format.CallerName | Format.ExitMessage | Format.Message;
        public const Format FORMAT_SIMPLE = Format.Date | Format.CallerName | Format.Message;
        public const string HasEnteredMessage = " - Enter";
        public const string HasExitMessage = " - Exit";
        public static string PROJECT_FILEPATH = Directory.GetCurrentDirectory();
    }

    /// <summary>
    /// インターフェース
    /// </summary>
    public interface Writer
    {
        void Write(string message = "", params object[] Params);
        void WriteLine(string message = "", params object[] Params);
    }

    /// <summary>
    /// デバッグメッセージを出力するクラス．
    /// モードを設定することで出力を制御できる．
    /// </summary>
    /// <remarks>
    /// モード<br/>
    /// Release     :   全てのメッセージが表示されない．
    /// Debug       :   Debugメッセージが表示される．
    /// Error       :   Errorメッセージのみが表示される．
    /// Status      :   Statusメッセージのみが表示される．
    /// Enter       :   Enter, Exitメッセージが表示される．
    /// Exit        :   Enter, Exitメッセージが表示される.
    /// All         :   全てのメッセージが表示される.
    /// Trace       :   Error, Status, Enter, Exitメッセージが表示される．
    /// Access      :   Enter, Exitメッセ―じが表示される．
    /// Message     :   Debug, Error, Statusメッセージが表示される．
    /// </remarks>
    public class Debugger
    {
        private Writer Debug { get; set; }
        private Writer Error { get; set; }
        private Writer Status { get; set; }
        private Writer Enter { get; set; }
        private Writer Exit { get; set; }
        public delegate void Write(string message = "", params object[] Params);
        public delegate void WriteLine(string message = "", params object[] Params);
        public Write DebugWrite;
        public WriteLine DebugWriteLine;
        public Write ErrorWrite;
        public WriteLine ErrorWriteLine;
        public Write StatusWrite;
        public WriteLine StatusWriteLine;
        public Write EnterWrite;
        public WriteLine EnterWriteLine;
        public Write ExitWrite;
        public WriteLine ExitWriteLine;

        public Debugger(in Mode mode, in Format format = Param.FORMAT_DEFAULT)
        {
            int stage = 0;
            bool canInit = true;

            while(true == canInit)
            {
                switch (stage)
                {
                    case 0:
                        Initialize(mode, format);
                        break;
                    case 1:
                        InitializeDebugWriterMethod();
                        break;
                    default:
                        canInit = false;
                        break;
                }

                stage++;
            }
        }

        private void Initialize(in Mode mode, in Format format)
        {
            Debug = SetWriter(mode, Mode.Debug, format);
            Error = SetWriter(mode, Mode.Error, format);
            Status = SetWriter(mode, Mode.Status, format);
            Enter = SetWriter(mode, Mode.Enter, format | Format.EnterMessage);
            Exit = SetWriter(mode, Mode.Exit, format | Format.ExitMessage);
        }

        private Writer SetWriter(in Mode mode, in Mode writerType, in Format format)
        {
            Writer debugWriter = new DebugWriter(writerType, format);

            if (false == Enable(mode, writerType))
            {
                debugWriter = new DebugWriterEmpty();
            }

            return debugWriter;
        }

        private bool Enable(in Mode mode, in Mode writerType)
        {
            bool ret = (0 != (mode & writerType));

            return ret;
        }

        private void InitializeDebugWriterMethod()
        {
            DebugWrite = new Write(Debug.Write);
            DebugWriteLine = new WriteLine(Debug.WriteLine);
            ErrorWrite = new Write(Error.Write);
            ErrorWriteLine = new WriteLine(Error.WriteLine);
            StatusWrite = new Write(Status.Write);
            StatusWriteLine = new WriteLine(Status.WriteLine);
            EnterWrite = new Write(Enter.Write);
            EnterWriteLine = new WriteLine(Enter.WriteLine);
            ExitWrite = new Write(Exit.Write);
            ExitWriteLine = new WriteLine(Exit.WriteLine);
        }
    }

    /// <summary>
    /// デバッグ表示を行うメソッドの本体を格納するクラス．
    /// </summary>
    public class DebugWriter : Writer
    {
        private string WriterMode { get; set; }
        private string MessageFormat { get; set; }
        private static string LogFilePath { get; set; }             // ログを出力するファイルパス

        public DebugWriter(in Mode mode, in Format format = (Format)Param.FORMAT_DEFAULT)
        {
            WriterMode = mode.ToString();
            InitializeFormat(format);

            if(null == LogFilePath)
            {
                LogFilePath = string.Join("", Param.PROJECT_FILEPATH, "\\Debug_", DateTime.Now.ToString("yyyyMMddHHmmss"), ".log");
            }
        }

        private void InitializeFormat(in Format format)
        {
            int mask;

            for (int count = 0; count < (sizeof(int) * 8); count++)
            {
                mask = 0x01 << count;
                switch (format & (Format)mask)
                {
                    case Format.Date:
                    case Format.Mode:
                    case Format.CallerName:
                    case Format.CallerLine:
                        MessageFormat += "[{" + count.ToString() + "}]";
                        break;
                    case Format.EnterMessage:
                    case Format.ExitMessage:
                        MessageFormat += "{" + count.ToString() + "}";
                        break;
                    case Format.Message:
                        MessageFormat += " : {" + count.ToString() + "}";
                        break;
                    default:
                        break;
                }
            }
        }

        public void Write(string message = "", params object[] Params) 
        {
            using(var stream = new StreamWriter(LogFilePath, true, Encoding.UTF8))
            {
                stream.Write(GenerateMessage(message, Params));
            }
        }
        public void WriteLine(string message = "", params object[] Params)
        {
            using(var stream = new StreamWriter(LogFilePath, true, Encoding.UTF8))
            {
                stream.WriteLine(GenerateMessage(message, Params));
            }
        }

        private string GenerateMessage(in string message, object[] Params)
        {
            System.Diagnostics.StackFrame caller = new System.Diagnostics.StackFrame(2);
            List<string> messageList = new List<string>();
            string number = "Line " + caller.GetFileLineNumber().ToString();
            string date = string.Format("{0}.{1}", DateTime.Now, DateTime.Now.Millisecond);
            string optionMessage = string.Format(message, Params);

            return string.Format(MessageFormat, date, WriterMode, caller.GetMethod().Name, number, Param.HasEnteredMessage, Param.HasExitMessage, optionMessage);
        }
    }

    /// <summary>
    /// リリースモードで使われる空クラス．
    /// </summary>
    public class DebugWriterEmpty : Writer
    {
        public void Write(string message = "", params object[] Params) { }
        public void WriteLine(string message = "", params object[] Params) { }
    }
}
