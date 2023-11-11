using ColorPrint;
using System.Diagnostics;

namespace CDLLogger.LoggerPrintHandle
{
    internal class ConsoleOutput : LoggerPrintHandle
    {
        static readonly Dictionary<LoggerLevel, ConsoleColor> LogLevel2Color = new Dictionary<LoggerLevel, ConsoleColor>()
        {
            { LoggerLevel.None, ConsoleColor.White },
            { LoggerLevel.info, ConsoleColor.Blue },
            { LoggerLevel.warn, ConsoleColor.Yellow },
            { LoggerLevel.error, ConsoleColor.Red },
            { LoggerLevel.fatal, ConsoleColor.DarkRed }
        };
        private static ColorPrinter printer = new ColorPrinter();
        public ConsoleOutput() { }

        public void close()
        {
            Console.WriteLine("Date back to original color.");
            printer.close();
            Console.WriteLine("Close Successfully\n");
        }

        public void Print(LoggerLevel level, string message, StackTrace stackTrace)
        {
            string FuncName = stackTrace.GetFrame(2).GetMethod().Name;
            string TName = Thread.CurrentThread.Name;
            string TId = Environment.CurrentManagedThreadId.ToString();
            if(TId == "1" && TName.Length == 0)
            {
                TName = "MainThread";
            }
            string time = System.DateTime.Now.ToString("yy-MM-dd HH:mm:ss:fff");
            printer.print("[")
                .print(ConsoleColor.Blue, ConsoleColor.Black, time)
                .print("|")
                .print(LogLevel2Color[level], ConsoleColor.Black, Logger.LogLevel2String[level])
                .print("](")
                .print(ConsoleColor.Magenta, ConsoleColor.Black, FuncName)
                .print("|")
                .print(ConsoleColor.Magenta, ConsoleColor.Black, TName)
                .print(":")
                .print(ConsoleColor.Magenta, ConsoleColor.Black, TId)
                .print(")")
                .print(ConsoleColor.Cyan, ConsoleColor.Black, message);
            Console.Write("\n");
        }

        string LoggerPrintHandle.getName()
        {
            return "CDLHandle: Stdout";
        }
    }
}
