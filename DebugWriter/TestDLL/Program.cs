using System;
using DebugWriterLib;

namespace TestDLL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Debug debug = new Debug("Sample.xml", 4);
            debug.SampleXmlFile("Sample.xml");
            DebugWriter writer = new DebugWriter(Mode.Debug, "Sample.xml");

            for (int count = 0; count < 2000; count++)
            {
                writer.Writer.OutputMessage(count, "TEST");
            }
        }
    }
}
