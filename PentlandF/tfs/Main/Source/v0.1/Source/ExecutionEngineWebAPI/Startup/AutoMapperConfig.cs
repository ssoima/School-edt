using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.ExecutionEngineWebAPI.Models;
using NextLAP.IP1.Models.Manufacturing;

namespace ExecutionEngineWebAPI.Startup
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Order, ManufacturingOrderModel>()
                    .ForMember(target => target.Status, opt => opt.MapFrom(source => source.Status.ToString()))
                    .ForMember(target => target.Customer, opt => opt.MapFrom(source => source.CustomerName));

               
            });
        }
    }
}