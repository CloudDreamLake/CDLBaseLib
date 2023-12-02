using CDLLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiDownloader
{
    public class Downloader
    {
        private Logger logger;
        List<Thread> tasks;
        public Downloader()
        {
            logger = new Logger();
        }
        public Downloader(Logger logger)
        {
            this.logger = logger;
        }
        public Downloader download(string url, string file_name)
        {
            Thread downloadTask = new Thread(new FileDownloader(logger, url, file_name).download);

            return this;
        }
    }

    internal class FileDownloader
    {
        private Logger logger;
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
        }

        public void download()
        {

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
