using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;
using PP = NextLAP.IP1.Models.Planning.ProcessPlan;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.ProcessPlan
{
    [RoutePrefix("processplan")]
    public class ProcessPlanController : DefaultGetApiController<ProcessPlanModel, ProcessPlanRepository, PP>
    {
        protected override ProcessPlanRepository GetRepository
        {
            get { return Repositories.ProcessPlanRepository; }
        }

        [Route("version/{factoryId}"), HttpGet]
        public int CurrentVersion(long factoryId)
        {
            return
                GetRepository.Entities.Where(x => x.FactoryId == factoryId)
                    .Select(x => x.CurrentVersion)
                    .FirstOrDefault();
        }

        [Route("stationconfiguration/{factoryId}"), HttpGet]
        public ProcessPlanDescriptionModel GetCurrentConfiguration(long factoryId)
        {
            var result =
                GetRepository.Entities.Where(x => x.FactoryId == factoryId)
                    .Project()
                    .To<ProcessPlanDescriptionModel>()
                    .FirstOrDefault();
            return result;
        } 
    }
}