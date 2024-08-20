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

            downloader.Download("https://github.com/CloudDreamLake/CDLBaseLib/releases/download/Rel/CDLLogger.dll", "CDLLogger.dll.down");
            //downloader.Download("https://github.com/CloudDreamLake/CDLBaseLib/releases/download/Rel/CDLLogger.dll", ".");

            downloader.Delete();

            logger.Close();
        }
    }
}