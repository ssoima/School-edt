using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.PlantLayout
{
    [RoutePrefix("assemblies")]
    public class AssemblyLineController : DefaultGetApiController<AssemblyLineModel, AssemblyLineRepository, AssemblyLine>
    {
        protected override AssemblyLineRepository GetRepository
        {
            get { return Repositories.AssemblyLineRepository; }
        }

        [Route("byfactory/{factoryId}"), HttpGet]
        public IEnumerable<AssemblyLineModel> AllForFactory(long factoryId)
        {
            var result =
                Repositories.AssemblyLineRepository.Entities.Where(x => x.FactoryId == factoryId)
                    .Project()
                    .To<AssemblyLineModel>()
                    .ToList();
            return result;
        }
    }
}