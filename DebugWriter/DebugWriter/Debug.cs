using System;
using System.Collections.Generic;
using System.IO;
using XMLStreamWrapper;

namespace DebugWriterLib
{
    public enum Mode
    {
        Debug,
        Error,
        Status,
        Release
    }

    public struct DebugMessage
    {
        public Mode Mode;
        public int Number;
        public string Message;
    }


    public interface Writer
    {
        void OutputMessage(in int messageNumber, in string optionMessage = "");
    }

    public class DebugWriter
    {
        public Writer Writer { get; set; }

        public DebugWriter(in Mode mode, in string filePath, in int digit = 4)
        {
            Initialize(mode, filePath, digit);
        }

        private void Initialize(in Mode mode, in string filePath, in int digit)
        {
            switch (mode)
            {
                    case Mode.Debug:
                        Writer = new Debug(filePath, digit);
                        break;
                    case Mode.Error:
                        Writer = new Error(filePath, digit);
                        break;
                    case Mode.Status:
                        Writer = new Status(filePath, digit);
                        break;
                    case Mode.Release:
                        Writer = new Release(filePath, digit);
                        break;
                    default:
                        break;
            }
        }
    }

    public abstract class Base : Writer
    {
        protected Dictionary<int, string> DebugMessages;
        protected Dictionary<int, string> ErrorMessages;
        protected Dictionary<int, string> StatusMessages;
        protected Dictionary<int, string> ReleaseMessages;

        /// <summary>
        /// 空コンストラクタ
        /// </summary>
        public Base() { }

        public abstract void OutputMessage(in int messageNumber, in string optionMessage = "");

        protected void InitializeDictionary()
        {
            DebugMessages = new Dictionary<int, string>();
            ErrorMessages = new Dictionary<int, string>();
            StatusMessages = new Dictionary<int, string>();
            ReleaseMessages = new Dictionary<int, string>();
        }

        protected void ReadMessageXml(in string filePath, in int digit)
        {
            // Dictionaryの初期化
            InitializeDictionary();

            // ファイルが存在しない場合
            if (false == File.Exists(filePath))
            {
                return;
            }

            // Xmlファイルを読み込む
            List<DebugMessage> messages = new List<DebugMessage>();
            SimpleXmlSerializerWrapper<List<DebugMessage>> serializer = new SimpleXmlSerializerWrapper<List<DebugMessage>>(messages, filePath);
            messages = serializer.Deserialize();
            // Dictionaryにデータを詰める
            SetMessages(messages, digit);
        }

        public void SetMessages(in List<DebugMessage> messages, in int digit)
        {
            string digitFormat = "D" + digit.ToString();

            foreach(DebugMessage message in messages)
            {
                switch (message.Mode)
                {
                    case Mode.Error:
                        DebugMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Error] " + message.Message);
                        ErrorMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Error] " + message.Message);
                        break;
                    case Mode.Status:
                        DebugMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Status] " + message.Message);
                        StatusMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Status] " + message.Message);
                        break;
                    case Mode.Release:
                        DebugMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Release] " + message.Message);
                        ReleaseMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Release] " + message.Message);
                        break;
                    default:
                        break;
                }
            }
        }

        protected void Output(in Dictionary<int, string> targetMessages, in int messageNumber, in string optionMessage)
        {
            string message;
            string date;
            DateTime dateTime = DateTime.Now;
            date = "[" + dateTime + "." + dateTime.Millisecond + "]";

            if(false == targetMessages.TryGetValue(messageNumber, out message))
            {
                return;
            }

            if("" != optionMessage)
            {
                message = message + " : " + optionMessage;
            }

            Console.WriteLine(date + message);
        }

        public void SampleXmlFile(in string filePath)
        {
            List<DebugMessage> messages = new List<DebugMessage>();
            messages.Add(setMessage(Mode.Error, 0, "Success"));
            messages.Add(setMessage(Mode.Error, 1, "Error1 サンプルエラーです"));
            messages.Add(setMessage(Mode.Error, 2, "Error2 サンプルエラーです"));
            messages.Add(setMessage(Mode.Error, 3, "Error3 サンプルエラーです"));
            messages.Add(setMessage(Mode.Error, 4, "Error4 サンプルエラーです"));
            messages.Add(setMessage(Mode.Error, 5, "Error5 サンプルエラーです"));
            messages.Add(setMessage(Mode.Status, 1000, "Success"));
            messages.Add(setMessage(Mode.Status, 1001, "Status1 サンプルステータスです"));
            messages.Add(setMessage(Mode.Status, 1002, "Status2 サンプルステータスです"));
            messages.Add(setMessage(Mode.Status, 1003, "Status3 サンプルステータスです"));
            messages.Add(setMessage(Mode.Status, 1004, "Status4 サンプルステータスです"));
            messages.Add(setMessage(Mode.Status, 1005, "Status5 サンプルステータスです"));

            SimpleXmlSerializerWrapper<List<DebugMessage>> serializer = new SimpleXmlSerializerWrapper<List<DebugMessage>>(messages, "sample.xml");
            serializer.Serialize();
        }

        private DebugMessage setMessage(in Mode mode, in int number, in string message)
        {
            DebugMessage debugMessage = new DebugMessage();
            debugMessage.Mode = mode;
            debugMessage.Number = number;
            debugMessage.Message = message;

            return debugMessage;
        }
    }

    public class Debug : Base
    {
        public Debug(in string filePath, in int digit)
        {
            ReadMessageXml(filePath, digit);
        }

        public override void OutputMessage(in int messageNumber, in string optionMessage = "")
        {
            Output(DebugMessages, messageNumber, optionMessage);
        }
    }

    public class Error : Base
    {
        public Error(in string filePath, in int digit)
        {
            ReadMessageXml(filePath, digit);
        }

        public override void OutputMessage(in int messageNumber, in string optionMessage = "")
        {
            Output(ErrorMessages, messageNumber, optionMessage);
        }
    }

    public class Status : Base
    {
        public Status(in string filePath, in int digit)
        {
            ReadMessageXml(filePath, digit);
        }

        public override void OutputMessage(in int messageNumber, in string optionMessage = "")
        {
            Output(StatusMessages, messageNumber, optionMessage);
        }
    }

    public class Release : Base
    {
        public Release(in string filePath, in int digit)
        {
        }

        public override void OutputMessage(in int messageNumber, in string optionMessage = "")
        {
        }
    }
}
