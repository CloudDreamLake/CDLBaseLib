namespace ColorPrint
{
    public class ColorPrinter
    {
        private ConsoleColor LsFr;
        private ConsoleColor LsBg;
        public ColorPrinter()
        {
            LsFr = Console.ForegroundColor;
            LsBg = Console.BackgroundColor;
        }
        public ColorPrinter print(string s)
        {
            Console.ForegroundColor = LsFr;
            Console.BackgroundColor = LsBg; 
            Console.Write(s);
            return this;    
        }
        public ColorPrinter print(ConsoleColor fr, ConsoleColor bg, string s)
        {
            Console.ForegroundColor = fr;
            Console.BackgroundColor = bg;
            Console.Write(s);
            return this;
        }
        public void close()
        {
            Console.ForegroundColor = LsFr;
            Console.BackgroundColor = LsBg;
        }
    }
}