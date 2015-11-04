using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.EquipmentDrivers.Shelf;
using NextLAP.IP1.EquipmentDrivers;

namespace NextLAP.IP1.ExecutionEngine.Equipment
{
    internal class EquipmentDriverFactory
    {
        public static IEquipment GetEquipment(string driver)
        {
            var type = Type.GetType(driver, false);
            if (type == null)
                throw new InvalidOperationException("The driver '" + driver + "' is not available.");
            var impl = Activator.CreateInstance(type) as IEquipment;
            if (impl == null) throw new InvalidOperationException("Creating an instance of driver '" + driver + "' failed.");
            return impl;
        }
    }
}
