using CDLLogger;
using MultiDownloader;

namespace Test
{
    internal class Program
    {
        static Logger logger = new();
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