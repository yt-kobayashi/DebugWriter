using System;
using DebuggerLib;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger debugger = new Debugger(Mode.Debug);
            debugger.Enter.WriteLine("Debug Test");
            debugger.Debug.WriteLine("DEBUG");
            debugger.Error.WriteLine("ERROR");
            debugger.Status.WriteLine("STATUS");
            debugger.Exit.WriteLine("Debug Test");
            debugger = new Debugger(Mode.Error);
            debugger.Enter.WriteLine("Error Test");
            debugger.Debug.WriteLine("DEBUG");
            debugger.Error.WriteLine("ERROR");
            debugger.Status.WriteLine("STATUS");
            debugger.Exit.WriteLine("Error Test");
            debugger = new Debugger(Mode.Trace);
            debugger.Enter.WriteLine("Trace Test");
            debugger.Debug.WriteLine("DEBUG");
            debugger.Error.WriteLine("ERROR");
            debugger.Status.WriteLine("STATUS");
            debugger.Exit.WriteLine("Trace Test");
            debugger = new Debugger(Mode.Status);
            debugger.Enter.WriteLine("Status Test");
            debugger.Debug.WriteLine("DEBUG");
            debugger.Error.WriteLine("ERROR");
            debugger.Status.WriteLine("STATUS");
            debugger.Exit.WriteLine("Status Test");
            debugger = new Debugger(Mode.Release);
            debugger.Enter.WriteLine("Release Test");
            debugger.Debug.WriteLine("DEBUG");
            debugger.Error.WriteLine("ERROR");
            debugger.Status.WriteLine("STATUS");
            debugger.Exit.WriteLine("Release Test");
        }
    }
}
