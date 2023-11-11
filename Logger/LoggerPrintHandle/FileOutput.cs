using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDLLogger.LoggerPrintHandle
{
    internal class FileOutput : LoggerPrintHandle
    {
        private string FileName;
        private FileStream fileStream;
        private StreamWriter writer;
        public FileOutput(string FileName)
        {
            this.FileName = FileName;
            fileStream = new FileStream(FileName, FileMode.OpenOrCreate);
            writer = new StreamWriter(fileStream);
            fileStream.SetLength(0);
        }
        public FileOutput()
        {
            FileName = DateTime.Now.ToString("yy-MM-dd") + ".log";
            fileStream = new FileStream(FileName, FileMode.OpenOrCreate);
            writer = new StreamWriter(fileStream);
            fileStream.SetLength(0);
        }
        public FileOutput(FileStream fileStream)
        {
            this.fileStream = fileStream;
            FileName = fileStream.Name.Split(".").Last();
            writer = new StreamWriter(fileStream);
            fileStream.SetLength(0);
        }
        public string getName()
        {
            return "CDLHandle: File Output [To \"" + FileName + "\"]";
        }

        public void Print(LoggerLevel level, string message, StackTrace stackTrace)
        {

            string FuncName = stackTrace.GetFrame(2).GetMethod().Name;
            string TName = Thread.CurrentThread.Name;
            string TId = Thread.CurrentThread.ManagedThreadId.ToString();
            if (TId == "1" && TName.Length == 0)
            {
                TName = "MainThread";
            }
            string time = System.DateTime.Now.ToString("yy-MM-dd HH:mm:ss:ffff");
            writer.WriteLine("[" + time + "|" + Logger.LogLevel2String[level] + "](" + FuncName + "|" + TName + ":" + TId + ")" + message);
        }
        public void close()
        {
            Console.WriteLine("Close File:" +  FileName);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("Close Successfully.\n");
        }
    }
}
