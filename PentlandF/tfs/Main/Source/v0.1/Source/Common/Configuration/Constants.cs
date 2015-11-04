using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.Common.Configuration
{
    public static class Constants
    {
        public static class AppSettings
        {
            public const string RunningEndpointUrl = "endpointUrl";
            public const string ExecutionEngineEndpointUrl = "executionEngineUrl";
            public const string ExecutionEngineWebAPIUrl = "executionAPIUrl";
            public const string ProcessPlanWebAPIUrl = "processplanAPIUrl";
            public const string WorkstationAPIUrl = "workstationAPIUrl";
        }

        public static class HubProxyNames
        {
            public const string ExecutionEngineComHub = "ExecutionEngineComHub";
            public const string TerminalExecutionHub = "TerminalExecutionConnectHub";
            public const string ConveyanceServiceHub = "ConveyanceServiceHub";
            public const string ConveyanceHub = "ConveyanceHub";
        }

        public static class ServiceType
        {
            public const string ExecutionEngine = "executionEngine";
            public const string Conveyance = "conveyance";
        }
    }
}
