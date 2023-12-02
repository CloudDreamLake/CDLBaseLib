using CDLLogger;
using MultiDownloader;

namespace Test
{
    internal class Program
    {
        static Logger logger = new();
        static void Main(string[] args)
        {
            try
            {
                MultiDownload downloader = new();
                downloader.AddDown("https://download.jetbrains.com.cn/go/goland-2023.2.4.exe", "D:\\迅雷下载", 0, "goland.exe");
                downloader.StartDown();

            }
            catch (Exception ex)
            {

            }
        }
        public static void Print(string message)
        {
            logger.Error(message);
        }
    }
}