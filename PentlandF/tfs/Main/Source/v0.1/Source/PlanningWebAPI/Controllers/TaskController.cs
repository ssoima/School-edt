using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models;

namespace NextLAP.IP1.PlanningWebAPI.Controllers
{
    [RoutePrefix("tasks")]
    public class TaskController : DefaultGetApiController<TaskModel, TaskRepository, Task>
    {
        protected override TaskRepository GetRepository
        {
            get { return Repositories.TaskRepository; }
        }

        [Route("create"),
         HttpPost]
        public TaskModel Create([FromBody] TaskModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = GetRepository;
            var entity = repo.Create();
            entity.Name = model.Name;
            entity.Description = model.Description;
            repo.SaveChanges();

            return Mapper.Map<Task, TaskModel>(entity);
        }

        [Route("update"),
         HttpPost,
         HttpPut]
        public TaskModel Update([FromBody] TaskModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null) throw new InvalidOperationException("Task with ID:" + model.Id + " does not exist.");
            entity.Name = model.Name;
            entity.Description = model.Description;

            repo.SaveChanges();

            return Mapper.Map<Task, TaskModel>(entity);
        }

        [Route("delete/{id}"),
         HttpPost,
         HttpDelete]
        public bool Delete(long id)
        {
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == id);
            if (entity == null) throw new InvalidOperationException("Task with ID:" + id + " does not exist.");
            repo.Delete(entity);
            return repo.SaveChanges();
        }
    }
}
