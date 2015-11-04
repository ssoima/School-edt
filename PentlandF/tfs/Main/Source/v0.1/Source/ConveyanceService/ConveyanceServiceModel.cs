using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace NextLAP.IP1.ConveyanceService
{
    [CollectionName("conveyanceSettings")]
    public class ConveyanceServiceModel : Entity
    {
        [BsonElement("pos")]
        public double Position { get; set; }
        [BsonElement("len")]
        public double TotalLength { get; set; }
        [BsonElement("interval")]
        public double UpdateInterval { get; set; }
        [BsonElement("speed")]
        public double Speed { get; set; }
        [BsonElement("status")]
        public ConveyanceStatus Status { get; set; }
    }

    public enum ConveyanceStatus
    {
        Stopped,
        Started
    }
}
