using CDLLogger.LoggerPrintHandle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDLLogger
{

    public class Logger
    {
        public class LogInfo(LoggerLevel level, string message, StackTrace stackTrace, string? TName, string TId)
        {
            public LoggerLevel level = level;
            public string message = message;
            public StackTrace stackTrace = stackTrace;
            public string? TName = TName;
            public string TId = TId;
        }
        private List<LogInfo> logInfos = [];
        private LoggerLevel print_level;
        private object locker = new object();
        private bool run_flag = true;
        private Thread? LogThread = null; 
        public static Dictionary<LoggerLevel, string> LogLevel2String = new Dictionary<LoggerLevel, string>()
        {
            { LoggerLevel.info, "Info " },
            { LoggerLevel.warn, "Warn " },
            { LoggerLevel.debug, "Debug" },
            { LoggerLevel.error, "Error" },
            { LoggerLevel.fatal, "Fatal" },
        };
        public List<LoggerPrintHandle.LoggerPrintHandle> outputs = new List<LoggerPrintHandle.LoggerPrintHandle>()
        {
            new ConsoleOutput(),
            new FileOutput(),
            new FileOutput("log.log")
        };
        public Logger()
        {
            print_level = LoggerLevel.info;
            Console.WriteLine("[CDL LOGGER] R1.0 for Windows.");
            Console.WriteLine("Prepare for Logger.");
            Console.WriteLine("Handle List:");
            foreach(var output in outputs) Console.WriteLine("\t" + output.getName());
            Console.WriteLine("");
        }
        public Logger(LoggerLevel print_level)
        {
            this.print_level = print_level;
        }
        public void Main()
        {
            while(run_flag || logInfos.Count > 0)
            {
                if (logInfos.Count > 0)
                {
                    LogInfo logInfo = logInfos.First();
                    if (logInfo.level >= print_level)
                    {
                        outputs.ForEach(printHandle =>
                        {
                            printHandle.Print(logInfo);
                        });
                    }
                    logInfos.Remove(logInfo);
                    continue;
                }
                Thread.Sleep(100);
            }
        }
        public static Logger Create()
        {
            Logger logger = new Logger();

            logger.LogThread = new Thread(logger.Main);

            logger.LogThread.Name = "Log Thread";

            logger.LogThread.Start();

            return logger;
        }
        public void SetPrintLevel(LoggerLevel print_level)
        {
            this.print_level = print_level;
        }
        private void Log(LoggerLevel loggerLevel, string message)
        {
            if( loggerLevel >= print_level)
            {
                logInfos.Add(new(
                    loggerLevel, 
                    message, 
                    new StackTrace(), 
                    Thread.CurrentThread.Name,
                    Environment.CurrentManagedThreadId.ToString()
                ));
            }
        }
        public Logger Log(string message)
        {
            Log(print_level, message);
            return this;
        }
        public Logger Info(string message)
        {
            Log(LoggerLevel.info, message);
            return this;
        }
        public Logger Debug(string message)
        {
            Log(LoggerLevel.debug, message);
            return this;
        }
        public Logger Error(string message)
        {
            Log(LoggerLevel.error, message);
            return this;
        }
        public Logger Fatal(string message)
        {
            Log(LoggerLevel.fatal, message);
            return this;
        }
        public Logger Warn(string message)
        {
            Log(LoggerLevel.warn, message);
            return this;
        }
        public Logger RegistPrintHandle(LoggerPrintHandle.LoggerPrintHandle print_handle)
        {
            outputs.Add(print_handle);

            Console.WriteLine("Append Handle:", print_handle.getName());
            Console.WriteLine("Handle List:");
            foreach (var output in outputs) Console.WriteLine("\t" + output.getName());
            return this;
        }
        public Logger ClearPrintHandle()
        {
            outputs.ForEach(output => { output.close(); Console.WriteLine("----------------------"); });
            outputs.Clear();
            Console.WriteLine("Clear the handles.");
            return this;
        }
        public void Close()
        {
            run_flag = false;
            if(LogThread == null)
            {
                throw new Exception("Log Thread");
            }
            LogThread.Join();

            Console.WriteLine("Prepared to be closed.");
            Console.WriteLine("Close Output Handles.");
            Console.WriteLine("Handle Message:\n");

            Console.WriteLine("----------------------");

            ClearPrintHandle();

            Console.WriteLine("\nClose Completed.");
            Console.WriteLine("Thanks for using.");
        }
    }

    public enum LoggerLevel
    {
        None = 0,
        debug = 1,
        info = 2,
        warn = 3,
        error = 4,
        fatal = 5
    }

}
