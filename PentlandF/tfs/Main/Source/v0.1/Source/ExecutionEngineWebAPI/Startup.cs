using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

namespace NextLAP.IP1.ExecutionEngineWebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            var hubby = new HubConfiguration()
            {
                EnableDetailedErrors = true
            };

            app.MapSignalR(hubby);
        }
    }
}