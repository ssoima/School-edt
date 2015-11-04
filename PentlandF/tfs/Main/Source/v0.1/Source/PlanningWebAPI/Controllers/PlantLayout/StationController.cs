using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Helper;
using NextLAP.IP1.PlanningWebAPI.Models;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout.Post;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.PlantLayout
{
    [RoutePrefix("stations")]
    public class StationController : DefaultGetApiController<StationModel, StationRepository, Station>
    {
        protected override StationRepository GetRepository
        {
            get { return Repositories.StationRepository; }
        }

        [Route("types"), HttpGet]
        public IEnumerable<StationTypeModel> GetStationTypes()
        {
            return Repositories.StationTypeRepository.Entities.Project().To<StationTypeModel>().ToList();
        }

        // GET /stations/bysection/1
        [Route("bysection/{sectionId}"), HttpGet]
        public IEnumerable<StationModel> AllForSection(long sectionId)
        {
            var result = GetRepository.Entities.Where(x => x.SectionId == sectionId)
                    .Project()
                    .To<StationModel>()
                    .ToList();
            return result;
        }

        // GET /stations/byassembly/1
        [Route("byassembly/{assemblyLineId}"), HttpGet]
        public IEnumerable<StationModel> AllForAssemblyLine(long assemblyLineId)
        {
            var result =GetRepository.Entities.Where(x => x.Section.AssemblyLineId == assemblyLineId)
                    .Project()
                    .To<StationModel>()
                    .ToList();
            return result;
        }

        // GET /stations/byfactory/1
        [Route("byfactory/{factoryId}"), HttpGet]
        public IEnumerable<StationModel> AllForFactory(long factoryId)
        {
            var result = GetRepository.Entities.Where(x => x.Section.AssemblyLine.FactoryId == factoryId)
                    .Project()
                    .To<StationModel>()
                    .ToList();
            return result;
        }

        // POST /stations/create
        [Route("create"),
         HttpPost]
        public StationModel Create([FromBody] StationModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(model.Type)) throw new InvalidOperationException("Station type must be provided.");

            var repo = GetRepository;
            var entity = repo.Create();
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.SectionId = model.SectionId;
            entity.PredecessorId = null;
            // try to assign station type
            if (!string.IsNullOrEmpty(model.Type))
            {
                var stationType = Repositories.StationTypeRepository.Entities.FirstOrDefault();
                if (stationType == null)
                    throw new InvalidOperationException("A Station Type '" + model.Type + "' does not exist.");
                entity.StationType = stationType;
            }
            LinkedEntityHelper.AddLinkedEntity(repo.Entities, entity, model);
            repo.SaveChanges();

            return Mapper.Map<Station, StationModel>(entity);
        }


        // POST /stations/create
        [Route("createwithworker"),
         HttpPost]
        public StationModel Create([FromBody] CreateStationWithWorkerModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(model.Type)) throw new InvalidOperationException("Station type must be provided.");
            if (model.Worker < 1) throw new InvalidOperationException("You need to specify at least one worker.");

            var repo = GetRepository;
            var entity = repo.Create();
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.SectionId = model.SectionId;
            entity.PredecessorId = null;
            // try to assign station type
            if (!string.IsNullOrEmpty(model.Type))
            {
                var stationType = Repositories.StationTypeRepository.Entities.FirstOrDefault(x => x.Name == model.Type);
                if (stationType == null)
                    throw new InvalidOperationException("A Station Type '" + model.Type + "' does not exist.");
                entity.StationType = stationType;
            }
            LinkedEntityHelper.AddLinkedEntity(repo.Entities, entity, model);
            repo.SaveChanges();

            for (int i = 0; i < model.Worker; i++)
            {
                Repositories.WorkstationRepository.CreateDefaultWorkstation(entity, i);
            }
            repo.SaveChanges();

            return Mapper.Map<Station, StationModel>(entity);
        }

        // POST /stations/update
        [Route("update"),
         HttpPost,
         HttpPut]
        public StationModel Update([FromBody] StationModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null) throw new InvalidOperationException("Station with ID:" + model.Id + " does not exist.");
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.SectionId = model.SectionId;
            // try to assign station type
            if (!string.IsNullOrEmpty(model.Type))
            {
                var stationType = Repositories.StationTypeRepository.Entities.FirstOrDefault();
                if (stationType == null)
                    throw new InvalidOperationException("A Station Type '" + model.Type + "' does not exist.");
                entity.StationType = stationType;
            }
            repo.SaveChanges();

            return Mapper.Map<Station, StationModel>(entity);
        }

        [Route("delete/{id}"),
         HttpPost,
         HttpDelete]
        public bool Delete(long id)
        {
            var repo = GetRepository;
            var entity =
                repo.Entities.Include(x => x.WorkStations).FirstOrDefault(x => x.Id == id);
            if (entity == null) throw new InvalidOperationException("Station with ID:" + id + " does not exist.");

            LinkedEntityHelper.RemoveLinkedEntity(repo.Entities, entity);
            repo.SaveChanges();

            Repositories.WorkstationRepository.Delete(entity.WorkStations);
            Repositories.StationRepository.Delete(entity);

            return Repositories.StationRepository.SaveChanges();
        }

        [Route("move"),
         HttpPost]
        public StationModel MoveStation([FromBody] MoveLinkedEntityModel model)
        {
            var repo = GetRepository;
            var entity = repo.Entities.Include(x => x.Predecessor).FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
                throw new InvalidOperationException("Station with ID: " + model.Id + " does not exist.");

            LinkedEntityHelper.MoveLinkedEntity(repo.Entities, entity, model);
            repo.SaveChanges();

            return Mapper.Map<Station, StationModel>(entity);
        }
    }
}