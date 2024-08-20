using CDLLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloaderFileMerger
{
    public class FileMergerConfigure
    {
        private string save_path = "multi.download";
        private int index = 0;

        public string Save_path { get => save_path; set => save_path = value; }
        public int Index { get => index; set => index = value; }

        public FileMergerConfigure() { }
    }

    public class FileMerger
    {
        private readonly FileMergerConfigure configure;
        private bool run_flag = false;
        private Thread? thread;
        private FileMerger(FileMergerConfigure configure)
        {
            this.configure = configure;
        }

        public void Main ()
        {

        }

        public static FileMerger Create(FileMergerConfigure config)
        {
            FileMerger fileMerger = new FileMerger(config)
            {
                run_flag = true,
            };
            fileMerger.thread = new(fileMerger.Main);
            fileMerger.thread.Name = "Merger Thread#" + config.Index.ToString();
            fileMerger.thread.Start();

            return fileMerger;
        }

        public static void Intro()
        {
            Logger logger = Logger.Create();
            logger.Info("CDL File Merger (default)");
        }

        public static bool Check()
        {
            return true;
        }

        public void Stop ()
        {
            run_flag = false;
            if(thread == null)
            {
                Logger.Create().Error("File Merger haven't start.");
                throw new Exception("File Merger haven't start.");
            }
            thread.Join();
        }
    }
}
