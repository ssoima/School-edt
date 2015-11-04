using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.ProcessPlan
{
    [RoutePrefix("equipments")]
    public class EquipmentController : DefaultGetApiController<EquipmentConfigurationModel, EquipmentConfigurationRepository, EquipmentConfiguration>
    {
        protected override EquipmentConfigurationRepository GetRepository
        {
            get { return Repositories.EquipmentConfigurationRepository; }
        }
        
        [Route("types")]
        public IEnumerable<EquipmentTypeModel> GetTypes()
        {
            return Repositories.EquipmentTypeRepository.Entities.Project().To<EquipmentTypeModel>().ToList();
        }

        [Route("bystation/{id}")]
        public IEnumerable<EquipmentConfigurationModel> GetEquipmentByStation(long id)
        {
            return
                GetRepository.Entities.Where(x => x.WorkStation.StationId == id)
                    .Project()
                    .To<EquipmentConfigurationModel>()
                    .ToList();
        }

        [Route("byworkstation/{id}")]
        public IEnumerable<EquipmentConfigurationModel> GetEquipmentByWorkstation(long id)
        {
            return
                GetRepository.Entities.Where(x => x.WorkStationId == id)
                    .Project()
                    .To<EquipmentConfigurationModel>()
                    .ToList();
        }

        [Route("create"),
         HttpPost]
        public EquipmentConfigurationModel Create([FromBody] CreateOrUpdateEquipmentConfigurationModel model)
        {
            var equipmentType =
                Repositories.EquipmentTypeRepository.Entities.FirstOrDefault(x => x.Id == model.EquipmentTypeId);
            if (equipmentType == null)
                throw new InvalidOperationException("There is no equipment type with ID:" + model.EquipmentTypeId);
            var workstation =
                Repositories.WorkstationRepository.Entities.FirstOrDefault(x => x.Id == model.WorkstationId);
            if (workstation == null)
                throw new InvalidOperationException("There is no workstation with ID:" + model.WorkstationId);
            EquipmentDriver driver = null;
            if (model.DriverId != null)
            {
                driver = Repositories.EquipmentDriverRepository.Entities.FirstOrDefault(x => x.Id == model.DriverId);
                if (driver == null) throw new InvalidOperationException("There is no driver with ID:" + model.DriverId);
            }
            if (driver != null && driver.EquipmentTypeId != model.EquipmentTypeId)
                throw new InvalidOperationException("The given driver '" + driver.Name +
                                                    "' is not intended for the given Equipment type [ID:" +
                                                    model.EquipmentTypeId + "].");
            var repo = GetRepository;
            var entity = repo.Create();
            entity.Name = model.Name;
            entity.Configuration = model.Configuration;
            entity.IpAddress = model.IpAddress;
            entity.UsedDriver = driver;
            entity.Description = model.Description;
            entity.EquipmentType = equipmentType;
            entity.WorkStation = workstation;
            repo.SaveChanges();
            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(entity);
        }

        [Route("update"),
         HttpPost]
        public EquipmentConfigurationModel Update([FromBody] CreateOrUpdateEquipmentConfigurationModel model)
        {
            
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
                throw new InvalidOperationException("There is no equipment configuration with ID:" + model.Id);
            EquipmentDriver driver = null;
            if (model.DriverId != null)
            {
                driver = Repositories.EquipmentDriverRepository.Entities.FirstOrDefault(x => x.Id == model.DriverId);
                if (driver == null) throw new InvalidOperationException("There is no driver with ID:" + model.DriverId);
            }
            if (driver != null && driver.EquipmentTypeId != entity.EquipmentTypeId)
                throw new InvalidOperationException("The given driver '" + driver.Name +
                                                    "' is not intended for the given Equipment type [ID:" +
                                                    entity.EquipmentTypeId + "].");
            entity.Name = model.Name;
            entity.Configuration = model.Configuration;
            entity.IpAddress = model.IpAddress;
            entity.UsedDriver = driver;
            entity.Description = model.Description;
            repo.SaveChanges();
            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(entity);
        }

        [Route("delete/{id}"),
         HttpPost,
         HttpDelete]
        public bool Delete(long id)
        {
            var repo = GetRepository;
            var equipment = repo.Entities.Include(x => x.StationTaskAssignments).FirstOrDefault(x => x.Id == id);
            if (equipment == null) throw new InvalidOperationException("There is no equipment with ID:" + id);
            if (equipment.StationTaskAssignments.Count > 0)
                throw new InvalidOperationException(
                    "There are still tasks assigned to the equipment. Please unassign first all tasks before removing this equipment.");
            repo.Delete(equipment);
            return repo.SaveChanges();
        }
    }
}