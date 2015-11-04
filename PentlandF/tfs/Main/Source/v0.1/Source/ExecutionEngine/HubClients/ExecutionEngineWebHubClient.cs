using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using NextLAP.IP1.Common.Configuration;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.ExecutionEngine.Models;

namespace NextLAP.IP1.ExecutionEngine.HubClients
{
    internal sealed class ExecutionEngineWebHubClient : IDisposable
    {
        private const int DefaultConnectionWait = 5000;
        private static readonly Logger Log = LogManager.GetLogger(typeof (ExecutionEngineWebHubClient));
        private readonly HubConnection _connection;
        private readonly IHubProxy _proxy;
        private readonly Engine _engine;
        private bool _isConnected = false;

        public ExecutionEngineWebHubClient(Engine engine)
        {
            _engine = engine;

            var url = ConfigurationManager.AppSettings[Constants.AppSettings.ExecutionEngineWebAPIUrl];
            Log.Debug("Connecting to Execution Engine Web at " + url);
            if (string.IsNullOrEmpty(url))
                throw new InvalidOperationException("Missing URL to the execution engine API url in config.appSettings.");

            _connection = new HubConnection(url);
            _proxy = _connection.CreateHubProxy(Constants.HubProxyNames.ExecutionEngineComHub);

            InitHandlers();
            Connect();
        }

        private void InitHandlers()
        {
            _proxy.On<ExecutionEngineOrderDescriptionModel>("EnqueueOrder", EnqueueOrderReceived);
        }

        private void Connect()
        {
            if(_isConnected) return;
            _isConnected = _connection.Start().Wait(DefaultConnectionWait);
            Log.Debug("Connected..." + _isConnected);
        }

        #region Event Handlers

        private void EnqueueOrderReceived(ExecutionEngineOrderDescriptionModel order)
        {
            Log.Debug("Enqueue order [" + order.OrderNumber + "] received...");
            _engine.EnqueueOrder(order);
        } 

        #endregion

        public void Dispose()
        {
            if(_connection != null) _connection.Dispose();
        }
    }
}
