using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.WorkstationService.Hubs;

namespace NextLAP.IP1.WorkstationService.Hubs
{
    [HubName("terminal")]
    public class WorkstationTerminalHub : Hub
    {
        private readonly Lazy<IHubContext> _executionEngine = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<ExecutionEngineHub>());
        private readonly Ip1Repositories _repositories;
        public WorkstationTerminalHub(Ip1Repositories repositories)
        {
            _repositories = repositories;
        }

        public override Task OnConnected()
        {
            var workstationId = Context.QueryString.Get("workstationId");
            long parsedWorkstationId;
            if (!Int64.TryParse(workstationId, out parsedWorkstationId))
            {
                throw new InvalidOperationException("Invalid workstation id set. [" + workstationId + "].");
            }
            var stationId =
                _repositories.WorkstationRepository.Entities.Where(x => x.Id == parsedWorkstationId)
                    .Select(x => x.StationId)
                    .FirstOrDefault();
            if (!string.IsNullOrEmpty(workstationId)) Groups.Add(Context.ConnectionId, "ws" + workstationId);
            if (stationId != null) Groups.Add(Context.ConnectionId, "s" + stationId);

            return base.OnConnected();
        }

        public void GetCurrentOrderInfo()
        {
            var workstationId = Context.QueryString.Get("workstationId");
            long parsedWorkstationId;
            if (!Int64.TryParse(workstationId, out parsedWorkstationId))
            {
                throw new InvalidOperationException("Invalid workstation id set. [" + workstationId + "].");
            }
            _executionEngine.Value.Clients.All.GetCurrentOrder(parsedWorkstationId);
        }

        public void GetWorkstationInfo()
        {
            var workstationId = Context.QueryString.Get("workstationId");
            long parsedWorkstationId;
            if (!Int64.TryParse(workstationId, out parsedWorkstationId))
            {
                throw new InvalidOperationException("Invalid workstation id set. [" + workstationId + "].");
            }
            _executionEngine.Value.Clients.All.GetWorkstationInfo(parsedWorkstationId);
        }

        public void StartConveyanceBelt()
        {
            var workstationId = Context.QueryString.Get("workstationId");
            long parsedWorkstationId;
            if (!Int64.TryParse(workstationId, out parsedWorkstationId))
            {
                throw new InvalidOperationException("Invalid workstation id set. [" + workstationId + "].");
            }
            _executionEngine.Value.Clients.All.StartConveyanceBelt(parsedWorkstationId);
        }

        public void StopConveyanceBelt()
        {
            var workstationId = Context.QueryString.Get("workstationId");
            long parsedWorkstationId;
            if (!Int64.TryParse(workstationId, out parsedWorkstationId))
            {
                throw new InvalidOperationException("Invalid workstation id set. [" + workstationId + "].");
            }
            _executionEngine.Value.Clients.All.StopConveyanceBelt(parsedWorkstationId);
        }

        public void ChangeTaskComment(string orderId, long stationTaskId, string comment)
        {
            var workstationId = Context.QueryString.Get("workstationId");
            long parsedWorkstationId;
            if (!Int64.TryParse(workstationId, out parsedWorkstationId))
            {
                throw new InvalidOperationException("Invalid workstation id set. [" + workstationId + "].");
            }
            _executionEngine.Value.Clients.All.ChangeTaskComment(orderId, workstationId, stationTaskId, comment);
        }
    }
}