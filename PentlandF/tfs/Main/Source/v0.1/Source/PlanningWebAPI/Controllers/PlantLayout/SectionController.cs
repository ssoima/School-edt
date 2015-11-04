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
    [RoutePrefix("sections")]
    public class SectionController : DefaultGetApiController<SectionModel, SectionRepository, Section>
    {
        protected override SectionRepository GetRepository
        {
            get { return Repositories.SectionRepository; }
        }

        [Route("byassembly/{assemblyLineId}"), HttpGet]
        public IEnumerable<SectionModel> AllForAssembly(long assemblyLineId)
        {
            var result =
                Repositories.SectionRepository.Entities.Where(x => x.AssemblyLineId == assemblyLineId)
                    .Project()
                    .To<SectionModel>()
                    .ToList();
            return result;
        }
    }
}