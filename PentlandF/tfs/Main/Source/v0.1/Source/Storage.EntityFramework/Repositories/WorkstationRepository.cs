using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NextLAP.IP1.Models.Equipment;
using NextLAP.IP1.Models.Structure;

namespace NextLAP.IP1.Storage.EntityFramework.Repositories
{
    public class WorkstationRepository : Ip1BaseRepository<Workstation>
    {
        public WorkstationRepository(Ip1Context context) : base(context)
        {
        }

        public Workstation CreateDefaultWorkstation(Station station, int? number)
        {
            var newWorker = Create();
            newWorker.Name = station.Name + " - WS " + (number != null ? (number.Value + 1).ToString() : string.Empty);
            newWorker.Position = number != null ? number.Value + 1 : 0;
            newWorker.Type = station.StationType != null ? station.StationType.Name : string.Empty;
            newWorker.Side = number.GetValueOrDefault(0) % 2 == 0 ? "left" : "right";
            newWorker.Station = station;

            // plus create default workstation info terminal
            newWorker.WorkStationTerminal = new WorkstationTerminal()
            {
                ConnectionIdentifier = Guid.NewGuid()
            };
            return newWorker;
        }
    }
}
