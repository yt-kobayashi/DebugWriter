using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using XMLStreamWrapper;

namespace DebuggerLib
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

    public class Debugger
    {
        public Writer Writer { get; set; }

        public Debugger(in Mode mode, in string filePath, in int digit = 4)
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

        public void XlsxToXml(in string xlsxFilePath, in string xmlFilePath, in int startRowNum = 2)
        {
            List<DebugMessage> messages = new List<DebugMessage>();                     // デバッグメッセージリスト
            string extension = xlsxFilePath.Substring(xlsxFilePath.Length - 5);         // 拡張子
            SimpleXmlSerializerWrapper<List<DebugMessage>> serialize = new SimpleXmlSerializerWrapper<List<DebugMessage>>(messages, xmlFilePath);

            // ファイルが存在しない場合は変換終了
            if(false == File.Exists(xlsxFilePath))
            {
                Console.WriteLine("Xlsx file not found!");
                return;
            }

            // 拡張子がxlsxかXlsxのもの出なければ変換終了
            if(".xlsx" != extension && ".Xlsx" != extension)
            {
                Console.WriteLine("Not xlsx or Xlsx file!");
                return;
            }

            // ExcelファイルからDebugMessageListを生成できなければ変換終了
            if(false == SetDebugMessageList(xlsxFilePath, startRowNum, ref messages))
            {
                Console.WriteLine("Failed set DebugMessage list!");
                return;
            }

            serialize.Serialize();
        }

        private bool SetDebugMessageList(in string xlsxFilePath, in int startRowNum, ref List<DebugMessage> messages)
        {
            // ワークブック取得
            XLWorkbook workbook = new XLWorkbook(xlsxFilePath);
            foreach(IXLWorksheet worksheet in workbook.Worksheets)
            {
                // 最終行番号を取得
                int lastRowNum = worksheet.LastRowUsed().RowNumber();

                // 取得を開始する行が範囲外なら変換終了
                if(startRowNum < 1 || lastRowNum <= startRowNum)
                {
                    Console.WriteLine("Not in the range of 1 to last row number!");
                    return false;
                }

                // 各要素を取得
                // 1行目は見出しと考えて処理をしない
                for (int rowNum = startRowNum; rowNum <= lastRowNum; rowNum++)
                {
                    DebugMessage debugMessage = new DebugMessage();         // デバッグメッセージ

                    // パラメータが入っているセルを取得
                    IXLCell mode = worksheet.Cell(rowNum, 1);
                    IXLCell num = worksheet.Cell(rowNum, 2);
                    IXLCell message = worksheet.Cell(rowNum, 3);

                    // デバッグメッセージを取得できなかった場合は次の処理に移る
                    if (false == SetDebugMessage(mode.Value.ToString(), num.Value.ToString(), message.Value.ToString(), ref debugMessage))
                    {
                        Console.WriteLine("Failed set DebugMessage!");
                        continue;
                    }

                    // デバッグメッセージをリストに追加する
                    messages.Add(debugMessage);
                }
            }

            return true;
        }

        private bool SetDebugMessage(in string mode, in string num, in string message, ref DebugMessage result)
        {
            Mode tempMode = Mode.Debug;
            int tempNumber = 0;
            string tempMessage = "DebugMessage";

            // モード選択
            switch (mode)
            {
                case "Debug":
                    tempMode = Mode.Debug;
                    break;
                case "Error":
                    tempMode = Mode.Error;
                    break;
                case "Status":
                    tempMode = Mode.Status;
                    break;
                case "Release":
                    tempMode = Mode.Release;
                    break;
                default:
                    Console.WriteLine("Mode not found! : " + mode);
                    return false;
                    break;
            }

            // メッセージ番号をstringからintへ変換
            if(false == int.TryParse(num, out tempNumber))
            {
                Console.WriteLine("Not number!");
                return false;
            }

            // パラメータ設定
            result.Mode = tempMode;
            result.Number = tempNumber;
            result.Message = tempMessage;

            return true;
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

        protected void SetMessages(in List<DebugMessage> messages, in int digit)
        {
            string digitFormat = "D" + digit.ToString();

            foreach(DebugMessage message in messages)
            {
                switch (message.Mode)
                {
                    case Mode.Debug:
                        DebugMessages.Add(message.Number, "[" + message.Number.ToString(digitFormat) + "]" + "[Debug] " + message.Message);
                        break;
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
