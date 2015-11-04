using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Common.Web;
using NextLAP.IP1.Models.Base;
using NextLAP.IP1.Storage.EntityFramework;
using NextLAP.IP1.PlanningWebAPI.Models.Base;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;

namespace NextLAP.IP1.PlanningWebAPI.Controllers
{
    public abstract class DefaultGetApiController<TModel, TRepository, TEntity> : BaseApiController
        where TModel : BaseModel
        where TEntity : BaseEntity
        where TRepository : Ip1BaseRepository<TEntity>
    {
        protected abstract TRepository GetRepository { get; }

        // GET /deriving_class_route_prefix
        [Route("")]
        public IEnumerable<TModel> Get()
        {
            var result = GetRepository.Entities.Project().To<TModel>().ToList();
            return result;
        }

        // GET /deriving_class_route_prefix/:id
        [Route("{id}")]
        public TModel Get(long id)
        {
            var model = GetRepository.Entities.Where(x => x.Id == id)
                    .Project()
                    .To<TModel>()
                    .FirstOrDefault();
            return model;
        }
    }
}