using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Models;

namespace NextLAP.IP1.ExecutionEngine.Hubs
{
    public class ConveyanceServiceHub : Hub
    {
        private static Logger Log = LogManager.GetLogger(typeof (ConveyanceServiceHub));

        public override Task OnConnected()
        {
            var type = Context.QueryString.Get("type");
            Log.Debug("Client [" + type + "] connected..ID:" + Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            Log.Debug("Client ID:" + Context.ConnectionId + " reconnected..");
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Log.Debug("Client " + Context.ConnectionId + " disconnected..");
            return base.OnDisconnected(stopCalled);
        }

        public void ConveyanceProgress(ConveyanceProgressModel model)
        {
            Engine.Instance.OnConveyanceMovement(model);
        }

        public void ConveyanceStarted(ConveyanceProgressModel model)
        {
            Engine.Instance.NotifyConveyanceStarted(model);
        }

        public void ConveyanceStopped(ConveyanceProgressModel model)
        {
            Engine.Instance.NotifyConveyanceStopped(model);
        }
    }
}
