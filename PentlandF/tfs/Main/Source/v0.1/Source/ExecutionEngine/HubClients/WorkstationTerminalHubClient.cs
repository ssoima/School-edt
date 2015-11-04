using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using NextLAP.IP1.Common.Configuration;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Models;

namespace NextLAP.IP1.ExecutionEngine.HubClients
{
    internal class WorkstationTerminalHubClient : IDisposable
    {
        private const int DefaultConnectionWait = 5000;
        private static Logger Log = LogManager.GetLogger(typeof(WorkstationTerminalHubClient));
        private readonly Engine _engine;
        private bool _isConnected = false;
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;

        public WorkstationTerminalHubClient(Engine engine)
        {
            _engine = engine;

            // start connection
            var url = ConfigurationManager.AppSettings[Constants.AppSettings.WorkstationAPIUrl];
            Log.Debug("Connecting to Workstation Terminal at " + url);
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("Missing URL to the wrokstation terminal API url in config.appSettings.");

            _connection = new HubConnection(url);
            _proxy = _connection.CreateHubProxy(Constants.HubProxyNames.TerminalExecutionHub);

            InitHandlers();
            Connect();
        }

        private void InitHandlers()
        {
            // register handlers on engine
            _engine.NewOrdersInStation += NotifyOrdersInStation;
            _engine.ConveyanceStarted += NotifyConveyanceStarted;
            _engine.ConveyanceStopped += NotifyConveyanceStopped;
            _engine.ConveyanceProgressChanged += NotifyConveyanceProgress;
            _engine.TaskProgressChanged += NotifyTaskProgressChanged;
            // on proxy
            _proxy.On<long>("GetCurrentOrder", GetCurrentOrder);
            _proxy.On<long>("GetWorkstationInfo", GetWorkstationInfo);
            _proxy.On<long>("StartConveyanceBelt", StartConveyanceBelt);
            _proxy.On<long>("StopConveyanceBelt", StopConveyanceBelt);
            _proxy.On<string, long, long, string>("ChangeTaskComment", ChangeTaskComment);
        }

        public void NotifyOrdersInStation(object sender, NewOrdersInStationEventArgs args)
        {
            var list = args.Orders.ToList();
            Log.Debug(list.Count + " stations will get notified about a new order in station.");
            var result = _engine.ResolveTerminalOrderInfos(list);
            _proxy.Invoke<TerminalOrderInfo>("NotifyOrdersInStation", result);
        }

        public void NotifyTaskProgressChanged(object sender, EquipmentTaskProgressChangedEventArgs args)
        {
            var task = args.Progress;
            var model = new TerminalTaskSequenceInfo()
            {
                WorkstationId = task.WorkstationId,
                Task = task.Task,
                Part = task.PartNumber,
                // TODO: create different model?!
                Success = task.Success
            };
            _proxy.Invoke<TerminalTaskSequenceInfo>("NotifyTaskProgressChanged", model);
        }

        private void NotifyConveyanceStarted(object sender, ConveyanceProgressChangedEventArgs args)
        {
            Log.Debug("Notify started..");
            _proxy.Invoke<ConveyanceProgressModel>("NotifyConveyanceStarted", args.ConveyanceProgress);
            Log.Debug("Notified..");
        }

        private void NotifyConveyanceStopped(object sender, ConveyanceProgressChangedEventArgs args)
        {
            Log.Debug("Notify stopped..");
            _proxy.Invoke<ConveyanceProgressModel>("NotifyConveyanceStopped", args.ConveyanceProgress);
            Log.Debug("Notified..");
        }

        private void NotifyConveyanceProgress(object sender, ConveyanceProgressChangedEventArgs args)
        {
            _proxy.Invoke<ConveyanceProgressModel>("NotifyConveyanceProgress", args.ConveyanceProgress);
        }

        public void GetCurrentOrder(long workstationId)
        {
            var result = _engine.GetCurrentOrderInfo(workstationId);

            if (result != null)
            {
                // needs to be threaded because otherwise it gets blocked
                new Thread(() =>
                {
                    try
                    {
                        _proxy.Invoke<TerminalOrderInfo>("GetCurrentOrderInfoSucceeded", result).Wait();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("An error occured when trying to send a message: " + ex.Message);
                        if (ex.InnerException != null) Log.Error("Details: " + ex.InnerException.Message);
                    }
                }).Start();
            }
        }

        public void GetWorkstationInfo(long workstationId)
        {
            Log.Debug("Workstation Info for " + workstationId);
            var result = _engine.GetWorkstationInfo(workstationId);

            if (result != null)
            {
                // needs to be threaded because otherwise it gets blocked
                new Thread(() =>
                {
                    try
                    {
                        _proxy.Invoke<WorkstationInfo>("GetWorkstationInfoSucceeded", result).Wait();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("An error occured when trying to send a message: " + ex.Message);
                        if(ex.InnerException != null) Log.Error("Details: " + ex.InnerException.Message);
                    }
                }).Start();
            }
        }

        public void StartConveyanceBelt(long triggeredByWorkstationId)
        {
            Log.Debug("Called to start conveyance belt");
            _engine.StartConveyanceBelt(triggeredByWorkstationId);
        }

        public void StopConveyanceBelt(long triggeredByWorkstationId)
        {
            Log.Debug("Called to stop conveyance belt");
            _engine.StopConveyanceBelt(triggeredByWorkstationId);
        }

        public void ChangeTaskComment(string orderId, long workstationId, long stationTaskId, string comment)
        {
            Log.Debug("Change comment received..");
            _engine.UpdateTaskComment(orderId, workstationId, stationTaskId, comment);
        }

        private void Connect()
        {
            if (_isConnected) return;
            _isConnected = _connection.Start().Wait(DefaultConnectionWait);
            Log.Debug("Connected..." + _isConnected);
        }

        public void Dispose()
        {
            _engine.NewOrdersInStation -= NotifyOrdersInStation;
            _engine.ConveyanceStarted -= NotifyConveyanceStarted;
            _engine.ConveyanceStopped -= NotifyConveyanceStopped;
            _engine.ConveyanceProgressChanged -= NotifyConveyanceProgress;

            if (_connection != null) _connection.Dispose();
        }
    }
}
