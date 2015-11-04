using System;
using System.Collections.Generic;
using System.Linq;
using NextLAP.IP1.Common;
using NextLAP.IP1.EquipmentDrivers;
using NextLAP.IP1.ExecutionEngine.Models;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Helper;

namespace NextLAP.IP1.ExecutionEngine.Equipment
{
    internal class EquipmentCoordinator : IDisposable
    {
        private static readonly Logger Log = LogManager.GetLogger(typeof(EquipmentCoordinator));
        private Engine _engine = null;
        // map IP to Runner
        private Dictionary<string, IEquipment> _threads;
        // map Workstation to a queue of things to do
        private Dictionary<long, Queue<EquipmentTaskProgress>> _tasks;

        private Dictionary<IEquipment, long> _equipmentWorkstationMap; 

        public EquipmentCoordinator(Engine engine)
        {
            _engine = engine;
            _threads = new Dictionary<string, IEquipment>();
            _tasks = new Dictionary<long, Queue<EquipmentTaskProgress>>();
            _equipmentWorkstationMap = new Dictionary<IEquipment, long>();
        }

        public void AddOrUpdateEquipmentConfigurations(List<EquipmentConfiguration> equipments)
        {
            // TODO: for now updating does not work. This method is called only once.
            // TODO: check for empty IP?
            foreach (var equipmentConfiguration in equipments.Where(x => !string.IsNullOrEmpty(x.IpAddress)))
            {
                var driver = EquipmentDriverFactory.GetEquipment(equipmentConfiguration.Driver);
                _threads.Add(equipmentConfiguration.IpAddress, driver);
                driver.Connect(equipmentConfiguration.IpAddress);
                driver.Receive += DriverOnReceive;
            }
        }

        private void DriverOnReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            var workstationIdForEquipment = _equipmentWorkstationMap[messageReceivedEventArgs.Response.Source];
            var taskQueue = _tasks[workstationIdForEquipment];
            var taskProgress = taskQueue.Peek();
            taskProgress.AckResult = messageReceivedEventArgs.Response.Message;
            taskProgress.Success = messageReceivedEventArgs.Response.Success;
            taskProgress.InProgress = !taskProgress.Success;
            taskProgress.Completed = taskProgress.Success;

            _engine.UpdateTaskProgress(taskProgress);
            if (messageReceivedEventArgs.Response.Success)
            {
                taskQueue.Dequeue();
                if (taskQueue.Count > 0) SendCommand(taskQueue.Peek());
            }
        }

        /// <summary>
        /// this should be fed only with the very current tasks of each workstation.
        /// there shouldn't be more than 1 taskProgress per workstation
        /// </summary>
        /// <param name="taskProgresses"></param>
        public void CoordinateTaskLoad(IEnumerable<CurrentWorkstationTaskProgress> taskProgresses)
        {
            try
            {
                // ok now we will check if need to create a runner
                // let's go through the entries by IP
                var ids = new Func<EquipmentTaskProgress, long>(x => x.StationTaskAssignmentId);
                var ps = new Func<EquipmentTaskProgress, long?>(x => x.PredecessorId);
                foreach (var currentWorkstationTaskProgress in taskProgresses)
                {
                    // first order in logical sequence so we know what comes first
                    var taskProgress =
                        currentWorkstationTaskProgress.TaskProgress
                        .Select(y => new LinkedEntityAdapter<EquipmentTaskProgress>(y, ids, ps)) // use helper to order by sequence
                            .ToListOrderedBySequence()
                            .Select(x => x.Adaptee) // and convert back to original
                            .ToList();
                    // next we will select only those that have equipment and are in progress 
                    var equipmentBasedTaskProgress = taskProgress.Where(x => x.InProgress && x.Equipment != null).ToList();
                    Queue<EquipmentTaskProgress> queue;
                    if (_tasks.ContainsKey(currentWorkstationTaskProgress.WorkstationId))
                        queue = _tasks[currentWorkstationTaskProgress.WorkstationId];
                    else _tasks.Add(currentWorkstationTaskProgress.WorkstationId, queue = new Queue<EquipmentTaskProgress>());
                    foreach (var equipmentTaskProgress in equipmentBasedTaskProgress)
                    {
                        queue.Enqueue(equipmentTaskProgress);
                    }
                    if(queue.Count > 0) SendCommand(queue.Peek());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void CancelTasks(CurrentWorkstationTaskProgress taskProgress)
        {
            Log.Debug("Cancelling tasks");
        }

        //public void NotifyEquipmentResponded(EquipmentRunner runner, string response, EquipmentRunnable runnable)
        //{
        //    if (_engine == null) return; // already disposed
        //    var progress = runnable.TaskProgress;
        //    // the runner should give access to the driver so the driver can give a uniformed interpretation of the response
        //    //progress.Success = response == runnable.Command;
        //    progress.InProgress = !progress.Success;
        //    progress.Completed = true;
        //    progress.AckResult = response;
        //    _engine.UpdateTaskProgress(progress);
        //}

        private void SendCommand(EquipmentTaskProgress task)
        {
            // TODO: no ipaddress=null should come here
            if(task.Equipment == null || string.IsNullOrEmpty(task.Equipment.IpAddress)) return;
            var driver = _threads[task.Equipment.IpAddress];
            if (!_equipmentWorkstationMap.ContainsKey(driver)) _equipmentWorkstationMap.Add(driver, task.WorkstationId);
            driver.Send(task.Configuration);
        }

        public void Dispose()
        {
            foreach (var t in _threads)
            {
                t.Value.Receive -= DriverOnReceive;
                t.Value.Dispose();
            }
            _engine = null;
            _threads = null;
        }
    }
}
