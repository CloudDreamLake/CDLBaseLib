using CDLLogger;
namespace Test
{
    internal class Program
    {
        static Logger logger = new();
        static void Main(string[] args)
        {
            try
            {

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