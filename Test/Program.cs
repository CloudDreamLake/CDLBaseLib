using CDLLogger;
using MultiDownloader;

namespace Test
{
    internal class Program
    {
        static Logger logger = new();
        static void Main(string[] args)
        {
            Print("test");
            logger.close();
        }
        public static void Print(string message)
        {
            logger.Error(message);
        }
    }
}