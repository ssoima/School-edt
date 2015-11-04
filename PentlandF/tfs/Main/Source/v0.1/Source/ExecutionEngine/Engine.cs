using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Timers;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using NextLAP.IP1.Common;
using NextLAP.IP1.Common.Configuration;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Equipment;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.ExecutionEngine.Repositories;
using Timer = System.Timers.Timer;
using NextLAP.IP1.ExecutionEngine.Helper;
using Microsoft.AspNet.SignalR;
using NextLAP.IP1.ExecutionEngine.Hubs;

namespace NextLAP.IP1.ExecutionEngine
{
    internal class Engine : IDisposable
    {
        private static readonly Logger Log = LogManager.GetLogger(typeof(Engine));

        private bool _isInitialized = false;
        private EngineSettings _engineSettings;
        private readonly long _factoryId;
        private static Engine _instance = null;
        private List<StationConfiguration> _stations;
        private List<WorkstationConfiguration> _workstations;
        private List<EquipmentConfiguration> _equipments;
        private List<WorkstationTaskConfiguration> _stationTasks;
        private List<ManufacturingOrder> _orders;
        private List<CurrentWorkstationTaskProgress> _currentTaskProgresses;
        private double _lastReportedPosition = 0;
        private double _lastPositionTimerKnows = 0;
        private readonly Timer _saveOrderProgressTimer;
        private readonly object _updateProgressLock = new object();
        private bool _updateInProgress = false;
        private static double _currentSpeed = 0;
        private readonly Lazy<IHubContext> _conveyanceHubContext = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<ConveyanceServiceHub>());
        public event EventHandler<NewOrdersInStationEventArgs> NewOrdersInStation;
        public event EventHandler<EquipmentTaskProgressChangedEventArgs> TaskProgressChanged;
        public event EventHandler<ConveyanceProgressChangedEventArgs> ConveyanceStarted;
        public event EventHandler<ConveyanceProgressChangedEventArgs> ConveyanceStopped;
        public event EventHandler<ConveyanceProgressChangedEventArgs> ConveyanceProgressChanged;

        private EquipmentCoordinator _equipmentCoordinator;

        private Engine()
        {
        }

        private Engine(string factoryId)
        {
            Int64.TryParse(factoryId, out _factoryId);
            _saveOrderProgressTimer = new Timer(10000);
            _saveOrderProgressTimer.Elapsed += OnSaveChangesTimerElapsed;
        }

        public static Engine Instance
        {
            get
            {
                return _instance ?? (_instance = new Engine(ConfigurationManager.AppSettings["factoryId"]));
            }
        }

        public void BootstrapEngine()
        {
            if (_isInitialized) return;

            Log.Debug("Bootstrapping Engine...");
            Log.Debug("Checking data store for current version of data..");

            _engineSettings = MongoRepositories.EngineSettings.FirstOrDefault();
            if (_engineSettings == null)
            {
                Log.Debug("No settings yet. Initiating first instance.");
                _engineSettings = new EngineSettings()
                {
                    CurrentAssignmentVersion = 0,
                    LastConveyancePosition = 0
                };
                MongoRepositories.EngineSettings.Add(_engineSettings);
            }
            // restore conveyance position
            _lastReportedPosition = _lastPositionTimerKnows = _engineSettings.LastConveyancePosition;
            Log.Debug("Local version: " + _engineSettings.CurrentAssignmentVersion);
            _isInitialized = true;

            CheckForProcessPlanUpdates();
            CheckForStationUpdates();
            CheckForWorkstationUpdates();
            LoadOrders();
            LoadCurrentTaskProgresses();
            // ok let's init the coordinator
            _equipmentCoordinator = new EquipmentCoordinator(this);
            _equipmentCoordinator.AddOrUpdateEquipmentConfigurations(_equipments);
            _equipmentCoordinator.CoordinateTaskLoad(_currentTaskProgresses);

            _saveOrderProgressTimer.Start();
        }

        #region Check For Updates

        private void CheckForProcessPlanUpdates()
        {
            if (!_isInitialized) return;

            Log.Debug("Checking Process plan for updates...");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings[Constants.AppSettings.ProcessPlanWebAPIUrl]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var result = client.GetStringAsync("processplan/version/" + _factoryId).Result;

                var reportedVersion = 0;
                if (!Int32.TryParse(result, out reportedVersion))
                {
                    Log.Debug("The reported version cannot be parsed into a number. (Reported version: '" + result + "')");
                    return;
                }
                var versionsMatch = reportedVersion == _engineSettings.CurrentAssignmentVersion;
                Log.Debug("Current version:  " + reportedVersion);
                if (!versionsMatch)
                {
                    Log.Debug("Updating data store...");
                    var stationTasksRequestResult = client.GetAsync("processplan/stationconfiguration/" + _factoryId).Result;
                    if (stationTasksRequestResult.IsSuccessStatusCode)
                    {
                        var processPlanDescriptor =
                            stationTasksRequestResult.Content.ReadAsAsync<ProcessPlanDescriptionModel>().Result;
                        var stationTasks = processPlanDescriptor.StationWorkload.ToList();

                        var workstationTaskConfigurations = new List<WorkstationTaskConfiguration>();
                        var equipmentConfigurations = new List<EquipmentConfiguration>();
                        foreach (var stationTask in stationTasks)
                        {
                            foreach (var workstation in stationTask.WorkstationWorkloads)
                            {
                                workstationTaskConfigurations.AddRange(
                                    workstation.Tasks.Select(workload => new WorkstationTaskConfiguration()
                                    {
                                        StationTaskAssignmentId = workload.Id,
                                        StationId = stationTask.StationId,
                                        Station = stationTask.StationName,
                                        WorkstationId = workstation.WorkstationId,
                                        Workstation = workstation.WorkstationName,
                                        PredecessorId = workload.PredecessorId,
                                        PartId = workload.PartId,
                                        PartName = workload.Part,
                                        PartNumber = workload.PartNumber,
                                        TaskId = workload.TaskId,
                                        Task = workload.Task,
                                        ShowInTerminal = workload.ShowInTerminal,
                                        TaskImageUrl = workload.ImageUrl,
                                        IsEquipmentRequired = workload.IsEquipmentRequired,
                                        EquipmentId = workload.EquipmentId,
                                        DriverConfiguration =
                                            workload.DriverConfiguration.ToDictionary(x => x.Key, x => x.Value)
                                    }));
                                equipmentConfigurations.AddRange(
                                    workstation.Equipments.Select(equipment => new EquipmentConfiguration()
                                    {
                                        EquipmentId = equipment.Id,
                                        Name = equipment.Name,
                                        Type = equipment.EquipmentType,
                                        Configuration = equipment.Configuration,
                                        Driver = equipment.Driver,
                                        IpAddress = equipment.IpAddress
                                    }));
                            }
                        }
                        Log.Debug("Read successfully " + workstationTaskConfigurations.Count + " task configurations..");
                        MongoRepositories.WorkstationConfigurations.DeleteAll();
                        if (workstationTaskConfigurations.Count > 0) MongoRepositories.WorkstationConfigurations.Add(workstationTaskConfigurations);
                        else Log.Warning("There are no workstation task informations available.");
                        Log.Debug("Task configurations successfully updated..");

                        Log.Debug("Read successfully " + equipmentConfigurations.Count + " equipment configurations..");
                        MongoRepositories.EquipmentConfigurations.DeleteAll();
                        if (equipmentConfigurations.Count > 0) MongoRepositories.EquipmentConfigurations.Add(equipmentConfigurations);
                        else Log.Warning("There are no equipment configurations available.");
                        Log.Debug("Equipment configurations successfully updated..");

                        // TODO: update settings
                        //setting.CurrentAssignmentVersion = reportedVersion;
                        //MongoRepositories.EngineSettings.Update(setting);

                        _stationTasks = workstationTaskConfigurations;
                        _equipments = equipmentConfigurations;
                    }
                    else
                    {
                        Log.Debug("Failed to load current version of data. Reason: " + stationTasksRequestResult.ReasonPhrase);
                    }
                }
            }
        }

        private void CheckForStationUpdates()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings[Constants.AppSettings.ProcessPlanWebAPIUrl]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var stationsRequestResult = client.GetAsync("stations/byfactory/" + _factoryId).Result;
                Log.Debug("Updating stations data store...");
                if (stationsRequestResult.IsSuccessStatusCode)
                {
                    var stationsResult =
                        stationsRequestResult.Content.ReadAsAsync<IEnumerable<StationModel>>().Result;
                    var stations = stationsResult.ToListOrderedBySequence();
                    var currentOffset = 0;

                    var stationConfigurations = new List<StationConfiguration>();
                    var position = 0;
                    foreach (var station in stations)
                    {
                        stationConfigurations.Add(new StationConfiguration()
                        {
                            StationId = station.Id,
                            StationName = station.Name,
                            Position = position,
                            ConveyanceOffset = currentOffset,
                            Length = station.Length
                        });
                        position++;
                        currentOffset += station.Length;
                    }
                    Log.Debug("Read successfully " + stationConfigurations.Count + " station configurations..");
                    MongoRepositories.Stations.DeleteAll();
                    MongoRepositories.Stations.Add(stationConfigurations);
                    Log.Debug("Station configurations successfully updated..");
                    _stations = stationConfigurations;
                }
                else
                {
                    Log.Debug("Failed to load current stations. Reason: " + stationsRequestResult.ReasonPhrase);
                }
            }
        }

        private void CheckForWorkstationUpdates()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings[Constants.AppSettings.ProcessPlanWebAPIUrl]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var stationsRequestResult = client.GetAsync("workstations/byfactory/" + _factoryId).Result;
                Log.Debug("Updating workstations data store...");
                if (stationsRequestResult.IsSuccessStatusCode)
                {
                    var stationsResult =
                        stationsRequestResult.Content.ReadAsAsync<IEnumerable<WorkstationModel>>().Result;
                    var stations = stationsResult.ToList();

                    var wsConfigurations = new List<WorkstationConfiguration>();
                    foreach (var station in stations)
                    {
                        wsConfigurations.Add(new WorkstationConfiguration()
                        {
                            WorkstationId = station.Id,
                            WorkstationName = station.Name,
                            StationId = station.StationId.GetValueOrDefault(0)
                        });
                    }
                    Log.Debug("Read successfully " + wsConfigurations.Count + " workstation configurations..");
                    MongoRepositories.Workstations.DeleteAll();
                    MongoRepositories.Workstations.Add(wsConfigurations);
                    Log.Debug("Workstation configurations successfully updated..");
                    _workstations = wsConfigurations;
                }
                else
                {
                    Log.Debug("Failed to load current workstations. Reason: " + stationsRequestResult.ReasonPhrase);
                }
            }
        }

        #endregion

        #region Preload MongoDB stuff into memory

        private void LoadOrders()
        {
            _orders = MongoRepositories.ManufacturingOrders.ToList();
            Log.Debug(_orders.Count + " orders loaded.");
        }

        private void LoadCurrentTaskProgresses()
        {
            _currentTaskProgresses = MongoRepositories.WorkstationProgress.Where(x => x.Complete == false).ToList();
            Log.Debug(_currentTaskProgresses.Count + " current equipment tasks in progress loaded.");
        }

        #endregion

        #region Methods called from Order Web Site

        public void EnqueueOrder(ExecutionEngineOrderDescriptionModel order)
        {
            Log.Debug("Checking for existing order..");
            var currentOrder = MongoRepositories.ManufacturingOrders.FirstOrDefault(x => x.OrderId == order.OrderNumber);
            if (currentOrder != null)
            {
                Log.Debug("Order '" + order.OrderNumber + "' already queued.");
                return;
            }
            currentOrder = new ManufacturingOrder()
            {
                OrderId = order.OrderNumber,
                Model = order.Model,
                Variant = order.Variant,
                Customer = order.Customer,
                PartList = order.PartNumbers
            };
            MongoRepositories.ManufacturingOrders.Add(currentOrder);
            _orders.Add(currentOrder);
            Log.Debug("Queuing order '" + order.OrderNumber + "'");
        }

        #endregion

        #region Methods called from Workstation Terminals

        public WorkstationInfo GetWorkstationInfo(long workstationId)
        {
            var ws = _workstations.FirstOrDefault(x => x.WorkstationId == workstationId);
            if (ws == null) return null;
            var s = _stations.FirstOrDefault(x => x.StationId == ws.StationId);
            if (s == null) return null;
            return new WorkstationInfo()
            {
                Station = s.StationName,
                StationId = s.StationId,
                Workstation = ws.WorkstationName,
                WorkstationId = ws.WorkstationId,
                Length = s.Length,
                Offset = s.ConveyanceOffset,
                Speed = _currentSpeed
            };
        }

        public TerminalOrderInfo GetCurrentOrderInfo(long workstationId)
        {
            var workstation = _workstations.FirstOrDefault(x => x.WorkstationId == workstationId);
            if (workstation == null) throw new InvalidOperationException("There is no workstation with ID:" + workstationId);
            var stationOrder = _orders.FirstOrDefault(x => x.CurrentStationId == workstation.StationId);
            if (stationOrder == null) return null;
            var result = ResolveTerminalOrderInfos(new[] { stationOrder }).FirstOrDefault(x => x.WorkstationId == workstationId);
            return result;
        }

        public void StartConveyanceBelt(long triggeredByWorkstationId)
        {
            // TODO: save in DB who did that
            _conveyanceHubContext.Value.Clients.All.Start();
        }

        public void StopConveyanceBelt(long triggeredByWorkstationId)
        {
            // TODO: save in DB who did that
            _conveyanceHubContext.Value.Clients.All.Stop();
        }

        #endregion

        public IEnumerable<TerminalOrderInfo> ResolveTerminalOrderInfos(IEnumerable<ManufacturingOrder> orders)
        {
            var result = new List<TerminalOrderInfo>();
            foreach (var order in orders)
            {
                // load the general task configuration
                var taskConfigurationsForStation =
                    _stationTasks.Where(
                        x => x.StationId == order.CurrentStationId &&
                            order.PartList.Contains(x.PartNumber) &&
                            x.ShowInTerminal) // only those marked to be relevant for terminal
                        .GroupBy(x => new
                        {
                            x.WorkstationId,
                            x.StationId
                        });
                var orderTaskProgress = _currentTaskProgresses.Where(x => x.OrderId == order.OrderId).ToList();
                foreach (var configPerWorkstation in taskConfigurationsForStation)
                {
                    var orderInfo = new TerminalOrderInfo();
                    orderInfo.OrderNumber = order.OrderId;
                    orderInfo.Model = order.Model;
                    orderInfo.Variant = order.Variant;
                    orderInfo.CurrentPosition = order.CurrentPosition;
                    orderInfo.StationId = configPerWorkstation.Key.StationId;
                    orderInfo.WorkstationId = configPerWorkstation.Key.WorkstationId;
                    var orderIndex = _orders.FindIndex(x => x.OrderId == order.OrderId);
                    var previousOrder = orderIndex == 0 ? null : _orders[orderIndex - 1].OrderId;
                    var nextOrder = orderIndex + 1 >= _orders.Count
                        ? null
                        : _orders[orderIndex + 1].OrderId;
                    orderInfo.OrderSequence = new OrderSequence()
                    {
                        Previous = previousOrder,
                        Current = order.OrderId,
                        Next = nextOrder
                    };
                    orderInfo.TaskSequenceInfos = new List<TerminalTaskSequenceInfo>();
                    var i = 0;
                    var ids = new Func<EquipmentTaskProgress, long>(x => x.StationTaskAssignmentId);
                    var ps = new Func<EquipmentTaskProgress, long?>(x => x.PredecessorId);
                    var taskProgress = orderTaskProgress
                        .Where(x => x.WorkstationId == configPerWorkstation.Key.WorkstationId)
                        .SelectMany(x => x.TaskProgress.Select(y => new LinkedEntityAdapter<EquipmentTaskProgress>(y, ids, ps))
                        .ToListOrderedBySequence());
                    foreach (var task in taskProgress)
                    {
                        orderInfo.TaskSequenceInfos.Add(new TerminalTaskSequenceInfo()
                        {
                            Id = task.Adaptee.StationTaskAssignmentId,
                            WorkstationId = configPerWorkstation.Key.WorkstationId,
                            Order = i++,
                            Part = task.Adaptee.PartNumber + " (" + task.Adaptee.PartName + ")",
                            Task = task.Adaptee.Task,
                            ImageUrl = task.Adaptee.TaskImageUrl,
                            IsEquipmentRequired = task.Adaptee.IsEquipmentRequired,
                            Equipment = task.Adaptee.Equipment != null ? task.Adaptee.Equipment.Name : null,
                            Success = task.Adaptee.InProgress ? null : (bool?)task.Adaptee.Success,
                            InProgress = task.Adaptee.InProgress,
                            Comment = task.Adaptee.Comment
                        });
                    }
                    result.Add(orderInfo);
                }
            }
            return result;
        }

        public IEnumerable<CurrentWorkstationTaskProgress> CreateWorkstationTaskProgress(IEnumerable<ManufacturingOrder> orders)
        {
            var result = new List<CurrentWorkstationTaskProgress>();
            foreach (var order in orders)
            {
                // load the general task configuration
                var taskConfigurationsForStation =
                    _stationTasks.Where(
                        x => x.StationId == order.CurrentStationId &&
                            order.PartList.Contains(x.PartNumber) &&
                            x.ShowInTerminal)
                        .GroupBy(x => new
                        {
                            x.WorkstationId,
                            x.StationId
                        });
                foreach (var configPerWorkstation in taskConfigurationsForStation)
                {
                    var progress = new CurrentWorkstationTaskProgress();
                    progress.OrderId = order.OrderId;
                    progress.StationId = configPerWorkstation.Key.StationId;
                    progress.WorkstationId = configPerWorkstation.Key.WorkstationId;
                    progress.Complete = false;
                    progress.TaskProgress = configPerWorkstation
                        .Select(x =>
                        {
                            var equipment = x.EquipmentId != null;
                            return new EquipmentTaskProgress()
                            {
                                WorkstationId = configPerWorkstation.Key.WorkstationId,
                                TaskId = x.TaskId,
                                PartId = x.PartId,
                                Task = x.Task,
                                PartName = x.PartName,
                                PartNumber = x.PartNumber,
                                StationTaskAssignmentId = x.StationTaskAssignmentId,
                                PredecessorId = x.PredecessorId,
                                TaskImageUrl = x.TaskImageUrl,
                                InProgress = equipment,
                                Completed = !equipment,
                                Success = !equipment,
                                IsEquipmentRequired = equipment,
                                Equipment = _equipments.FirstOrDefault(eq => eq.EquipmentId == x.EquipmentId),
                                Configuration = equipment ? new Dictionary<string, string>(x.DriverConfiguration) : null
                            };
                        }).ToList();

                    result.Add(progress);
                }
            }
            return result;
        }



        /// <summary>
        /// The concept of the conveyance belt is that it moves a position marker to a specific length. When the length is reached, the position jumps back to 0.
        /// The order positions on the other hand will track their logical position. We need to track that information also because we know only the logical offsets of 
        /// the stations.
        /// </summary>
        /// <param name="model"></param>
        public void OnConveyanceMovement(ConveyanceProgressModel model)
        {
            SlimUpdatePosition(model.Position);
            // if nothing to do we immediately jump out (no orders or no stations)
            if (_orders.Count == 0 || _stations.Count == 0) return;
            lock (_updateProgressLock)
            {
                _updateInProgress = true;
                Log.Debug("Thread: " + Thread.CurrentThread.ManagedThreadId);

                if (model.MaxReached)
                {
                    // if the conveyance belt in now restarting with positions from 0 we need to align
                    _lastReportedPosition = _lastReportedPosition - model.TotalLength;
                }
                var lengthMoved = model.Position - _lastReportedPosition;
                _lastReportedPosition = model.Position;
                Log.Debug("Conveyance reports new position at: " + model.Position + " - moved by " + Math.Round(lengthMoved, 2) + "cm..");

                ManufacturingOrder queuedForFirstStation = null;
                bool firstStationFree = true;
                var ordersThatChangedStation = new List<ManufacturingOrder>();
                var firstStationId = _stations[0].StationId;
                var positionForCommencingIntoFirstStation = 0d;
                foreach (var manufacturingOrder in _orders)
                {
                    var newLogicalPosition = manufacturingOrder.CurrentPosition + lengthMoved;
                    var currentStation = manufacturingOrder.CurrentStationId == null
                        ? null
                        : _stations.FirstOrDefault(x => x.StationId == manufacturingOrder.CurrentStationId);
                    // the order has not yet been commenced
                    if (currentStation == null)
                    {
                        // check first if there is already something queued to be processed..
                        if (queuedForFirstStation == null) queuedForFirstStation = manufacturingOrder;
                        // all other orders must wait until they will be processed
                        continue;
                    }
                    // we use index based search for the next station
                    var nextStation = (currentStation.Position >= _stations.Count - 1)
                        ? null
                        : _stations[currentStation.Position + 1];
                    // there is no other station anymore?
                    if (nextStation == null)
                    {
                        // we are through all stations
                        // TODO: notify
                        continue;
                    }
                    // this order has reached a new station
                    if (nextStation.ConveyanceOffset <= newLogicalPosition)
                    {
                        // in case this order left station 1 we calculate the theoretical correct first position for the to be commenced next order
                        if (manufacturingOrder.CurrentStationId == firstStationId)
                            positionForCommencingIntoFirstStation = newLogicalPosition - nextStation.ConveyanceOffset;
                        // set the tasks to complete 
                        // TODO: check if all tasks have been done
                        var currentTasks =
                            _currentTaskProgresses.FirstOrDefault(x => x.StationId == manufacturingOrder.CurrentStationId &&
                                                                       x.OrderId == manufacturingOrder.OrderId);
                        if (currentTasks == null)
                        {
                            Log.Error("There seems to be no tasks to set to complete for Order ID: " +
                                      manufacturingOrder.OrderId + " for Station:" +
                                      currentStation.StationName);
                        }
                        else
                        {
                            // mark task progress to be completed
                            CompleteTasks(currentTasks);
                            // and we will remove it from our internal list
                            _currentTaskProgresses.Remove(currentTasks);
                        }
                        manufacturingOrder.CurrentStationId = nextStation.StationId;
                        Log.Debug("Order '" + manufacturingOrder.OrderId + "' moves to '" +
                                  nextStation.StationName + "'..");
                        // collect this order to send notifications
                        ordersThatChangedStation.Add(manufacturingOrder);

                    }
                    // always progress position
                    manufacturingOrder.CurrentPosition = newLogicalPosition;
                    // prevent queuing into station 1 if occupied
                    // check first for the flag (if it is set to false already we do not need to reevaluate the principal condition)
                    if (firstStationFree && manufacturingOrder.CurrentStationId == firstStationId) firstStationFree = false;
                }
                // anything in the pipe for putting into station one?
                if (firstStationFree && queuedForFirstStation != null)
                {
                    queuedForFirstStation.CurrentStationId = firstStationId;
                    queuedForFirstStation.CurrentPosition = positionForCommencingIntoFirstStation;
                    Log.Debug("Order '" + queuedForFirstStation.OrderId + "' is now in progress..");
                    Log.Debug("Order '" + queuedForFirstStation.OrderId + "' is now in station '" +
                              _stations[0].StationName + "'..");
                    ordersThatChangedStation.Add(queuedForFirstStation);

                    // TODO: notify exe web that order is in progress
                }
                if (ordersThatChangedStation.Count > 0)
                {
                    // we are going to prepare the equipment configurations here
                    // for this we need to resolve first the equipments we need
                    var newProgresses = CreateWorkstationTaskProgress(ordersThatChangedStation).ToList();
                    if (newProgresses.Count > 0)
                    {
                        // add them to our internal list
                        _currentTaskProgresses.AddRange(newProgresses);
                        // update in DB
                        MongoRepositories.WorkstationProgress.Add(newProgresses);
                        // notify coordinator
                        _equipmentCoordinator.CoordinateTaskLoad(newProgresses);
                    }
                    // notify listeners
                    OnNewOrdersInStation(ordersThatChangedStation);
                }
                // and we notify all listeners
                OnConveyanceProgressChanged(model);
                _currentSpeed = model.Speed;
                _updateInProgress = false;
            }
        }

        private void CompleteTasks(CurrentWorkstationTaskProgress task)
        {
            var uncomplete = task.TaskProgress.Where(x => !x.Completed).ToList();
            for (var i = 0; i < uncomplete.Count; i++)
            {
                var uc = uncomplete[i];
                uc.InProgress = false;
                uc.Completed = true;
                if (uc.Equipment == null)
                {
                    // this is fine and we will set all properties to IO
                    uc.Success = true;
                }
                else
                {
                    Log.Error("Uncompleted task '" + uc.Task + "' for part '" + uc.PartNumber + "'..");
                    uc.Success = false;
                }
            }
            // notify equipment coordinator that tasks need to be cancelled
            _equipmentCoordinator.CancelTasks(task);
            task.Complete = true;
            MongoRepositories.WorkstationProgress.Update(task);
        }

        private void SlimUpdatePosition(double conveyancePosition)
        {
            var collection = MongoRepositories.EngineSettings.Collection;
            var query = Query.EQ("_id", ObjectId.Parse(_engineSettings.Id));
            var update = Update.Set("lastConveyancePosition", conveyancePosition);
            collection.Update(query, update);
        }

        private void SlimUpdateTaskComplete(CurrentWorkstationTaskProgress task)
        {
            var collection = MongoRepositories.WorkstationProgress.Collection;
            var query = Query.EQ("_id", ObjectId.Parse(task.Id));
            var update = Update.Set("complete", true);
            collection.Update(query, update);
        }

        public void UpdateTaskProgress(EquipmentTaskProgress progress)
        {
            var task = _currentTaskProgresses.FirstOrDefault(x => x.TaskProgress.Contains(progress));
            if (task == null) Log.Error("Could not find task.");
            else
            {
                MongoRepositories.WorkstationProgress.Update(task);
                OnTaskProgressChanged(progress);
            }
        }

        public void UpdateTaskComment(string orderId, long workstationId, long stationTaskId, string comment)
        {
            // we will get the one from the mongodb
            var collection = MongoRepositories.WorkstationProgress.Collection;
            var filter = Query.And(Query.EQ("progress.id", stationTaskId), Query.EQ("oId", orderId), Query.EQ("wId", workstationId));
            var entity = collection.FindOne(filter);
            if (entity == null)
            {
                Log.Error("No current task found for station task ID:" + stationTaskId);
                return;
            }
            var task = entity.TaskProgress.FirstOrDefault(x => x.StationTaskAssignmentId == stationTaskId);
            if (task == null)
            {
                // this shouldn't happen at this location anymore
                Log.Error("Couldn't find the task anymore");
                return;
            }
            task.Comment = comment;
            MongoRepositories.WorkstationProgress.Update(entity);
            // and update potentially our internal model
            var internalModel = _currentTaskProgresses.FirstOrDefault(x => x.Id == entity.Id);
            if (internalModel != null)
            {
                var taskToUpdate =
                    internalModel.TaskProgress.FirstOrDefault(
                        x => x.StationTaskAssignmentId == task.StationTaskAssignmentId);
                if (taskToUpdate != null) taskToUpdate.Comment = comment;
            }
        }

        public void NotifyConveyanceStarted(ConveyanceProgressModel model)
        {
            _engineSettings.LastConveyancePosition = model.Position;
            _currentSpeed = model.Speed;
            OnConveyanceStarted(model);
        }

        public void NotifyConveyanceStopped(ConveyanceProgressModel model)
        {
            Log.Debug("Notifying conveyance stopped...");
            if (model != null) _engineSettings.LastConveyancePosition = model.Position;
            _currentSpeed = 0;
            OnConveyanceStopped(model);
        }

        private void OnNewOrdersInStation(IEnumerable<ManufacturingOrder> orders)
        {
            if (NewOrdersInStation != null)
                NewOrdersInStation(this, new NewOrdersInStationEventArgs(orders));
        }

        private void OnTaskProgressChanged(EquipmentTaskProgress task)
        {
            if (TaskProgressChanged != null)
                TaskProgressChanged(this, new EquipmentTaskProgressChangedEventArgs(task));
        }

        private void OnConveyanceProgressChanged(ConveyanceProgressModel model)
        {
            if (ConveyanceProgressChanged != null)
                ConveyanceProgressChanged(this, new ConveyanceProgressChangedEventArgs(model));
        }
        private void OnConveyanceStarted(ConveyanceProgressModel model)
        {
            if (ConveyanceStarted != null)
                ConveyanceStarted(this, new ConveyanceProgressChangedEventArgs(model));
        }

        private void OnConveyanceStopped(ConveyanceProgressModel model)
        {
            if (ConveyanceStopped != null)
                ConveyanceStopped(this, new ConveyanceProgressChangedEventArgs(model));
        }

        private void OnSaveChangesTimerElapsed(object sender, ElapsedEventArgs args)
        {
            SaveOrders();
        }

        internal void SaveOrders()
        {
            // if no progress has been made we do nothing here
            if (_lastPositionTimerKnows.Equals(_lastReportedPosition)) return;
            // if there is an update in progress we will spin here for a while
            while (_updateInProgress)
            {
                Thread.Sleep(1);
            }
            _lastPositionTimerKnows = _lastReportedPosition;
            MongoRepositories.ManufacturingOrders.Update(_orders);
            Log.Debug("Orders have been saved.");
        }

        public void Dispose()
        {
            if (_saveOrderProgressTimer != null) _saveOrderProgressTimer.Dispose();
            if (_equipmentCoordinator != null) _equipmentCoordinator.Dispose();
            _equipmentCoordinator = null;
            _instance = null;
        }
    }
}
