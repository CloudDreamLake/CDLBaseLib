using CDLLogger;

namespace MultiDownloader
{
    public class Downloader
    {
        static Downloader? Instance = null;
        static Object locker = new();
        public class DownloadInfo(Uri url, string file_name)
        {
            public Uri url = url;
            public string file_name = file_name;
        }
        private readonly List<DownloadInfo> download_infos = [];
        private readonly Logger logger;
        private readonly List<Thread> tasks = [];
        private Thread? DownLoaderThread = null;
        private bool run_flag = true;
        private int index = 0;
        public Downloader() : this(Logger.Create()) { }
        public Downloader(Logger logger)
        {
            lock(locker)
            {
                if(Instance == null)
                {
                    this.logger = logger;
                    Instance = this;
                }
                else
                {
                    logger.Error("Too much Downloader Exists!!!");
                    this.logger = Instance.logger;
                }
            }
        }
        
        public Downloader Download(Uri url, string file_name)
        {
            lock(download_infos)
            {
                download_infos.Add(new DownloadInfo(url, file_name));
                return this;
            }
        }
        public Downloader Download(string url, string file_name)
        {
            return Download(new Uri(url), file_name);
        }
        public void Main()
        {
            while(run_flag || tasks.Count > 0 || download_infos.Count > 0)
            {
                int tmp_idx = -1;
                for (int i = 0; i < tasks.Count; i++)
                {
                    Thread task= tasks[i];
                    if(!task.IsAlive)
                    {
                        tmp_idx = i;
                        break;
                    }
                }
                if(tmp_idx >= 0)
                {
                    tasks.Remove(tasks[tmp_idx]);
                } 
                //tasks.ForEach(task =>
                //{
                //    if (!task.IsAlive)
                //    {
                //        tasks.Remove(task);
                //    }
                //});

                lock(download_infos)
                {
                    download_infos.ForEach(download_info =>
                    {
                        if(download_info != null)
                        {
                            Thread downloadTask = new Thread(new FileDownloader(logger, download_info).Download);
                            downloadTask.Name = "DownLoad#" + (++index).ToString();
                            downloadTask.Start();
                            tasks.Add(downloadTask);
                        }
                    });
                    download_infos.Clear();
                }

                Thread.Sleep(100);
            }
        }

        public static Downloader Create()
        {
            return Create(Logger.Create());
        }
        public static Downloader Create(Logger logger)
        {
            logger.Info("Create DownLoader");
            Downloader downloader = new(logger);
            downloader.DownLoaderThread = new(downloader.Main);
            downloader.DownLoaderThread.Name = "DownLoad Thread";
            downloader.DownLoaderThread.Start();

            return downloader;
        }
        public void Delete()
        {
            Close();
            run_flag = false;
            if(DownLoaderThread != null)
            {
                DownLoaderThread.Join();
                logger.Info("Close DownLoader");
            }
            else
            {
                logger.Error("Have not run downloader thread");
            }
        }
        public void Close()
        {
            tasks.ForEach(task =>
            {
                task.Join();
            });
        }
    }
}
