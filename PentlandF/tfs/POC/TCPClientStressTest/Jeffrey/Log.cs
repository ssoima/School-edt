using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jeffrey
{
    public static class Log
    {
        private static Logger logger { get; set; }

        private static Logger Logger()
        {
            if(null==logger)
            {
                logger=LogManager.GetLogger("logger");
            }
            return logger;

        }

        public static void Info(string message)
        {
            Logger().Info(message);
        }

        public static void Error(string message)
        {
            Logger().Error(message);
        }
    }
}
