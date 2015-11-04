using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Common.Web;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.ProcessPlan
{
    [RoutePrefix("assignableparts")]
    public class TaskAssignmentController : BaseApiController
    {
        [Route("")]
        public IEnumerable<AssignablePartListModel> Get()
        {
            var result = Repositories.PartRepository.Entities.Project().To<AssignablePartListModel>().ToList();
            return result;
        } 
    }
}
