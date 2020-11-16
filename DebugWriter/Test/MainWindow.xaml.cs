using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DebuggerLib;
using Microsoft.Win32;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Debugger debugger = new Debugger(Mode.Debug);
            debugger.Debug.Write("テスト開始");
            debugger.Debug.Write("デバッグメッセージ");
            debugger.Error.Write("エラーメッセージ");
            debugger.Status.Write("ステータスメッセージ");
            debugger.Release.Write("リリースメッセージ");
            debugger = new Debugger(Mode.Error);
            debugger.Debug.Write("デバッグメッセージ");
            debugger.Error.Write("エラーメッセージ");
            debugger.Status.Write("ステータスメッセージ");
            debugger.Release.Write("リリースメッセージ");
            debugger = new Debugger(Mode.Status);
            debugger.Debug.Write("デバッグメッセージ");
            debugger.Error.Write("エラーメッセージ");
            debugger.Status.Write("ステータスメッセージ");
            debugger.Release.Write("リリースメッセージ");
            debugger = new Debugger(Mode.Release);
            debugger.Debug.Write("デバッグメッセージ");
            debugger.Error.Write("エラーメッセージ");
            debugger.Status.Write("ステータスメッセージ");
            debugger.Release.Write("リリースメッセージ");
        }
    }
}
