using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace NextLAP.IP1.WorkstationService.Configuration
{
    public static class WebApiConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            RouteConfiguration.Configure(config);
        }
    }
}