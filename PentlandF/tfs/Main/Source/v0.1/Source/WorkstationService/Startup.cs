using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using NextLAP.IP1.Storage.EntityFramework;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.WorkstationService.Hubs;
using Owin;

namespace NextLAP.IP1.WorkstationService
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
            // register DI for WorkstationTerminalHub
            GlobalHost.DependencyResolver.Register(typeof (WorkstationTerminalHub),
                () => new WorkstationTerminalHub(new Ip1Repositories(new Ip1Context())));

            app.MapSignalR(hubby);
        }
    }
}