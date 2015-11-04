using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NextLAP.IP1.Common.Diagnostics;
using NextLAP.IP1.EquipmentDrivers;

namespace NextLAP.IP1.ExecutionEngine.Equipment
{
    internal class EquipmentRunner : IDisposable
    {
        private static readonly Logger Log = LogManager.GetLogger(typeof(EquipmentRunner));
        private Thread _runner;
        private IEquipment _equipment;
        private BlockingCollection<EquipmentRunnable> _commands;
        private EquipmentCoordinator _coordinator;

        public EquipmentRunner(EquipmentCoordinator coordinator, IEquipment equipment)
        {
            _coordinator = coordinator;
            _commands = new BlockingCollection<EquipmentRunnable>();
            _equipment = equipment;
            _equipment.Receive += EquipmentOnReceive;
            _runner = new Thread(Run);
            _runner.Start();
        }

        private void EquipmentOnReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            throw new NotImplementedException();
        }

        public void Add(EquipmentRunnable command)
        {
            _commands.Add(command);
        }

        public void Stop()
        {
            Log.Debug("Stop called");
            _commands.CompleteAdding();
        }

        private void Run()
        {
            try
            {
                foreach (var runnable in _commands.GetConsumingEnumerable())
                {
                    //Log.Debug("Sending command to " + _equipment.Address + " => '" + runnable.Command + "'..");
                    //var response = _equipment.Send(runnable.TaskProgress.co);
                    //Log.Debug("Received response '" + response + "'.");
                    //OnEquipmentResponded(response, runnable);
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        //private void OnEquipmentResponded(string response, EquipmentRunnable runnable)
        //{
        //    _coordinator.NotifyEquipmentResponded(this, response, runnable);
        //}

        public void Dispose()
        {
            Log.Debug("Dispose called.");
            Stop();
            _runner.Abort();
            _runner = null;
            _commands.Dispose();
            _equipment.Dispose();
        }
    }
}
