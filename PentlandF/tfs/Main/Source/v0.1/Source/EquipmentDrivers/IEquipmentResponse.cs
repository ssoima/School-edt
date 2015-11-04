using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.EquipmentDrivers
{
    public interface IEquipmentResponse
    {
        IEquipment Source { get; }
        string Message { get; }
        bool Success { get; }
    }
}
