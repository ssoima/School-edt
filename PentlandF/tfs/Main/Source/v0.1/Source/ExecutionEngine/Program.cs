using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NextLAP.IP1.Common.Configuration;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.HubClients;

namespace NextLAP.IP1.ExecutionEngine
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetLogger(typeof(Program));
        private const string DefaultEndpointUrl = "http://localhost:8080";
        static void Main(string[] args)
        {
            Log.Debug("Execution Engine Startup..");

            try
            {
                var factoryIdSetting = ConfigurationManager.AppSettings["factoryId"];
                if (string.IsNullOrEmpty(factoryIdSetting))
                    throw new InvalidOperationException("Please specify a 'factoryId' in AppSettings.");
                var factoryId = 0L;
                if (!Int64.TryParse(factoryIdSetting, out factoryId))
                    throw new InvalidOperationException("The factoryId setting in AppSettings is not a valid Int64.");

                var startingUrl = ConfigurationManager.AppSettings[Constants.AppSettings.RunningEndpointUrl] ??
                                  DefaultEndpointUrl;
                using (var engine = Engine.Instance)
                {
                    engine.BootstrapEngine();
                    using (WebApp.Start(startingUrl))
                    using (new ExecutionEngineWebHubClient(engine))
                    using (new WorkstationTerminalHubClient(engine))
                    {
                        Log.Debug(string.Format("Execution Engine Server running on {0}", startingUrl));
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.Message);
                Log.Debug(ex.StackTrace);
                Console.ReadLine();
            }
            //Console.ReadLine();
        }
    }
}
