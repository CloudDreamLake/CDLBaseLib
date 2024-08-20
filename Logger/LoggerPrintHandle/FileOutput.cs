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
            fileStream.Position = fileStream.Length;
            writer.WriteLine("---------------------------------------------------");
        }
        public FileOutput() : this(DateTime.Now.ToString("yy-MM-dd") + ".log")
        {

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

        public void Print(Logger.LogInfo info)
        {
            string? FuncName = info.stackTrace?.GetFrame(2)?.GetMethod()?.Name;
            if (FuncName == null)
            {
                FuncName = "<Unknown>";
            }
            //string? TName = Thread.CurrentThread.Name;
            //string TId = Environment.CurrentManagedThreadId.ToString();
            if (info.TName == null || (info.TId == "1" && info.TName.Length == 0))
            {
                info.TName = "MainThread";
            }
            string time = System.DateTime.Now.ToString("yy-MM-dd HH:mm:ss:ffff");
            writer.WriteLine("[" + time + "|" + Logger.LogLevel2String[info.level] + "](" + FuncName + "|" + info.TName + ":" + info.TId + ") " + info.message);
        }
        public void close()
        {
            Console.WriteLine("    Close File:" +  FileName);
            writer.Close();
            fileStream.Close();
            Console.WriteLine("    Close Successfully.");
        }
    }
}
