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
    [RoutePrefix("factories")]
    public class FactoryController : DefaultGetApiController<FactoryModel, FactoryRepository, Factory>
    {
        protected override FactoryRepository GetRepository
        {
            get { return Repositories.FactoryRepository; }
        }

        [Route("bycompany/{companyId}"), HttpGet]
        public IEnumerable<FactoryModel> AllForCompany(long companyId)
        {
            var result =
                Repositories.FactoryRepository.Entities.Where(x => x.CompanyId == companyId)
                    .Project()
                    .To<FactoryModel>()
                    .ToList();
            return result;
        }
    }
}