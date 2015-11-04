using NextLAP.IP1.ExecutionEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.ExecutionEngine
{
    public class NewOrdersInStationEventArgs : EventArgs
    {
        public NewOrdersInStationEventArgs(IEnumerable<ManufacturingOrder> orders)
        {
            Orders = orders;
        }

        public IEnumerable<ManufacturingOrder> Orders { get; private set; }
    }

    public class ConveyanceProgressChangedEventArgs : EventArgs
    {
        public ConveyanceProgressChangedEventArgs(ConveyanceProgressModel model)
        {
            ConveyanceProgress = model;
        }

        public ConveyanceProgressModel ConveyanceProgress { get; private set; }
    }

    public class EquipmentTaskProgressChangedEventArgs : EventArgs
    {
        public EquipmentTaskProgressChangedEventArgs(EquipmentTaskProgress progress)
        {
            Progress = progress;
        }
        public EquipmentTaskProgress Progress { get; private set; }
    }
}
