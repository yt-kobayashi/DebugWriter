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
        Debug,
        Error,
        Status,
        Trace,
        Release,
        Enter,
        Exit
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
        void Write(in string message = "", params object[] Params);
        void WriteLine(in string message = "", params object[] Params);
    }

    /// <summary>
    /// デバッグメッセージを出力するクラス．
    /// モードを設定することで出力を制御できる．
    /// </summary>
    /// <remarks>
    /// モード<br/>
    /// Debug       :   全てのメッセージが表示される．
    /// Error       :   Errorメッセージのみが表示される．
    /// Status      :   Statusメッセージのみが表示される．
    /// Trace       :   Trace, Enter, Exitメッセージが表示される．
    /// Release     :   全てのメッセージが表示されない．
    /// Enter       :   Enter, Exitメッセージが表示される．
    /// Exit        :   Enter, Exitメッセージが表示される
    /// </remarks>
    public class Debugger
    {
        public Writer Debug { get; set; }
        public Writer Error { get; set; }
        public Writer Status { get; set; }
        public Writer Enter { get; set; }
        public Writer Exit { get; set; }

        private Dictionary<Mode, bool[]> ModeParam { get; set; }

        public Debugger(in Mode mode, in Format format = Param.FORMAT_DEFAULT)
        {
            ModeParam = new Dictionary<Mode, bool[]>();
            ModeParam.Add(Mode.Debug, new bool[] { true, true, true, true});
            ModeParam.Add(Mode.Error, new bool[] { false, true, false, false});
            ModeParam.Add(Mode.Trace, new bool[] { false, false, true, true});
            ModeParam.Add(Mode.Status, new bool[] { false, false, true, false});
            ModeParam.Add(Mode.Enter, new bool[] { false, false, false, true});
            ModeParam.Add(Mode.Exit, new bool[] { false, false, false, true});
            ModeParam.Add(Mode.Release, new bool[] { false, false, false, false});

            Initialize(mode, format);
        }

        public void Initialize(in Mode mode, in Format format)
        {
            bool[] param = ModeParam[mode];

            Debug = SetWriter(param[0], Mode.Debug, format);
            Error = SetWriter(param[1], Mode.Error, format);
            Status = SetWriter(param[2], Mode.Status, format);
            Enter = SetWriter(param[3], Mode.Enter, format | Format.EnterMessage);
            Exit = SetWriter(param[3], Mode.Exit, format | Format.ExitMessage);
        }

        public Writer SetWriter(in bool enable, in Mode mode, in Format format)
        {
            Writer debugWriter = new DebugWriter(mode, format);

            if (false == enable)
            {
                debugWriter = new DebugWriterEmpty();
            }

            return debugWriter;
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

        public void Write(in string message = "", params object[] Params) 
        {
            if(Mode.Release.ToString() == WriterMode)
            {
                return;
            }

            using(var stream = new StreamWriter(LogFilePath, true, Encoding.UTF8))
            {
                stream.Write(GenerateMessage(message, Params));
            }
        }
        public void WriteLine(in string message = "", params object[] Params)
        {
            if(Mode.Release.ToString() == WriterMode)
            {
                return;
            }

            using(var stream = new StreamWriter(LogFilePath, true, Encoding.UTF8))
            {
                stream.WriteLine(GenerateMessage(message, Params));
            }
        }

        private string GenerateMessage(in string message, object[] Params)
        {
            System.Diagnostics.StackFrame caller = new System.Diagnostics.StackFrame(3);
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
        public void Write(in string message = "", params object[] Params) { }
        public void WriteLine(in string message = "", params object[] Params) { }
    }
}
