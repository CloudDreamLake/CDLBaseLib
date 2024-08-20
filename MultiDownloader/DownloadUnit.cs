using CDLLogger;

namespace MultiDownloader
{
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
