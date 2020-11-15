using System;
using System.Collections.Generic;
using System.IO;
using XMLStreamWrapper;

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

    public class Base
    {
        protected struct DebugMessage
        {
            public Mode Mode;
            public int Number;
            public string Message;
        }

        private Dictionary<int, string> DebugMessages;
        private Dictionary<int, string> ErrorMessages;
        private Dictionary<int, string> StatusMessages;
        private Dictionary<int, string> ReleaseMessages;

        /// <summary>
        /// 空コンストラクタ
        /// </summary>
        public Base() { }

        protected void InitializeDictionary()
        {
            DebugMessages = new Dictionary<int, string>();
            ErrorMessages = new Dictionary<int, string>();
            StatusMessages = new Dictionary<int, string>();
            ReleaseMessages = new Dictionary<int, string>();
        }

        protected void ReadMessageXml(in string filePath)
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
            SetMessages(messages);
        }

        protected void SetMessages(in List<DebugMessage> messages)
        {
            foreach(DebugMessage message in messages)
            {
                switch (message.Mode)
                {
                    case Mode.Debug:
                        DebugMessages.Add(message.Number, message.Message);
                        break;
                    case Mode.Error:
                        ErrorMessages.Add(message.Number, message.Message);
                        break;
                    case Mode.Status:
                        StatusMessages.Add(message.Number, message.Message);
                        break;
                    case Mode.Release:
                        ReleaseMessages.Add(message.Number, message.Message);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public class DebugWriter : Base, Writer
    {
        public void OutputMessage()
        {
        }
    }

    public class ErrorWriter : Base, Writer
    {
        public void OutputMessage()
        {
        }
    }

    public class StatusWriter : Base, Writer
    {
        public void OutputMessage()
        {
        }
    }

    public class ReleaseWriter : Base, Writer
    {
        public void OutputMessage()
        {
        }
    }
}
