using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Common.Web;
using System.Data.Entity.Migrations;

namespace NextLAP.IP1.PlanningWebAPI.Controllers
{
    [RoutePrefix("administration")]
    public class AdministrationController : BaseApiController // Not using DefaultGetApiController because not having models and repository for the admin management for now.
    {
        [Route("resetdb"), HttpGet]
        public string ResetDb()
        {
            Storage.EntityFramework.Ip1Context.ResetDB();

            return "IP1 Planning database was reset successfully!";
        }

      
    }
}
