using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Hosting;
using MongoDB.Driver.Linq;
using NextLAP.IP1.Common.Configuration;
using NextLAP.IP1.Common.Diagnostics;
using MongoRepository;
using NextLAP.IP1.ExecutionEngine.Models;
using Timer = System.Timers.Timer;

namespace NextLAP.IP1.ConveyanceService
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetLogger(typeof(Program));
        private const string DefaultEndpointUrl = "http://localhost:8080";
        private const int ConnectionTimeout = 8000;
        private static Timer ConveyanceTimer;
        private static System.Threading.Timer ConnectTimer;
        private const double DefaultTimerInterval = 2000; // milliseconds
        private const double DefaultConveyanceSpeed = 6.67; // centimeters per second
        private const int DefaultTotalLength = 25000;
        private static ConveyanceServiceModel _settings;
        private static HubConnection _connection;
        internal static IHubProxy ExecutionEngineClient;
        private static bool _executionEngineConnectionEstablished = false;

        private static readonly MongoRepository<ConveyanceServiceModel> ConveyanceServiceStore =
            new MongoRepository<ConveyanceServiceModel>();

        static void Main(string[] args)
        {
            Log.Debug("Conveyance Service started..");
            Log.Debug("Loading conveyance settings...");
            _settings = ConveyanceServiceStore.FirstOrDefault();
            if (_settings == null)
            {
                Log.Debug("No settings available..creating default.");
                _settings = new ConveyanceServiceModel()
                {
                    Position = 0,
                    TotalLength = DefaultTotalLength,
                    UpdateInterval = DefaultTimerInterval,
                    Speed = DefaultConveyanceSpeed,
                    Status = ConveyanceStatus.Stopped
                };
                ConveyanceServiceStore.Add(_settings);
            }

            var startingUrl = ConfigurationManager.AppSettings[Constants.AppSettings.RunningEndpointUrl] ??
                              DefaultEndpointUrl;

            var executionEngineUrl = ConfigurationManager.AppSettings[Constants.AppSettings.ExecutionEngineEndpointUrl] ??
                                     DefaultEndpointUrl;

            using (WebApp.Start(startingUrl))
            {
                try
                {
                    // setup timer for notifying listeners
                    ConveyanceTimer = new Timer(_settings.UpdateInterval);
                    ConveyanceTimer.Elapsed += notifyConveyanceService;
                    // when successful we are going to connect to the execution engine
                    var queryString = new Dictionary<string, string>()
                    {
                        {"type", Constants.ServiceType.Conveyance}
                    };
                    Log.Debug("Connecting to Execution Engine at " + executionEngineUrl);
                    using (_connection = new HubConnection(executionEngineUrl, queryString))
                    {
                        _connection.StateChanged += HubConnectionOnStateChanged;

                        ExecutionEngineClient = _connection.CreateHubProxy("ConveyanceServiceHub");
                        // define some OnSomething event handlers
                        ExecutionEngineClient.On("Start", Start);
                        ExecutionEngineClient.On("Stop", Stop);

                        _connection.Start();
                        
                        Log.Debug(string.Format("Server running on {0}", startingUrl));
                        Console.ReadLine();

                        Stop();
                        Log.Debug("Removing timer handlers..");
                        _connection.StateChanged -= HubConnectionOnStateChanged;
                        Log.Debug("Disposing conveyance timer..");
                        ConveyanceTimer.Dispose();
                        Log.Debug("Conveyance Timer disposed..");
                        ExecutionEngineClient = null;
                    }
                }
                finally
                {
                    Log.Debug("Updating settings..");
                    // before exit we are going to update the latest settings
                    ConveyanceServiceStore.Update(_settings);
                    Log.Debug("Updated..");
                    // dispose timers
                }
            }
            Console.WriteLine("Press ANY key to EXIT.");
            Console.ReadLine();
        }

        private static void HubConnectionOnStateChanged(StateChange stateChange)
        {
            Log.Debug("Connection to Execution Engine changed from [" + stateChange.OldState.ToString() + "] to [" + stateChange.NewState.ToString());
            switch (stateChange.NewState)
            {
                case ConnectionState.Connected:
                    _connected = true;
                    Start();
                    break;
                case ConnectionState.Disconnected:
                    Stop(false);
                    if (stateChange.OldState == ConnectionState.Connecting)
                    {
                        // we tried to connect but host not available..issue a retry in few seconds
                        if (ConnectTimer == null)
                            ConnectTimer = new System.Threading.Timer(new TimerCallback(ConnectTimerElapsed), null, 0,
                                1000);
                    }
                    break;
                case ConnectionState.Reconnecting:
                    Stop(false);
                    break;
                default:
                    break;
            }
            _currentlyConnecting = false;
        }

        private static bool _currentlyConnecting = false;
        private static bool _connected = false;
        private static void ConnectTimerElapsed(object state)
        {
            if(_currentlyConnecting) return;
            Log.Debug("Trying to connect..");
            if (_connected)
            {
                Log.Debug("Connected..");
                ConnectTimer.Dispose();
                ConnectTimer = null;
                return;
            }
            if (!_currentlyConnecting)
            {
                _currentlyConnecting = true;
                _connection.Start();
            }
        }

        private static void Start()
        {
            Start(true);
        }

        private static void Start(bool notifyEngine)
        {
            if (_settings.Status == ConveyanceStatus.Started) return;
            Log.Debug("Starting Conveyance Belt..");
            ConveyanceTimer.Start();
            _settings.Status = ConveyanceStatus.Started;
            ConveyanceServiceStore.Update(_settings);
            if (notifyEngine)
            {
                Log.Debug("Notifying started..");
                ExecutionEngineClient.Invoke<ConveyanceProgressModel>("ConveyanceStarted", new ConveyanceProgressModel()
                {
                    Position = _settings.Position,
                    TotalLength = _settings.TotalLength,
                    Speed = _settings.Speed,
                    UpdateInterval = _settings.UpdateInterval
                });
                Log.Debug("Notified");
            }
        }

        private static void Stop()
        {
            Stop(true);
        }

        private static void Stop(bool notifyEngine)
        {
            if (_settings.Status == ConveyanceStatus.Stopped) return;
            Log.Debug("Stopping Conveyance Belt..");
            ConveyanceTimer.Stop();
            _settings.Status = ConveyanceStatus.Stopped;
            ConveyanceServiceStore.Update(_settings);
            if (notifyEngine)
            {
                Log.Debug("Notifying stopped..");
                ExecutionEngineClient.Invoke<ConveyanceProgressModel>("ConveyanceStopped", new ConveyanceProgressModel()
                {
                    Position = _settings.Position,
                    TotalLength = _settings.TotalLength,
                    Speed = _settings.Speed,
                    UpdateInterval = _settings.UpdateInterval
                });
                Log.Debug("Notified");
            }
        }

        static void notifyConveyanceService(object sender, ElapsedEventArgs args)
        {
            if (ExecutionEngineClient != null)
            {
                var currentPos = _settings.Position;
                var currentSpeed = _settings.Speed;
                var currentInterval = _settings.UpdateInterval;
                var newPosition = currentPos + (currentSpeed * currentInterval / 1000);
                bool maxReached = false;
                if (newPosition > _settings.TotalLength)
                {
                    maxReached = true;
                    newPosition = newPosition - _settings.TotalLength;
                }
                _settings.Position = newPosition;
                ExecutionEngineClient.Invoke<ConveyanceProgressModel>("ConveyanceProgress", new ConveyanceProgressModel()
                {
                    Position = _settings.Position,
                    TotalLength = _settings.TotalLength,
                    Speed = _settings.Speed,
                    UpdateInterval = _settings.UpdateInterval,
                    MaxReached = maxReached
                });
            }
        }
    }
}
