using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Common.Processing;
using NextLAP.IP1.Common.Web;
using NextLAP.IP1.ExecutionEngineWebAPI.HubClients;
using NextLAP.IP1.ExecutionEngineWebAPI.Models;
using NextLAP.IP1.Models.Manufacturing;

namespace NextLAP.IP1.ExecutionEngineWebAPI.Controllers
{
    [RoutePrefix("orders")]
    public class ManufacturingOrderController : BaseApiController
    {
        [Route("")]
        public IEnumerable<ManufacturingOrderModel> Get()
        {
            return Repositories.OrderRepository.Entities.Project().To<ManufacturingOrderModel>().ToList();
        }

        [Route("createfake"),
         HttpPost]
        public ManufacturingOrderModel CreateFakeOrder([FromBody] CreateFakeOrder model)
        {
            var repo = Repositories.OrderRepository;
            var partList = new StaticOrderPartlistResolver().ResolvePartlist(Enumerable.Empty<string>());

            var entity = repo.Create();
            entity.OrderNumber = CreateOrderNumber();
            entity.CustomerName = model.CustomerName;
            entity.OrderDate = DateTime.Now;
            entity.PlannedDeliveryDate = DateTime.Today.AddMonths(1);
            entity.Model = "A4";
            entity.Variant = model.Variant;
            entity.Status = OrderStatus.New;

            var partEntities = Repositories.PartRepository.Entities.Where(x => partList.Contains(x.Number)).ToList();
            foreach (var part in partEntities)
            {
                var newMap = Repositories.OrderPartItemRepository.Create();
                newMap.Order = entity;
                newMap.Part = part;
            }
            repo.SaveChanges();
            return Mapper.Map<Order, ManufacturingOrderModel>(entity);
        }

        [Route("enqueue"),
         HttpPost]
        public ManufacturingOrderModel EnqueueNewOrder([FromBody] EnqueueNewOrderModel model)
        {
            var repo = Repositories.OrderRepository;
            var order =
                repo.Entities.Include(x => x.Parts.Select(y => y.Part))
                    .FirstOrDefault(x => x.OrderNumber == model.OrderNumber);
            if (order == null)
                throw new InvalidOperationException("Order with number: " + model.OrderNumber + " does not exist.");
            if (order.Status > OrderStatus.New) throw new InvalidOperationException("Order status is not proper.");
            order.Status = OrderStatus.Queued;
            order.StatusChanged = DateTime.Now;

            if (repo.SaveChanges())
            {
                // we will inform the execution engine at the production to queue the order
                if(!ExecutionEngineBroadcaster.EnqueueOrderForProduction(order))
                    throw new InvalidOperationException("Enqueuing failed");
            }
            return Mapper.Map<Order, ManufacturingOrderModel>(order);
        }

        private string CreateOrderNumber()
        {
            return "MO-" + Guid.NewGuid().ToString("n").Substring(0, 8).ToUpper();
        }
    }
}