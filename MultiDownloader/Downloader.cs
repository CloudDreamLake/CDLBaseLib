using CDLLogger;

namespace MultiDownloader
{
    public class Downloader
    {
        private static Downloader? instance;
        private static Object locker = new Object();
        public class DownloadInfo(Uri url, string file_name)
        {
            public Uri url = url;
            public string file_name = file_name;
        }
        private readonly List<DownloadInfo> download_infos = [];
        private Logger? logger;
        private readonly List<Thread> tasks = [];
        private Thread? DownLoaderThread = null;
        private bool run_flag = true;
        private int index = 0;
        private Downloader() { }
        
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
            logger.Info("DownLoad Thread Start!");
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
                            Thread downloadTask = new(new FileDownloader(logger, download_info).Download)
                            {
                                Name = "DownLoad#" + (++index).ToString()
                            };
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
            if (instance == null)
            {
                lock (locker)
                {
                    if(instance == null)
                    {
                        logger.Info("Create DownLoader");
                        Downloader downloader = new()
                        {
                            logger = logger
                        };
                        downloader.DownLoaderThread = new(downloader.Main)
                        {
                            Name = "DownLoad Thread"
                        };
                        downloader.DownLoaderThread.Start();

                        return downloader;
                    }
                }
            }
            return instance;
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
