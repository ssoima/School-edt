using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NextLAP.IP1.Common.Web;

namespace NextLAP.IP1.WorkstationService
{
    public abstract class BaseApiControllerWithHub<THub> : BaseApiController where THub : IHub
    {
        private readonly Lazy<IHubContext> _hub = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<THub>());

        protected IHubContext Hub { get { return _hub.Value; } }
    }
}