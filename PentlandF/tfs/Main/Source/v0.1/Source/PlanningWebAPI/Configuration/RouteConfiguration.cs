using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NextLAP.IP1.Common.Web.Filters;

namespace NextLAP.IP1.PlanningWebAPI.Configuration
{
    public class RouteConfiguration
    {
        public static void Configure(HttpConfiguration config)
        {
            config.Filters.Add(new IpOneExceptionFilterAttribute());

            config.MapHttpAttributeRoutes(new InheritanceRouteProvider());
        }
    }
}