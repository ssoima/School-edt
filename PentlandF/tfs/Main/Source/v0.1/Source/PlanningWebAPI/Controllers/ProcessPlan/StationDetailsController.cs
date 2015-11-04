using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Common.Web;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.ProcessPlan
{
    [RoutePrefix("stationdetails")]
    public class StationDetailsController : BaseApiController
    {
        [Route("{stationId}"), HttpGet]
        public StationDetailsOverviewModel GetOverview(long stationId)
        {
            var result =
                Repositories.StationRepository.Entities.Where(x => x.Id == stationId)
                    .Project()
                    .To<StationDetailsOverviewModel>()
                    .FirstOrDefault();
            return result;
        } 
    }
}