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
            Console.WriteLine("    Date back to original color.");
            printer.close();
            Console.WriteLine("    Close Successfully");
        }

        public void Print(Logger.LogInfo info)
        {
            string? FuncName = info.stackTrace?.GetFrame(2)?.GetMethod()?.Name;
            if (FuncName == null)
            {
                FuncName = "<Unknown>";
            }
            if(info.TName == null || (info.TId == "1" && info.TName.Length == 0))
            {
                info.TName = "MainThread";
            }
            string time = System.DateTime.Now.ToString("yy-MM-dd HH:mm:ss:fff");
            printer.print("[")
                .print(LogLevel2Color[info.level], ConsoleColor.Black, time)
                .print("|")
                .print(LogLevel2Color[info.level], ConsoleColor.Black, Logger.LogLevel2String[info.level])
                .print("](")
                .print(ConsoleColor.Magenta, ConsoleColor.Black, FuncName)
                .print("|")
                .print(ConsoleColor.Magenta, ConsoleColor.Black, info.TName)
                .print(":")
                .print(ConsoleColor.Magenta, ConsoleColor.Black, info.TId)
                .print(") ")
                .print(ConsoleColor.Cyan, ConsoleColor.Black, info.message)
                .print("\n")
                .close();
        }

        string LoggerPrintHandle.getName()
        {
            return "CDLHandle: Stdout";
        }
    }
}
