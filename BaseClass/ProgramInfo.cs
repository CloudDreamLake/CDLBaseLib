using CDLLogger;

namespace BaseClass
{
    public class ProgramInfo
    {
        private Logger logger;
        public ProgramInfo()
        {
            logger = new Logger();
        }
        public ProgramInfo(Logger logger)
        {
            this.logger = logger;
        }
        public Logger GetLogger()
        {
            return logger;
        }
        public ProgramInfo SetLogger(Logger logger)
        {
            this.logger = logger;
            return this;
        }
        public override string ToString()
        {
            return "Info: Logger:" + logger;
        }
        public ProgramInfo copy()
        {
            ProgramInfo info = new ProgramInfo(logger);
            return info;
        }
    }
}