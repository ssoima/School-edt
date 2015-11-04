using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("workstations")]
    public class WorkstationConfiguration : Entity
    {
        [BsonElement("wId")]
        public long WorkstationId { get; set; }
        [BsonElement("name")]
        public string WorkstationName { get; set; }
        [BsonElement("sId")]
        public long StationId { get; set; }
    }
}
