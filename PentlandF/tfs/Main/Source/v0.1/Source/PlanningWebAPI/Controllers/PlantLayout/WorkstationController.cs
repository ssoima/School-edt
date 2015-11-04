using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.PlantLayout
{
    [RoutePrefix("workstations")]
    public class WorkstationController : DefaultGetApiController<WorkstationModel, WorkstationRepository, Workstation>
    {
        protected override WorkstationRepository GetRepository
        {
            get { return Repositories.WorkstationRepository; }
        }

        [Route("bystation/{stationId}"),
         HttpGet]
        public IEnumerable<WorkstationModel> AllForStation(long stationId)
        {
            var result =
                Repositories.WorkstationRepository.Entities.Where(x => x.StationId == stationId)
                    .Project()
                    .To<WorkstationModel>()
                    .ToList();
            return result;
        }

        [Route("bysection/{sectionId}"),
         HttpGet]
        public IEnumerable<WorkstationModel> AllForSection(long sectionId)
        {
            var result =
                Repositories.WorkstationRepository.Entities.Where(x => x.Station.SectionId == sectionId)
                    .Project()
                    .To<WorkstationModel>()
                    .ToList();
            return result;
        }

        [Route("byfactory/{factoryId}"),
         HttpGet]
        public IEnumerable<WorkstationModel> AllForFactory(long factoryId)
        {
            var result =
                Repositories.WorkstationRepository.Entities.Where(x => x.Station.Section.AssemblyLine.FactoryId == factoryId)
                    .Project()
                    .To<WorkstationModel>()
                    .ToList();
            return result;
        }

        [Route("create"),
         HttpPost]
        public WorkstationModel Create([FromBody] WorkstationModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = Repositories.WorkstationRepository;
            var station =
                Repositories.StationRepository.Entities.Include(x => x.StationType)
                    .Include(x => x.WorkStations)
                    .FirstOrDefault(x => x.Id == model.StationId);
            if (station == null) throw new InvalidOperationException("There is no station with ID: " + model.StationId);

            var entity = repo.Create();
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.StationId = model.StationId;
            entity.Side = model.Side;
            entity.Type = model.Type;
            entity.Position = model.Position;

            repo.SaveChanges();

            return Mapper.Map<Workstation, WorkstationModel>(entity);
        }

        [Route("createdefault"),
         HttpPost]
        public WorkstationModel CreateDefault([FromBody] WorkstationModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = Repositories.WorkstationRepository;
            var station =
                Repositories.StationRepository.Entities.Include(x => x.StationType)
                    .Include(x => x.WorkStations)
                    .FirstOrDefault(x => x.Id == model.StationId);
            if (station == null) throw new InvalidOperationException("There is no station with ID: " + model.StationId);
            var currentWorkerCount = station.WorkStations.Count();
            var entity = repo.CreateDefaultWorkstation(station, currentWorkerCount);

            repo.SaveChanges();

            return Mapper.Map<Workstation, WorkstationModel>(entity);
        }


        [Route("createmultiple"),
         HttpPost]
        public IEnumerable<WorkstationModel> Create([FromBody] IEnumerable<WorkstationModel> models)
        {
            if (models == null) throw new ArgumentNullException("models");
            var repo = Repositories.WorkstationRepository;
            var result = new List<Workstation>();
            foreach (var model in models)
            {
                var entity = repo.Create();
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.StationId = model.StationId;
                entity.Side = model.Side;
                entity.Type = model.Type;
                entity.Position = model.Position;
                result.Add(entity);
            }

            repo.SaveChanges();

            return result.Select(Mapper.Map<Workstation, WorkstationModel>);
        }

        [Route("update"),
         HttpPost,
         HttpPut]
        public WorkstationModel Update([FromBody] WorkstationModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var repo = Repositories.WorkstationRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null) throw new InvalidOperationException("Workstation with ID:" + model.Id + " does not exist.");
            entity.Name = model.Name;
            entity.Description = model.Description;
            entity.StationId = model.StationId;
            entity.Side = model.Side;
            entity.Type = model.Type;
            entity.Position = model.Position;

            repo.SaveChanges();

            return Mapper.Map<Workstation, WorkstationModel>(entity);
        }

        [Route("delete/{id}"),
         HttpPost,
         HttpDelete]
        public bool Delete(long id)
        {
            var entity =
                Repositories.WorkstationRepository.Entities.FirstOrDefault(x => x.Id == id);
            if (entity == null) throw new InvalidOperationException("Workstation with ID:" + id + " does not exist.");
            Repositories.WorkstationRepository.Delete(entity);
            return Repositories.WorkstationRepository.SaveChanges();
        }
    }
}