using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoMapper.QueryableExtensions.Impl;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.ProcessPlan
{
    [RoutePrefix("equipmentdrivers")]
    public class EquipmentDriverController : DefaultGetApiController<EquipmentDriverModel, EquipmentDriverRepository, EquipmentDriver>
    {
        protected override EquipmentDriverRepository GetRepository
        {
            get { return Repositories.EquipmentDriverRepository; }
        }

        [Route("bytype/{typeId}")]
        public IEnumerable<EquipmentDriverModel> GetDriversForEquipmentType(long typeId)
        {
            return
                GetRepository.Entities.Where(x => x.EquipmentTypeId == typeId)
                    .Project()
                    .To<EquipmentDriverModel>()
                    .ToList();
        }

        #region Create for Equipment Type

        [Route("create"),
         HttpPost]
        public EquipmentDriverModel CreateDriverForType([FromBody] CreateEquipmentDriverModel model)
        {
            var equipmentType =
                Repositories.EquipmentTypeRepository.Entities.FirstOrDefault(x => x.Id == model.EquipmentTypeId);
            if (equipmentType == null)
                throw new InvalidOperationException("There is no equipment type with ID:" + model.EquipmentTypeId);
            if (string.IsNullOrEmpty(model.ClrType)) throw new InvalidOperationException("You must set a Clr Type");

            var repo = GetRepository;
            var entity = repo.Create();
            entity.EquipmentType = equipmentType;
            entity.ClrType = model.ClrType;
            entity.Name = model.Name;
            entity.Description = model.Description;

            repo.SaveChanges();
            return Mapper.Map<EquipmentDriver, EquipmentDriverModel>(entity);
        }

        #endregion
    }
}