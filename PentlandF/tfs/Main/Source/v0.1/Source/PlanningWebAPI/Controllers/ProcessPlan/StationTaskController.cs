using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using NextLAP.IP1.Common;
using NextLAP.IP1.Common.Web;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Storage.EntityFramework.Repositories;
using NextLAP.IP1.PlanningWebAPI.Helper;
using NextLAP.IP1.PlanningWebAPI.Models;
using NextLAP.IP1.PlanningWebAPI.Models.Base;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan.Post;

namespace NextLAP.IP1.PlanningWebAPI.Controllers.ProcessPlan
{
    [RoutePrefix("stationtasks")]
    public class StationTaskController : DefaultGetApiController<StationTaskModel, StationTaskAssignmentRepository, StationTaskAssignment>
    {
        private static object s_Lock = new object();
        protected override StationTaskAssignmentRepository GetRepository
        {
            get
            {
                return Repositories.StationTaskAssignmentRepository;
            }
        }

        #region Specialized functions for (un)assigning Tasks to Stations/Workstations

        /// <summary>
        /// Assigns a TaskAssignment to a Station in General identified by the AssignTaskToModel.TargetId property.
        /// The AssignTaskToModel.Id identifies the Task Assignment by Id.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("assigntasktostation"),
         HttpPost]
        public StationTaskModel AssignTaskToStation([FromBody] AssignTargetToModel model)
        {
            // first get the TaskAssignment
            var taskAssignment =
                Repositories.TaskAssignmentRepository.Entities
                    .Include(x => x.Task)
                    .Include(x => x.Part)
                    .FirstOrDefault(x => x.Id == model.Id);
            if (taskAssignment == null)
                throw new InvalidOperationException("Task to Part Assignment with ID: " + model.Id + " does not exist.");
            var station = Repositories.StationRepository.Entities.Include(x => x.WorkStations).Where(x => x.Id == model.TargetId).Select(x => new
            {
                x.Id,
                x.Section.AssemblyLine.FactoryId,
                x.WorkStations
            }).FirstOrDefault();
            if (station == null)
                throw new InvalidOperationException("Station with ID: " + model.TargetId + " does not exist.");
            var processPlanId =
                Repositories.ProcessPlanRepository.Entities.Where(x => x.FactoryId == station.FactoryId)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            // if the station has just one workstation, we are going to assign it right away to that workstation
            var assignableWorkstationId = station.WorkStations.Count == 1
                ? station.WorkStations.Select(x => (long?)x.Id).First()
                : null;
            var repo = GetRepository;
            var stationTaskAssignment = repo.Entities
                .Include(x => x.Task)
                .Include(x => x.Part)
                .Include(x => x.EquipmentConfiguration)
                .FirstOrDefault(x => x.BasedOnTaskAssignmentId == taskAssignment.Id);
            if (stationTaskAssignment == null)
            {
                // first time assignment
                stationTaskAssignment = CreateFromTaskAssignment(taskAssignment, processPlanId);
                repo.Create(stationTaskAssignment);
            }

            var previousWorkstation = stationTaskAssignment.WorkstationId;
            // update existing entity
            stationTaskAssignment.WorkstationId = assignableWorkstationId;
            stationTaskAssignment.StationId = station.Id;
            MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, assignableWorkstationId);
            if (previousWorkstation != assignableWorkstationId)
            {
                MaintainEquipmentConfiguraton(stationTaskAssignment, assignableWorkstationId);
            }

            repo.SaveChanges();

            return Mapper.Map<StationTaskAssignment, StationTaskModel>(stationTaskAssignment);
        }


        /// <summary>
        /// Assigns a TaskAssignment to a Workstation in General identified by the AssignTaskToModel.TargetId property.
        /// The AssignTaskToModel.Id identifies the Task Assignment by Id.
        /// </summary>
        /// <remarks>
        /// When a TaskAssignment is being assigned to a workstation the StationTaskAssignment.Predecessor get set to the 
        /// last StationTaskAssignment.
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("assigntasktoworkstation"),
         HttpPost]
        public StationTaskModel AssignTaskToWorkstation([FromBody] AssignTargetToModel model)
        {
            //var taskAssignment =
            //    Repositories.TaskAssignmentRepository.Entities
            //        .Include(x => x.Task)
            //        .Include(x => x.Part)
            //        .FirstOrDefault(x => x.Id == model.Id);
            //if (taskAssignment == null)
            //    throw new InvalidOperationException("Task to Part Assignment with ID: " + model.Id + " does not exist.");

            lock (s_Lock)
            {
                var workstation =
                    Repositories.WorkstationRepository.Entities
                        .Where(x => x.Id == model.TargetId)
                        .Select(x => new
                        {
                            x.Id,
                            x.StationId,
                            x.Name,
                            x.Station.Section.AssemblyLine.FactoryId
                        })
                        .FirstOrDefault();
                if (workstation == null)
                    throw new InvalidOperationException("Workstation with ID: " + model.TargetId + " does not exist.");
                var repo = GetRepository;
                var stationTaskAssignment = repo.Entities
                    .Include(x => x.Task)
                    .Include(x => x.Part)
                    .Include(x => x.EquipmentConfiguration)
                    .FirstOrDefault(x => x.Id == model.Id);
                if (stationTaskAssignment == null)
                {
                    throw new InvalidOperationException("There is no station task assignment with ID:" + model.Id);
                    // first time assignment
                    //stationTaskAssignment = CreateFromTaskAssignment(taskAssignment);
                    //repo.Create(stationTaskAssignment);
                }
                else if (stationTaskAssignment.WorkstationId == workstation.Id)
                {
                    throw new InvalidOperationException("This task is already assigned to the Workstation: " +
                                                        workstation.Name);
                }
                var previousWorkstation = stationTaskAssignment.WorkstationId;
                stationTaskAssignment.StationId = workstation.StationId;
                stationTaskAssignment.WorkstationId = workstation.Id;
                MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, workstation.Id);

                if (previousWorkstation != workstation.Id)
                {
                    MaintainEquipmentConfiguraton(stationTaskAssignment, workstation.Id);
                }

                repo.SaveChanges();
                return Mapper.Map<StationTaskAssignment, StationTaskModel>(stationTaskAssignment);
            }
        }

        /// <summary>
        /// Assigns a TaskAssignment to a Workstation in General identified by the AssignTaskToModel.TargetId property.
        /// The AssignTaskToModel.Id identifies the Task Assignment by Id.
        /// </summary>
        /// <remarks>
        /// When a TaskAssignment is being assigned to a workstation the StationTaskAssignment.Predecessor get set to the 
        /// last StationTaskAssignment.
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("directassigntasktoworkstation"),
         HttpPost]
        public StationTaskModel DirectAssignTaskToWorkstation([FromBody] AssignTargetToModel model)
        {
            var taskAssignment =
                Repositories.TaskAssignmentRepository.Entities
                    .Include(x => x.Task)
                    .Include(x => x.Part)
                    .FirstOrDefault(x => x.Id == model.Id);
            if (taskAssignment == null)
                throw new InvalidOperationException("Task to Part Assignment with ID: " + model.Id + " does not exist.");

            lock (s_Lock)
            {
                var workstation =
                    Repositories.WorkstationRepository.Entities
                        .Where(x => x.Id == model.TargetId)
                        .Select(x => new
                        {
                            x.Id,
                            x.StationId,
                            x.Name,
                            x.Station.Section.AssemblyLine.FactoryId
                        })
                        .FirstOrDefault();
                if (workstation == null)
                    throw new InvalidOperationException("Workstation with ID: " + model.TargetId + " does not exist.");
                var repo = GetRepository;
                var stationTaskAssignment = repo.Entities
                    .FirstOrDefault(x => x.BasedOnTaskAssignmentId == model.Id && x.AssignedProcessPlan.FactoryId == workstation.FactoryId);
                if (stationTaskAssignment != null)
                {
                    throw new InvalidOperationException("There is already a station task assignment with ID:" + stationTaskAssignment.Id + ". Please call /assigntasktoworkstation instead.");
                }
                var processPlanId =
                    Repositories.ProcessPlanRepository.Entities.Where(x => x.FactoryId == workstation.FactoryId)
                        .Select(x => x.Id)
                        .FirstOrDefault();
                stationTaskAssignment = CreateFromTaskAssignment(taskAssignment, processPlanId);
                repo.Create(stationTaskAssignment);
                stationTaskAssignment.StationId = workstation.StationId;
                stationTaskAssignment.WorkstationId = workstation.Id;
                MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, workstation.Id);

                repo.SaveChanges();
                return Mapper.Map<StationTaskAssignment, StationTaskModel>(stationTaskAssignment);
            }
        }

        /// <summary>
        /// Assigns a part (which is all currently assigned tasks of a part) to a station.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("assignparttostation"),
         HttpPost]
        public IEnumerable<StationTaskModel> AssignPartToStation([FromBody] AssignTargetToModel model)
        {
            var partWithTasks =
                Repositories.PartRepository.Entities.Include(x => x.AssignedTasks).FirstOrDefault(x => x.Id == model.Id);
            if (partWithTasks == null)
                throw new InvalidOperationException("Part with ID: " + model.Id + " does not exist.");
            var taskAssignments = partWithTasks.AssignedTasks.ToList();
            if (taskAssignments.Count == 0)
                throw new InvalidOperationException("Part '" + partWithTasks.Number + "' has no tasks to assign.");
            var station = Repositories.StationRepository.Entities.Include(x => x.WorkStations).Where(x => x.Id == model.TargetId).Select(x => new
            {
                x.Id,
                x.Section.AssemblyLine.FactoryId,
                x.WorkStations
            }).FirstOrDefault();
            if (station == null)
                throw new InvalidOperationException("Station with ID: " + model.TargetId + " does not exist.");
            // if the station has just one workstation, we are going to assign it right away to that workstation
            var assignableWorkstationId = station.WorkStations.Count == 1
                ? station.WorkStations.Select(x => (long?)x.Id).First()
                : null;

            var processPlanId =
                Repositories.ProcessPlanRepository.Entities.Where(x => x.FactoryId == station.FactoryId)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            var repo = GetRepository;
            var result = new List<StationTaskModel>();
            for (var i = 0; i < taskAssignments.Count; i++)
            {
                var taskAssignment = taskAssignments[i];
                var stationTaskAssignment = repo.Entities
                    .Include(x => x.Task)
                    .Include(x => x.Part)
                    .Include(x => x.EquipmentConfiguration)
                    .FirstOrDefault(x => x.BasedOnTaskAssignmentId == taskAssignment.Id);
                if (stationTaskAssignment == null)
                {
                    // first time assignment
                    stationTaskAssignment = CreateFromTaskAssignment(taskAssignment, processPlanId);
                    repo.Create(stationTaskAssignment);
                }
                var previousWorkstation = stationTaskAssignment.WorkstationId;
                // update existing entity
                stationTaskAssignment.WorkstationId = assignableWorkstationId;
                stationTaskAssignment.StationId = station.Id;
                // trigger maintain method (this is basically a reset of the sequence)
                MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, assignableWorkstationId);
                if (previousWorkstation != assignableWorkstationId)
                {
                    MaintainEquipmentConfiguraton(stationTaskAssignment, assignableWorkstationId);
                }

                repo.SaveChanges();
                result.Add(Mapper.Map<StationTaskAssignment, StationTaskModel>(stationTaskAssignment));
            }
            return result;
        }


        /// <summary>
        /// Assigns a part (which is all currently assigned tasks of a part) to a workstation and also reassigns the station if it is a different one.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("assignparttoworkstation"),
         HttpPost]
        public IEnumerable<StationTaskModel> AssignPartToWorkstation([FromBody] AssignTargetToModel model)
        {
            var partWithTasks =
                Repositories.PartRepository.Entities.Include(x => x.AssignedTasks).FirstOrDefault(x => x.Id == model.Id);
            if (partWithTasks == null)
                throw new InvalidOperationException("Part with ID: " + model.Id + " does not exist.");
            var taskAssignments = partWithTasks.AssignedTasks.ToListOrderedBySequence();
            if (taskAssignments.Count == 0)
                throw new InvalidOperationException("Part '" + partWithTasks.Number + "' has no tasks to assign.");
            var workstation =
                Repositories.WorkstationRepository.Entities
                    .Where(x => x.Id == model.TargetId)
                    .Select(x => new
                    {
                        x.Id,
                        x.StationId,
                        x.Station.Section.AssemblyLine.FactoryId
                    })
                    .FirstOrDefault();
            if (workstation == null)
                throw new InvalidOperationException("Workstation with ID: " + model.TargetId + " does not exist.");
            var processPlanId =
                Repositories.ProcessPlanRepository.Entities.Where(x => x.FactoryId == workstation.FactoryId)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            var repo = GetRepository;
            var result = new List<StationTaskModel>();
            // we are in correct sequence order, so we will assign them in the same order as specified by the TaskAssignments
            for (var i = 0; i < taskAssignments.Count; i++)
            {
                var taskAssignment = taskAssignments[i];
                var stationTaskAssignment = repo.Entities
                    .Include(x => x.Task)
                    .Include(x => x.Part)
                    .Include(x => x.EquipmentConfiguration)
                    .FirstOrDefault(x => x.BasedOnTaskAssignmentId == taskAssignment.Id);
                if (stationTaskAssignment == null)
                {
                    // first time assignment
                    stationTaskAssignment = CreateFromTaskAssignment(taskAssignment, processPlanId);
                    repo.Create(stationTaskAssignment);
                }
                var previousWorkstation = stationTaskAssignment.WorkstationId;
                stationTaskAssignment.StationId = workstation.StationId;
                stationTaskAssignment.WorkstationId = workstation.Id;
                // always append the task in sequence
                MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, workstation.Id);

                if (previousWorkstation != workstation.Id)
                {
                    MaintainEquipmentConfiguraton(stationTaskAssignment, workstation.Id);
                }

                repo.SaveChanges();
                result.Add(Mapper.Map<StationTaskAssignment, StationTaskModel>(stationTaskAssignment));
            }
            return result;
        }

        [Route("unassigntaskfromworkstation"),
         HttpPost]
        public StationTaskModel UnassignTaskFromWorkstation([FromBody] BaseModel model)
        {
            var repo = Repositories.StationTaskAssignmentRepository;
            var stationTaskAssignment = repo.Entities
                    .Include(x => x.Task)
                    .Include(x => x.Part)
                    .FirstOrDefault(x => x.Id == model.Id);
            if (stationTaskAssignment == null)
                throw new InvalidOperationException("Station Task Assignment with ID: " + model.Id + " does not exist.");

            stationTaskAssignment.WorkstationId = null;
            MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, null);
            // if this task moved from somewhere else we need to repair the current equipment
            // remove equipment configurations
            RemoveEquipmentConfiguration(stationTaskAssignment);
            repo.SaveChanges();
            return Mapper.Map<StationTaskAssignment, StationTaskModel>(stationTaskAssignment);
        }

        #endregion

        #region Get for Stations and Workstations

        [Route("bystation/{stationId}"), HttpGet]
        public IEnumerable<StationTaskModel> GetAssignedTaskOfStation(long stationId)
        {
            return GetRepository.Entities.Where(x => x.StationId == stationId).Project().To<StationTaskModel>();
        }

        [Route("byworkstation/{stationId}"), HttpGet]
        public IEnumerable<StationTaskModel> GetAssignedTaskOfWorkstation(long stationId)
        {
            return GetRepository.Entities.Where(x => x.WorkstationId == stationId).Project().To<StationTaskModel>();
        }

        #endregion

        #region Get Count on Stations

        [Route("count/bysection/{sectionId}"), HttpGet]
        public IEnumerable<CountModel> GetStationTaskCountsPerSection(long sectionId)
        {
            var result = Repositories.StationRepository.Entities.Where(x => x.SectionId == sectionId).Select(x => new CountModel()
            {
                Id = x.Id,
                Count = x.TaskAssignments.Count()
            }).ToList();
            return result;
        }

        [Route("count/bystation/{stationId}"), HttpGet]
        public IEnumerable<CountModel> GetWorkstationTaskCountsPerStation(long stationId)
        {
            var result = Repositories.WorkstationRepository.Entities.Where(x => x.StationId == stationId).Select(x => new CountModel()
            {
                Id = x.Id,
                Count = x.TaskAssignments.Count()
            }).ToList();
            return result;
        }

        #endregion

        #region Move in Sequence

        [Route("move"),
         HttpPost,
         HttpPut]
        public StationTaskModel MoveStationTask([FromBody] MoveLinkedEntityModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (model.Id == model.PredecessorId)
                throw new InvalidOperationException("You cannot set this entity to be a predecessor of himself.");
            var repo = GetRepository;
            var entity = repo.Entities.Include(x => x.Predecessor).FirstOrDefault(x => x.Id == model.Id);
            if (entity == null)
                throw new InvalidOperationException("Station Task with ID: " + model.Id + " does not exist.");
            if (entity.WorkstationId == null) throw new InvalidOperationException("You cannot move a task as long it is not assigned to a workstation.");

            MaintainStationTaskSequenceForWorkstation(entity, entity.WorkstationId, model.PredecessorId);
            repo.SaveChanges();

            return Mapper.Map<StationTaskAssignment, StationTaskModel>(entity);
        }

        #endregion

        #region Update times

        // Lukas: removed for now because updating times is not possible by now
        //[Route("updatetimes"),
        // HttpPost,
        // HttpPut]
        //public StationTaskModel Update([FromBody] StationTaskModel model)
        //{
        //    if (model == null) throw new ArgumentNullException("model");
        //    var repo = GetRepository;
        //    var entity = repo.Entities.FirstOrDefault(x => x.Id == model.Id);
        //    if (entity == null) throw new InvalidOperationException("Station Task with ID:" + model.Id + " does not exist.");

        //    entity.Time = model.ExpectedTime;

        //    repo.SaveChanges();

        //    return Mapper.Map<StationTaskAssignment, StationTaskModel>(entity);
        //}

        #endregion

        #region Toggle Show In Workerscreen

        [Route("toggleShowInWorkerScreen/{id}"),
         HttpPost]
        public HttpResponseMessage ToggleShowInWorkerScreen(long id)
        {
            var assignment = GetRepository.Entities.FirstOrDefault(x => x.Id == id);
            if (assignment == null) throw new InvalidOperationException("There is no assignment with ID:" + id);
            assignment.ShowInTerminal = !assignment.ShowInTerminal;
            GetRepository.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        #endregion

        #region Delete

        [Route("delete/{id}"),
         HttpPost,
         HttpDelete]
        public bool Delete(long id)
        {
            var repo = GetRepository;
            var entity = repo.Entities.FirstOrDefault(x => x.Id == id);
            if (entity == null) throw new InvalidOperationException("Station Task with ID:" + id + " does not exist.");
            LinkedEntityHelper.RemoveLinkedEntity(repo.Entities, entity);
            repo.Delete(entity);
            return repo.SaveChanges();
        }

        #endregion

        #region Assign Equipment to Station Task

        [Route("assignequipmenttype/{override:bool?}"),
         HttpPost]
        public EquipmentConfigurationModel AssignEquipmentType([FromBody] AssignTargetToModel model, bool? @override = null)
        {
            var repo = GetRepository;
            var assignment =
                repo.Entities.Include(x => x.EquipmentConfiguration)
                    .Include(x => x.BasedOnTaskAssignment.NeededEquipmentType)
                    .FirstOrDefault(x => x.Id == model.Id);
            if (assignment == null)
                throw new InvalidOperationException("Station Task Assignment with ID: " + model.Id + " does not exist.");
            if (assignment.WorkstationId == null) throw new InvalidOperationException("Please assign the Task Assignment to a Workstation first.");
            if (assignment.EquipmentConfiguration != null)
            {
                if (!@override.GetValueOrDefault())
                    throw new InvalidOperationException(
                        "There is already an equipment assigned. Use the flag /true to override");
                RemoveEquipmentConfiguration(assignment);
                repo.SaveChanges();
            }

            var equipmentType = Repositories.EquipmentTypeRepository.Entities.FirstOrDefault(x => x.Id == model.TargetId);
            if (equipmentType == null)
                throw new InvalidOperationException("There is no equipment type with ID:" + model.TargetId);
            // let's check the task assignment if we have the right equipment
            if (assignment.BasedOnTaskAssignment.NeededEquipmentTypeId != null &&
                assignment.BasedOnTaskAssignment.NeededEquipmentTypeId != equipmentType.Id)
                throw new InvalidOperationException(
                    "The Task Assignment defines a different equipment type for this task: " +
                    assignment.BasedOnTaskAssignment.NeededEquipmentType.Name);

            // look for equipment of that type
            var equipmentConfiguration =
                Repositories.EquipmentConfigurationRepository.Entities.FirstOrDefault(
                    x => x.EquipmentTypeId == equipmentType.Id && x.WorkStationId == assignment.WorkstationId);
            if (equipmentConfiguration == null)
            {
                // let's create an instance of that equipment
                equipmentConfiguration = Repositories.EquipmentConfigurationRepository.Create();
                equipmentConfiguration.EquipmentType = equipmentType;
                equipmentConfiguration.WorkStationId = assignment.WorkstationId;
                equipmentConfiguration.Name = "Default " + equipmentType.Name + " instance";
                // TODO: this is not good :)
                if (equipmentType.Name.Contains("helf")) equipmentConfiguration.Configuration = "3:3";
            }
            assignment.EquipmentConfiguration = equipmentConfiguration;
            repo.SaveChanges();

            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(equipmentConfiguration);
        }

        [Route("assignequipment/{override:bool?}"),
         HttpPost]
        public EquipmentConfigurationModel AssignEquipment([FromBody] AssignTargetToModel model, bool? @override)
        {
            var repo = GetRepository;
            var assignment =
                repo.Entities.Include(x => x.EquipmentConfiguration)
                    .Include(x => x.BasedOnTaskAssignment.NeededEquipmentType)
                    .FirstOrDefault(x => x.Id == model.Id);
            if (assignment == null)
                throw new InvalidOperationException("Station Task Assignment with ID: " + model.Id + " does not exist.");
            if (assignment.WorkstationId == null) throw new InvalidOperationException("Please assign the Task Assignment to a Workstation first.");

            var equipment =
                Repositories.EquipmentConfigurationRepository.Entities.Include(x => x.EquipmentType)
                    .FirstOrDefault(x => x.Id == model.TargetId);
            if (equipment == null)
                throw new InvalidOperationException("There is no equipment with ID:" + model.TargetId);

            // first check if this is an equipment from the same workstation
            if (equipment.WorkStationId != assignment.WorkstationId)
                throw new InvalidOperationException(
                    "The equipment you want to assign is from another Workstation. You can assign this task only to equipment from the same Workstation.");

            // let's check the task assignment if we have the right equipment
            if (assignment.BasedOnTaskAssignment.NeededEquipmentTypeId != null &&
                assignment.BasedOnTaskAssignment.NeededEquipmentTypeId != equipment.EquipmentTypeId)
                throw new InvalidOperationException(
                    "The Task Assignment defines a different equipment type for this task: " +
                    assignment.BasedOnTaskAssignment.NeededEquipmentType.Name +
                    ". The equipment you want to use is of type: " + equipment.EquipmentType.Name + ".");

            if (assignment.EquipmentConfiguration != null)
            {
                if (!@override.GetValueOrDefault())
                    throw new InvalidOperationException(
                        "There is already an equipment assigned. Use the flag /true to override");
                assignment.EquipmentConfigurationId = model.TargetId;
            }
            else
            {
                // let's create an instance of that equipment
                equipment.StationTaskAssignments.Add(assignment);
            }

            repo.SaveChanges();

            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(equipment);
        }

        [Route("unassignequipment"),
         HttpPost]
        public bool UnassignEquipment([FromBody] BaseModel model)
        {
            var repo = GetRepository;
            var assignment =
                repo.Entities.Include(x => x.EquipmentConfiguration)
                    .Include(x => x.BasedOnTaskAssignment.NeededEquipmentType)
                    .FirstOrDefault(x => x.Id == model.Id);
            if (assignment == null)
                throw new InvalidOperationException("Station Task Assignment with ID: " + model.Id + " does not exist.");
            if (assignment.EquipmentConfiguration != null)
            {
                RemoveEquipmentConfiguration(assignment);
                repo.SaveChanges();
                return true;
            }
            return false;
        }

        [Route("moveequipment"),
         HttpPost]
        public EquipmentConfigurationModel MoveEquipment([FromBody] AssignTargetToModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var equipmentInstance =
                Repositories.EquipmentConfigurationRepository.Entities.Include(x => x.StationTaskAssignments).FirstOrDefault(x => x.Id == model.Id);
            if (equipmentInstance == null) throw new InvalidOperationException("There is no equipment configuration with ID:" + model.Id);
            var workstation = Repositories.WorkstationRepository.Entities.FirstOrDefault(x => x.Id == model.TargetId);
            if (workstation == null)
                throw new InvalidOperationException("There is no workstation with ID:" + model.TargetId);
            // already at that workstation? => no need to do anything
            if (equipmentInstance.WorkStationId == workstation.Id)
                return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(equipmentInstance);
            // ok lets do the reassign of task assignments
            var stationTasksToReassign = equipmentInstance.StationTaskAssignments.ToListOrderedBySequence();
            foreach (var stationTaskAssignment in stationTasksToReassign)
            {
                stationTaskAssignment.WorkstationId = workstation.Id;
                stationTaskAssignment.StationId = workstation.StationId;
                MaintainStationTaskSequenceForWorkstation(stationTaskAssignment, workstation.Id);
            }
            // and finally move the equipment
            equipmentInstance.WorkStationId = workstation.Id;
            GetRepository.SaveChanges();
            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(equipmentInstance);
        }

        #endregion

        #region Assign Driver to Equipment

        [Route("assigndriver"),
         HttpPost]
        public EquipmentConfigurationModel AssignDriver([FromBody] AssignTargetToModel model)
        {
            var equipmentConfig =
                Repositories.StationTaskAssignmentRepository.Entities.Include(x => x.EquipmentConfiguration)
                    .Where(x => x.Id == model.Id)
                    .Select(x => x.EquipmentConfiguration)
                    .FirstOrDefault();
            if (equipmentConfig == null)
                throw new InvalidOperationException("There is no equipment configuration for station task ID:" + model.Id);
            var driver = Repositories.EquipmentDriverRepository.Entities.FirstOrDefault(x => x.Id == model.TargetId);
            if (driver == null) throw new InvalidOperationException("There is no driver with ID:" + model.TargetId);
            equipmentConfig.UsedDriver = driver;
            GetRepository.SaveChanges();
            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(equipmentConfig);
        }

        [Route("configureequipment"),
         HttpPost]
        public EquipmentConfigurationModel ConfigureDriver([FromBody] ConfigureEquipmentConfigurationModel model)
        {
            var equipmentConfiguration =
                 Repositories.StationTaskAssignmentRepository.Entities.Include(x => x.EquipmentConfiguration)
                     .Where(x => x.Id == model.Id)
                     .Select(x => x.EquipmentConfiguration)
                     .FirstOrDefault();
            if (equipmentConfiguration == null)
                throw new InvalidOperationException("There is no equipment configuration for station task ID:" + model.Id);
            equipmentConfiguration.IpAddress = model.IpAddress;
            GetRepository.SaveChanges();

            return Mapper.Map<EquipmentConfiguration, EquipmentConfigurationModel>(equipmentConfiguration);
        }
        #endregion

        #region Configure Equipment Driver

        [Route("configuredriver"),
         HttpPost]
        public EquipmentDriverConfigurationModel ConfigureDriverForTask([FromBody] ConfigureStationTaskDriverModel model)
        {
            var repo = GetRepository;
            var assingment =
                repo.Entities.Include(x => x.EquipmentDriverConfiguration.Values).FirstOrDefault(x => x.Id == model.Id);
            if (assingment == null)
                throw new InvalidOperationException("There is no station task assingment with ID:" + model.Id);
            // first time configuration?
            if (assingment.EquipmentDriverConfiguration == null)
            {
                assingment.EquipmentDriverConfiguration = new EquipmentDriverConfiguration();
                assingment.EquipmentDriverConfiguration.Values = new List<EquipmentDriverConfigurationValue>();
            }
            var configValues = assingment.EquipmentDriverConfiguration.Values;
            foreach (var pair in model.Values)
            {
                var existingSetting = configValues.FirstOrDefault(x => x.Name == pair.Key);
                if (existingSetting != null) existingSetting.Value = pair.Value;
                else
                {
                    configValues.Add(new EquipmentDriverConfigurationValue()
                    {
                        Name = pair.Key,
                        Value = pair.Value
                    });
                }
            }
            repo.SaveChanges();

            return
                Mapper.Map<EquipmentDriverConfiguration, EquipmentDriverConfigurationModel>(
                    assingment.EquipmentDriverConfiguration);
        }

        [Route("adddriverconfig"),
         HttpPost]
        public EquipmentDriverConfigurationModel AddDriverConfiguration([FromBody] AddDriverConfigurationModel model)
        {
            var repo = GetRepository;
            var assingment =
                repo.Entities.Include(x => x.EquipmentDriverConfiguration.Values).FirstOrDefault(x => x.Id == model.Id);
            if (assingment == null)
                throw new InvalidOperationException("There is no station task assingment with ID:" + model.Id);
            // first time configuration?
            if (assingment.EquipmentDriverConfiguration == null)
            {
                assingment.EquipmentDriverConfiguration = new EquipmentDriverConfiguration();
                assingment.EquipmentDriverConfiguration.Values = new List<EquipmentDriverConfigurationValue>();
            }
            var driverConfig =
                assingment.EquipmentDriverConfiguration.Values.FirstOrDefault(x => x.Name == model.Value.Key);
            if (driverConfig == null)
                assingment.EquipmentDriverConfiguration.Values.Add(
                    driverConfig = new EquipmentDriverConfigurationValue() { Name = model.Value.Key });
            driverConfig.Value = model.Value.Value;
            repo.SaveChanges();
            return
                Mapper.Map<EquipmentDriverConfiguration, EquipmentDriverConfigurationModel>(
                    assingment.EquipmentDriverConfiguration);
        }

        [Route("removedriverconfig"),
         HttpPost,
         HttpDelete]
        public bool RemoveDriverConfiguration([FromBody] RemoveDriverConfigurationModel model)
        {
            var repo = GetRepository;
            var assingment =
                repo.Entities.Include(x => x.EquipmentDriverConfiguration.Values).FirstOrDefault(x => x.Id == model.Id);
            if (assingment == null)
                throw new InvalidOperationException("There is no station task assingment with ID:" + model.Id);
            var driverConfig =
                assingment.EquipmentDriverConfiguration.Values.FirstOrDefault(x => x.Name == model.Key);
            if (driverConfig != null)
            {
                Repositories.EquipmentDriverConfigurationValuesRepository.Delete(driverConfig);
                repo.SaveChanges();
            }
            return true;
        }

        #endregion

        #region Upload Images

        [Route("upload"),
         HttpPost]
        public async Task<HttpResponseMessage> UploadStationTaskImage(HttpRequestMessage request)
        {
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var data = await Request.Content.ParseMultipartAsync();
            if (!data.Fields.ContainsKey("id"))
                throw new InvalidOperationException(
                    "You must provide an 'id' in the form pointing to the Station task assignment.");
            var idString = data.Fields["id"].Value;
            long id;
            if (!Int64.TryParse(idString, out id))
                throw new InvalidOperationException("The specified field 'id' does not contain a valid int64. Value=" +
                                                    idString);

            var assignment = GetRepository.Entities.FirstOrDefault(x => x.Id == id);
            if (assignment == null)
                throw new InvalidOperationException("The Station task assignment with ID:" + id + " does not exist.");

            var currentAssignedImageId = assignment.TaskAssignmentImageId;
            if (!data.Files.ContainsKey("file")) throw new InvalidOperationException("There is no image included in a field called 'file'.");

            var fileContent = data.Files["file"];
            var hash = GetImageHash(fileContent.File);
            var existingImage = Repositories.TaskAssignmentImageRepository.Entities.FirstOrDefault(x => x.Hash == hash);
            if (existingImage == null)
            {
                existingImage = Repositories.TaskAssignmentImageRepository.Create();
                existingImage.Image = fileContent.File;
                existingImage.Hash = hash;
                existingImage.Type = fileContent.MimeType;
                Repositories.TaskAssignmentImageRepository.SaveChanges();
            }
            assignment.TaskAssignmentImage = existingImage;
            Repositories.TaskAssignmentImageRepository.SaveChanges();
            // check if we need to remove the unused picure
            if (currentAssignedImageId != null && GetRepository.Entities.All(x => x.TaskAssignmentImageId != currentAssignedImageId))
            {
                var deleteImage =
                    Repositories.TaskAssignmentImageRepository.Entities.FirstOrDefault(
                        x => x.Id == currentAssignedImageId);
                Repositories.TaskAssignmentImageRepository.Delete(deleteImage);
                Repositories.TaskAssignmentImageRepository.SaveChanges();
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Thank you for uploading the file...")
            };
        }

        [Route("image/{id}"),
         HttpGet]
        public HttpResponseMessage GetImage(long id)
        {
            var assignment = GetRepository.Entities.Include(x => x.TaskAssignmentImage).FirstOrDefault(x => x.Id == id);
            if (assignment == null) throw new InvalidOperationException("There is no assignment with ID:" + id);
            if (assignment.TaskAssignmentImage == null)
                throw new InvalidOperationException("There is no image for assignment " + assignment.Name);
            var response = new HttpResponseMessage { Content = new ByteArrayContent(assignment.TaskAssignmentImage.Image) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(assignment.TaskAssignmentImage.Type);
            response.Content.Headers.ContentLength = assignment.TaskAssignmentImage.Image.Length;
            return response;
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Create a StationTaskAssignment from an TaskAssignment template.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private StationTaskAssignment CreateFromTaskAssignment(TaskAssignment template, long? processPlanId)
        {
            var repo = GetRepository;
            var entity = repo.Create();
            entity.AssignedProcessPlanId = processPlanId;
            entity.Part = template.Part;
            entity.Task = template.Task;
            entity.Name = template.Name;
            entity.Description = template.Description;
            entity.Time = template.Time;
            entity.BasedOnTaskAssignment = template;
            entity.ValidFrom = DateTime.Now;
            entity.ShowInTerminal = template.Task.ShowInTerminal;
            entity.TaskAssignmentImage = template.TaskAssignmentImage;
            return entity;
        }

        private bool MaintainEquipmentConfiguraton(StationTaskAssignment assignment, long? newWorkstationId)
        {
            // we need to create a copy of the current equipment configuration (if any)
            if (assignment.EquipmentConfiguration != null)
            {
                if (newWorkstationId != null)
                {
                    // check the target workstation
                    var equipmentConfig =
                        Repositories.EquipmentConfigurationRepository.Entities.FirstOrDefault(
                            x =>
                                x.WorkStationId == newWorkstationId &&
                                x.EquipmentTypeId == assignment.EquipmentConfiguration.EquipmentTypeId);
                    if (equipmentConfig == null)
                    {
                        equipmentConfig =
                            CopyEquipmentForWorkstation(assignment.EquipmentConfiguration);
                        equipmentConfig.WorkStationId = newWorkstationId;
                    }
                    //RemoveEquipmentConfiguration(assignment);
                    assignment.EquipmentConfiguration = equipmentConfig;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This sets the sequence of the given StationTaskAssignment (STA) to be the last within the sequence of already assigned STA.
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="workstationId"></param>
        private bool MaintainStationTaskSequenceForWorkstation(StationTaskAssignment assignment, long? workstationId)
        {
            var repo = GetRepository;
            // this is a remove operation from the context of linked station task assignments for a workstation
            LinkedEntityHelper.RemoveLinkedEntity(repo.Entities, assignment);
            // if the workstationId is null (not assigned to a particular workstation)
            // we will clear the sequence
            if (workstationId == null)
            {
                assignment.PredecessorId = null;
                return true;
            }
            var currentWorkstationAssignmentsOrdered =
                repo.Entities.Where(x => x.WorkstationId == workstationId.Value).ToListOrderedBySequence();
            if (currentWorkstationAssignmentsOrdered.Count == 0)
            {
                // the first assignment to this workstation
                assignment.PredecessorId = null;
                return true;
            }
            assignment.PredecessorId =
                currentWorkstationAssignmentsOrdered[currentWorkstationAssignmentsOrdered.Count - 1].Id;
            return true;
        }

        /// <summary>
        /// This sets the sequence of the given StationTaskAssignment (STA) to the given Predecessor. In case the given Predecessor does not belong 
        /// to the same Workstation an error will be thrown. In case the Workstation is null we will clear the sequence.
        /// If the given predecessor is null this will be added at the First place.
        /// </summary>
        /// <param name="assignment"></param>
        /// <param name="workstationId"></param>
        /// <param name="predecessorId"></param>
        /// <returns></returns>
        private bool MaintainStationTaskSequenceForWorkstation(StationTaskAssignment assignment, long? workstationId, long? predecessorId)
        {
            var repo = GetRepository;
            if (workstationId == null)
            {
                return LinkedEntityHelper.RemoveLinkedEntity(repo.Entities, assignment);
            }
            var currentWorkstationAssignments =
                repo.Entities.Where(x => x.WorkstationId == workstationId.Value);
            // here we do not need the ordered list since we have all we need to know
            var template = new MoveLinkedEntityModel() { PredecessorId = predecessorId };
            return LinkedEntityHelper.MoveLinkedEntity(currentWorkstationAssignments, assignment, template);
        }

        private EquipmentConfiguration CopyEquipmentForWorkstation(EquipmentConfiguration template)
        {
            var newEntity = Repositories.EquipmentConfigurationRepository.Create();
            newEntity.UsedDriverId = template.UsedDriverId;
            newEntity.Name = template.Name;
            newEntity.Description = template.Description;
            newEntity.IpAddress = template.IpAddress;
            newEntity.EquipmentTypeId = template.EquipmentTypeId;
            return newEntity;
        }

        private void RemoveEquipmentConfiguration(StationTaskAssignment stationTaskAssignment, bool deleteEquipmentDriverConfiguration = true)
        {
            if (stationTaskAssignment.EquipmentConfigurationId != null)
            {
                var equipmentConfig =
                    Repositories.EquipmentConfigurationRepository.Entities.Include(x => x.StationTaskAssignments)
                        .FirstOrDefault(x => x.Id == stationTaskAssignment.EquipmentConfigurationId);
                if (equipmentConfig != null && equipmentConfig.StationTaskAssignments.Count(x => x.Id != stationTaskAssignment.Id) == 0)
                {
                    stationTaskAssignment.EquipmentConfigurationId = null;
                    Repositories.EquipmentConfigurationRepository.Delete(equipmentConfig);
                }
            }
            if (deleteEquipmentDriverConfiguration && stationTaskAssignment.EquipmentDriverConfigurationId != null)
            {
                Repositories.EquipmentDriverConfigurationRepository.DeleteEquipmentDriverConfiguration(
                    stationTaskAssignment.EquipmentDriverConfigurationId.Value);
            }
        }

        private string GetImageHash(byte[] imageBinary)
        {
            var provider = new MD5CryptoServiceProvider();
            var hash = provider.ComputeHash(imageBinary);
            var builder = new StringBuilder();
            for (var i = 0; i < hash.Length; i++) builder.Append(hash[i].ToString("x2"));
            return builder.ToString();
        }

        #endregion
    }
}
