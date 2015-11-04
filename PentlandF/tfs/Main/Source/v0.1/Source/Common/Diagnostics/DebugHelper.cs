using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common.Diagnostics
{
    public sealed class DebugHelper
    {
        private static readonly Logger Log = LogManager.GetPerfMonLogger();
        public static void LogToDebug(string message)
        {
            Debug.WriteLine(message);
        }

        public static void LogToFile(string message)
        {
            Log.Debug(message);
        }

        public static void LogToBoth(string message)
        {
            Debug.WriteLine(message);
            Log.Debug(message);
        }
    }
}
