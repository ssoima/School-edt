using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("orders")]
    public class ManufacturingOrder : Entity
    {
        [BsonElement("order")]
        public string OrderId { get; set; }
        [BsonElement("model")]
        public string Model { get; set; }
        [BsonElement("variant")]
        public string Variant { get; set; }
        [BsonElement("customer")]
        public string Customer { get; set; }
        [BsonElement("at_station")]
        public long? CurrentStationId { get; set; }
        [BsonElement("pos")]
        public double CurrentPosition { get; set; }
        [BsonElement("parts")]
        public ICollection<string> PartList { get; set; }
    }
}
