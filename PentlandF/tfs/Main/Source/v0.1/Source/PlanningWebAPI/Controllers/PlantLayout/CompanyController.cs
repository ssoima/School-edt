using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.PlantLayout
{
    [RoutePrefix("companies")]
    public class CompanyController : DefaultGetApiController<CompanyModel, CompanyRepository, Company>
    {
        protected override CompanyRepository GetRepository
        {
            get { return Repositories.CompanyRepository; }
        }
    }
}
