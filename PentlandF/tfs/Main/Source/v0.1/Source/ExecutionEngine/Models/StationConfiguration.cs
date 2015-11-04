using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("stations")]
    public class StationConfiguration : Entity
    {
        [BsonElement("sId")]
        public long StationId { get; set; }
        [BsonElement("name")]
        public string StationName { get; set; }
        [BsonElement("pos")]
        public int Position { get; set; }
        [BsonElement("offset")]
        public int ConveyanceOffset { get; set; }
        [BsonElement("len")]
        public int Length { get; set; }
    }
}
