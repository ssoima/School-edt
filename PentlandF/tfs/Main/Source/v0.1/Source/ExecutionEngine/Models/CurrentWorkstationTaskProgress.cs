using System.Security.AccessControl;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("currentTaskProgress")]
    public class CurrentWorkstationTaskProgress : Entity
    {
        [BsonElement("oId")]
        public string OrderId { get; set; }
        [BsonElement("wId")]
        public long WorkstationId { get; set; }
        [BsonElement("sId")]
        public long StationId { get; set; }
        [BsonElement("complete")]
        public bool Complete { get; set; }
        [BsonElement("progress")]
        public ICollection<EquipmentTaskProgress> TaskProgress { get; set; }
    }
}
