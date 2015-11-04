using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models;

namespace NextLAP.IP1.PlanningWebAPI.Controllers
{
    [RoutePrefix("parts")]
    public class PartController :  DefaultGetApiController<PartModel, PartRepository, Part>
    {
        protected override PartRepository GetRepository
        {
            get { return Repositories.PartRepository; }
        }

        [Route("create"),
         HttpPost]
        public PartModel Create([FromBody] PartModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = GetRepository;
            var entity = repo.Create();
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Number = model.Number;
            entity.LongName = model.LongName;
            entity.PartType = model.PartType;
            entity.PartGroup = model.PartGroup;
            repo.SaveChanges();

            return Mapper.Map<Part, PartModel>(entity);
        }

        [Route("update"),
         HttpPost,
         HttpPut]
        public PartModel Update([FromBody] PartModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null) throw new InvalidOperationException("Part with ID:" + model.Id + " does not exist.");
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.Number = model.Number;
            entity.LongName = model.LongName;
            entity.PartType = model.PartType;
            entity.PartGroup = model.PartGroup;

            repo.SaveChanges();

            return Mapper.Map<Part, PartModel>(entity);
        }

        [Route("delete/{id}"),
         HttpPost,
         HttpDelete]
        public bool Delete(long id)
        {
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == id);
            if (entity == null) throw new InvalidOperationException("Part with ID:" + id + " does not exist.");
            repo.Delete(entity);
            return repo.SaveChanges();
        }
    }
}