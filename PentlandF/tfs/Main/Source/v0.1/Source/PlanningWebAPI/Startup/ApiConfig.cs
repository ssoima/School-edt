using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using NextLAP.IP1.PlanningWebAPI.Configuration;

namespace NextLAP.IP1.PlanningWebAPI.Startup
{
    public static class ApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            // here we will configure the WebAPI filter and handler pipeline
            // f.e. config.Filters.Add(new MyCustomFilterAttribute())
            // f.e. config.MessageHandlers.Add(new MyCustomDelegatingHandler())
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            
            RouteConfiguration.Configure(config);
        }
    }
}