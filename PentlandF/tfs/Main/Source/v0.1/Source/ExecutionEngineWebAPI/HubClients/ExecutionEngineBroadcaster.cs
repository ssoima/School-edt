using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.ExecutionEngineWebAPI.Hubs;
using NextLAP.IP1.Models.Manufacturing;
using NextLAP.IP1.Storage.EntityFramework.Repositories;

namespace NextLAP.IP1.ExecutionEngineWebAPI.HubClients
{
    public class ExecutionEngineBroadcaster
    {
        private static readonly Lazy<IHubContext> _hub =
            new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<ExecutionEngineComHub>());

        public static bool EnqueueOrderForProduction(Order order)
        {
            var partList = order.Parts.Select(x => x.Part.Number).ToList();

            

            var model = new ExecutionEngineOrderDescriptionModel()
            {
                OrderNumber = order.OrderNumber,
                Customer = order.CustomerName,
                Model = order.Model,
                Variant = order.Variant,
                PartNumbers = partList
            };

            _hub.Value.Clients.All.EnqueueOrder(model);
            return true;
        }
    }
}