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
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("テスト開始 {0}", 1);
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
            debugger = new Debugger(Mode.Debug, Param.FORMAT_SIMPLE);
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("テスト開始");
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
            debugger = new Debugger(Mode.Debug, Param.FORMAT_DEFAULT | Format.CallerLine);
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("テスト開始");
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
            debugger = new Debugger(Mode.Error);
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
            debugger = new Debugger(Mode.Status);
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
            debugger = new Debugger(Mode.Trace);
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
            debugger = new Debugger(Mode.Release);
            debugger.Enter.WriteLine();
            debugger.Debug.WriteLine("デバッグメッセージ");
            debugger.Error.WriteLine("エラーメッセージ");
            debugger.Status.WriteLine("ステータスメッセージ");
            debugger.Exit.WriteLine();
        }
    }
}
