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
            Console.Write(s);
            return this;    
        }
        public ColorPrinter print(ConsoleColor fr, ConsoleColor bg, string s)
        {
            Console.ForegroundColor = fr;
            Console.BackgroundColor = bg;
            Console.Write(s);
            Console.ForegroundColor = LsFr;
            Console.BackgroundColor = LsBg;
            return this;
        }
        public ColorPrinter set(ConsoleColor fr, ConsoleColor fg)
        {
            LsFr = fr;
            LsBg = fg;
            return this;
        }
        public void close()
        {
            Console.ForegroundColor = LsFr;
            Console.BackgroundColor = LsBg;
        }
    }
}