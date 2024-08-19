using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDLLogger.LoggerPrintHandle
{
    public interface LoggerPrintHandle
    {
        public string getName();
        public void Print(Logger.LogInfo info) ;
        public void close() ;
    }
}
