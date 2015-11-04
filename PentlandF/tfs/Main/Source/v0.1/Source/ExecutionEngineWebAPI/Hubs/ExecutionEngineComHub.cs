using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using NextLAP.IP1.Common.Diagnostics;

namespace NextLAP.IP1.ExecutionEngineWebAPI.Hubs
{
    public class ExecutionEngineComHub : Hub
    {
        private static Logger Log = LogManager.GetLogger(typeof (ExecutionEngineComHub));

        #region Connect, Reconnect, Disconnect

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        #endregion

        // here come methods for being called from on-site execution engine

        
    }
}