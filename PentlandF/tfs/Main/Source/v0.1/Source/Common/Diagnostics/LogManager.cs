using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common.Diagnostics
{
    public class LogManager
    {
        public static Logger GetLogger(Type logger)
        {
            return new Logger(logger);
        }

        public static Logger GetPerfMonLogger()
        {
            return new Logger("perfMon");
        }
    }
}
