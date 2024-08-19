using CDLLogger;

namespace MultiDownloader
{
    public class Downloader
    {
        private class DownloadInfo(string url, string file_name)
        {
            public string url = url;
            public string file_name = file_name;
        }
        private readonly List<DownloadInfo> download_infos = [];
        private readonly Logger logger;
        private readonly List<Thread> tasks = [];
        private Thread? DownLoaderThread = null;
        private bool run_flag = true;
        private int index = 0;
        public Downloader()
        {
            logger = new Logger();
        }
        public Downloader(Logger logger)
        {
            this.logger = logger;
        }
        public Downloader Download(string url, string file_name)
        {
            lock(download_infos)
            {
                download_infos.Add(new DownloadInfo(url, file_name));
                return this;
            }
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
                            Thread downloadTask = new Thread(new FileDownloader(logger, download_info.url, download_info.file_name).Download);
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
            return Create(new Logger());
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
                logger.Info("Close DownLoader");
                DownLoaderThread.Join();
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

    internal class FileDownloader
    {
        private readonly Logger logger;
        private string url;
        private string file_name;
        private FileStream filestream;
        private StreamWriter filewriter;
        public FileDownloader(Logger logger, string url, string file_name)
        {
            this.logger = logger;
            this.url = url;
            this.file_name = file_name;
            filestream = File.Open(file_name, FileMode.OpenOrCreate);
            filewriter = new StreamWriter(filestream);

            logger.Info("Download " + url + " to file <" + file_name + ">");
        }

        public void Download()
        {
            logger.Info("Start Download " + url);
        }
    }

    internal class DownloadUnit
    {

        private Logger logger;
        private string url;
        private StreamWriter writer;
        private int l;
        private int r;
        public DownloadUnit(Logger logger, string url, StreamWriter writer, int l, int r)
        {
            this.l = l;
            this.writer = writer;  
            this.url = url;
            this.r = r;
            this.logger = logger;
        }
        public void start()
        {

        }
    }
}
