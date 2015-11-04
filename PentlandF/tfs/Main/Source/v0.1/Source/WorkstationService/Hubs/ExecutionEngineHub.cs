using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Models;

namespace NextLAP.IP1.WorkstationService.Hubs
{
    [HubName("TerminalExecutionConnectHub")]
    public class ExecutionEngineHub : Hub
    {
        private readonly Lazy<IHubContext> _workstationHub = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<WorkstationTerminalHub>());

        public void NotifyOrdersInStation(IEnumerable<TerminalOrderInfo> model)
        {
            foreach (var orderInfo in model)
            {
                _workstationHub.Value.Clients.Group("ws" + orderInfo.WorkstationId).notifyOrderInStation(orderInfo);
            }
        }

        public void NotifyTaskProgressChanged(TerminalTaskSequenceInfo model)
        {
            _workstationHub.Value.Clients.Group("ws" + model.WorkstationId).notifyTaskProgress(model);
        }

        public void NotifyConveyanceProgress(ConveyanceProgressModel model)
        {
            _workstationHub.Value.Clients.All.notifyConveyanceProgress(model);
        }

        public void NotifyConveyanceStarted(ConveyanceProgressModel model)
        {
            _workstationHub.Value.Clients.All.notifyConveyanceStarted(model);
        }

        public void NotifyConveyanceStopped(ConveyanceProgressModel model)
        {
            _workstationHub.Value.Clients.All.notifyConveyanceStopped(model);
        }

        public void GetCurrentOrderInfoSucceeded(TerminalOrderInfo model)
        {
            _workstationHub.Value.Clients.Group("ws" + model.WorkstationId).notifyOrderInStation(model);
        }

        public void GetWorkstationInfoSucceeded(WorkstationInfo model)
        {
            _workstationHub.Value.Clients.Group("ws" + model.WorkstationId).notifyWorkstationInfo(model);
        }
    }
}