using CDLLogger;
using MultiDownloader;

namespace Test
{
    internal class Program
    {
        public static readonly Logger logger = Logger.Create();
        static void Main(string[] args)
        {
            Downloader downloader = Downloader.Create(logger);

            new Downloader(logger).Delete();

            downloader.Download("lll", "sss.ccc");

            downloader.Delete();

            logger.Close();
        }
    }
}