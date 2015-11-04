using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using AutoMapper;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Planning;
using NextLAP.IP1.Models.Structure;
using NextLAP.IP1.PlanningWebAPI.Models;
using NextLAP.IP1.PlanningWebAPI.Models.PlantLayout;
using NextLAP.IP1.PlanningWebAPI.Models.ProcessPlan;
using StationModel = NextLAP.IP1.PlanningWebAPI.Models.PlantLayout.StationModel;

namespace NextLAP.IP1.PlanningWebAPI.Startup
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Company, CompanyModel>();
                cfg.CreateMap<Factory, FactoryModel>();
                cfg.CreateMap<AssemblyLine, AssemblyLineModel>();
                cfg.CreateMap<Section, SectionModel>();
                cfg.CreateMap<Station, StationModel>()
                    .ForMember(model => model.Type, option => option.MapFrom(source => source.StationType != null ? source.StationType.Name : string.Empty))
                    .ForMember(model => model.Length, option => option.MapFrom(source => source.StationType != null ? source.StationType.Length : 0));
                cfg.CreateMap<StationType, StationTypeModel>();
                cfg.CreateMap<Workstation, NextLAP.IP1.PlanningWebAPI.Models.PlantLayout.WorkstationModel>()
                    .ForMember(model => model.StationName, opt => opt.MapFrom(source => source.Station.Name));
                cfg.CreateMap<EquipmentType, EquipmentTypeModel>();

                cfg.CreateMap<Task, TaskModel>();
                cfg.CreateMap<Part, PartModel>();

                #region Process Planning 

                #region StationTaskAssignment => StationTaskModel

                cfg.CreateMap<StationTaskAssignment, StationTaskModel>()
                    .ForMember(model => model.Part, option => option.MapFrom(source => new PartModel()
                    {
                        Id = source.Part.Id,
                        Name = source.Part.Name,
                        Description = source.Part.Description
                    }))
                    .ForMember(model => model.Task, option => option.MapFrom(source => new TaskModel()
                    {
                        Id = source.Task.Id,
                        Name = source.Task.Name,
                        Description = source.Task.Description
                    }))
                    .ForMember(model => model.ExpectedTime, opt => opt.MapFrom(s => s.Time))
                    .ForMember(model => model.AssignedStationId, 
                        option => option.MapFrom(source => source.StationId))
                    .ForMember(model => model.AssignedWorkstationId,
                        option => option.MapFrom(source => source.WorkstationId));

                #endregion

                #region Part => AssignalbePartListModel

                cfg.CreateMap<Part, AssignablePartListModel>()
                    .ForMember(model => model.PartNumber, opt => opt.MapFrom(s => s.Number))
                    .ForMember(model => model.AssignableTasks,
                        opt => opt.MapFrom(s => s.AssignedTasks.Select(x => new AssignableTaskModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Task = x.Task.Name,
                            StationdId =
                                x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                    .Select(sta => sta.StationId)
                                    .FirstOrDefault(),
                            StationName =
                                x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                    .Select(sta => sta.Station.Name)
                                    .FirstOrDefault(),
                            WorkstationId =
                                x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                    .Select(sta => sta.WorkstationId)
                                    .FirstOrDefault(),
                            WorkstationName =
                                x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                    .Select(sta => sta.Workstation.Name)
                                    .FirstOrDefault(),
                            NeedsEquipment = x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                .Select(sta => sta.Task.NeedsEquipment)
                                .FirstOrDefault(),
                            AssignedEquipmentId = x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                .Select(sta => sta.EquipmentConfigurationId)
                                .FirstOrDefault(),
                            AssignedEquipment = x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                .Select(sta => sta.EquipmentConfiguration.EquipmentType.Name)
                                .FirstOrDefault(),
                            CurrentStationTaskAssignmentId =
                                x.StationTaskAssignments.Where(sta => sta.Version == x.Version)
                                    .Select(sta => sta.Id)
                                    .FirstOrDefault(),
                        })));

                #endregion

                #region Station => StationDetailsOverviewModel

                cfg.CreateMap<Station, StationDetailsOverviewModel>()
                    .ForMember(model => model.Workstations, opt => opt.MapFrom(s => s.WorkStations.Select(ws => new NextLAP.IP1.PlanningWebAPI.Models.PlantLayout.WorkstationModel()
                    {
                        Id = ws.Id,
                        Name = ws.Name,
                        Description = ws.Description,
                        Position = ws.Position,
                        Side = ws.Side,
                        StationId = ws.StationId,
                        StationName = ws.Station.Name,
                        Type = ws.Type,
                        Equipments = ws.EquipmentConfigurations.Select(eq => new EquipmentConfigurationModel()
                        {
                            Id = eq.Id,
                            Name = eq.Name,
                            Description = eq.Description,
                            EquipmentTypeId = eq.EquipmentTypeId,
                            EquipmentType = eq.EquipmentType.Name,
                            AssignedDriver = eq.UsedDriver.ClrType,
                            AssignedDriverId = eq.UsedDriverId,
                            IpAddress = eq.IpAddress,
                            Configuration = eq.Configuration,
                            Workstation = ws.Name,
                            WorkstationId = ws.Id
                        })
                    })))
                    .ForMember(model => model.AssignedTasks,
                        opt =>
                            opt.MapFrom(
                                s => s.TaskAssignments.Select(sta => new StationDetailsTaskModel()
                                {
                                    Id = sta.Id,
                                    Name = sta.Name,
                                    Description = sta.Description,
                                    Part = sta.Part.Number,
                                    Task = sta.Task.Action.ToString(),
                                    StationdId = sta.StationId,
                                    StationName = sta.Station.Name,
                                    WorkstationId = sta.WorkstationId,
                                    WorkstationName = sta.Workstation.Name,
                                    EquipmentConfigurationId = sta.EquipmentConfigurationId,
                                    EquipmentConfiguration = sta.EquipmentConfiguration.Name,
                                    EquipmentConfigurationConfiguration = sta.EquipmentConfiguration.Configuration,
                                    EquipmentDriverConfigurationId = sta.EquipmentDriverConfigurationId,
                                    // TODO: This is a (dirty) hotfix in order to return EquipmentDriverConfiguration for a StationTask assignment.
                                    // This currently only return the first configuration value, although there could be more in future.
                                    EquipmentDriverConfiguration = 
                                        sta.EquipmentDriverConfiguration.Values.FirstOrDefault() != null ? sta.EquipmentDriverConfiguration.Values.FirstOrDefault().Value : null,
                                    PredecessorId = sta.PredecessorId,
                                    Time = sta.Time,
                                    BasedOnTaskAssignmentId = sta.BasedOnTaskAssignmentId,
                                    ShowOnWorkerScreen = sta.ShowInTerminal,
                                    HasImage = sta.TaskAssignmentImageId != null
                                })));

                #endregion

                #region EquipmentConfiguration => EquipmentConfigurationModel

                cfg.CreateMap<EquipmentConfiguration, EquipmentConfigurationModel>()
                    .ForMember(model => model.AssignedDriverId, opt => opt.MapFrom(source => source.UsedDriverId))
                    .ForMember(model => model.AssignedDriver, opt => opt.MapFrom(source => source.UsedDriver.Name))
                    .ForMember(model => model.EquipmentType, opt => opt.MapFrom(source => source.EquipmentType.Name))
                    .ForMember(model => model.Workstation, opt => opt.MapFrom(source => source.WorkStation.Name));

                #endregion

                #region EquipmentDriver => EquipmentDriverModel

                cfg.CreateMap<EquipmentDriver, EquipmentDriverModel>()
                    .ForMember(model => model.EquipmentType, opt => opt.MapFrom(source => source.EquipmentType.Name));

                #endregion

                #region EquipmentDriverConfiguration => EquipmentDriverConfigurationModel

                cfg.CreateMap<EquipmentDriverConfiguration, EquipmentDriverConfigurationModel>();
                cfg.CreateMap<EquipmentDriverConfigurationValue, EquipmentDriverConfigurationValueModel>();

                #endregion

                #region ProcessPlan => ProcessPlanDescriptionModel

                cfg.CreateMap<ProcessPlan, ProcessPlanDescriptionModel>()
                    .ForMember(model => model.StationWorkload,
                        opt =>
                            opt.MapFrom(
                                source =>
                                    source.StationTaskAssignments.GroupBy(x => x.Station)
                                        .Select(x => new StationTasksDescriptor()
                                        {
                                            StationId = x.Key.Id,
                                            StationName = x.Key.Name,
                                            WorkstationWorkloads =
                                                x.GroupBy(ws => ws.Workstation)
                                                    .Select(ws => new WorkstationTasksDescriptor()
                                                    {
                                                        WorkstationId = ws.Key.Id,
                                                        WorkstationName = ws.Key.Name,
                                                        Tasks = ws.Select(wl => new TaskDescriptor()
                                                        {
                                                            Id = wl.Id,
                                                            PredecessorId = wl.PredecessorId,
                                                            TaskId = wl.Task.Id,
                                                            Task = wl.Task.Name,
                                                            PartId = wl.Part.Id,
                                                            Part = wl.Part.Name,
                                                            PartNumber = wl.Part.Number,
                                                            ShowInTerminal = wl.ShowInTerminal,
                                                            ImageUrl = wl.TaskAssignmentImageId != null ? "/stationtasks/image/" + wl.Id : null,
                                                            IsEquipmentRequired = wl.Task.NeedsEquipment,
                                                            EquipmentId = wl.EquipmentConfigurationId,
                                                            DriverConfiguration =
                                                                wl.EquipmentDriverConfiguration.Values.Select(conf => new NextLAP.IP1.ExecutionEngine.Models.KeyValue()
                                                                {
                                                                    Key = conf.Name,
                                                                    Value = conf.Value
                                                                }).ToList()
                                                        }).ToList(),
                                                        Equipments = ws.Where(eq => eq.EquipmentConfiguration != null).GroupBy(eq => eq.EquipmentConfiguration).Select(eq => new EquipmentDescriptor()
                                                        {
                                                            Id = eq.Key.Id,
                                                            Configuration = eq.Key.Configuration,
                                                            Driver = eq.Key.UsedDriver.ClrType,
                                                            EquipmentType = eq.Key.EquipmentType.Name,
                                                            Name = eq.Key.Name,
                                                            IpAddress = eq.Key.IpAddress
                                                        }).ToList()
                                                    }).ToList()
                                        })));

                #endregion

                #endregion
            });
        }
    }
}