using System;
using DebuggerLib;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger debugger = new Debugger(Mode.Debug);
            debugger.EnterWriteLine("Debug Test");
            debugger.DebugWriteLine("DEBUG");
            debugger.ErrorWriteLine("ERROR");
            debugger.StatusWriteLine("STATUS");
            debugger.ExitWriteLine("Debug Test");
            debugger = new Debugger(Mode.Error);
            debugger.EnterWriteLine("Error Test");
            debugger.DebugWriteLine("DEBUG");
            debugger.ErrorWriteLine("ERROR");
            debugger.StatusWriteLine("STATUS");
            debugger.ExitWriteLine("Error Test");
            debugger = new Debugger(Mode.Trace);
            debugger.EnterWriteLine("Trace Test");
            debugger.DebugWriteLine("DEBUG");
            debugger.ErrorWriteLine("ERROR");
            debugger.StatusWriteLine("STATUS");
            debugger.ExitWriteLine("Trace Test");
            debugger = new Debugger(Mode.Status);
            debugger.EnterWriteLine("Status Test");
            debugger.DebugWriteLine("DEBUG");
            debugger.ErrorWriteLine("ERROR");
            debugger.StatusWriteLine("STATUS");
            debugger.ExitWriteLine("Status Test");
            debugger = new Debugger(Mode.Release);
            debugger.EnterWriteLine("Release Test");
            debugger.DebugWriteLine("DEBUG");
            debugger.ErrorWriteLine("ERROR");
            debugger.StatusWriteLine("STATUS");
            debugger.ExitWriteLine("Release Test");
            debugger = new Debugger(Mode.All);
            Sample(debugger);
        }

        static void Sample(Debugger debugger)
        {
            debugger.EnterWriteLine("Sample Test");
            debugger.DebugWriteLine("DEBUG");
            debugger.ErrorWriteLine("ERROR");
            debugger.StatusWriteLine("STATUS");
            debugger.ExitWriteLine("Sample Test");
        }
    }
}
