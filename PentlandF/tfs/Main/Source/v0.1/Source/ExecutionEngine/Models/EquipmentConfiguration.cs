using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("equipmentConfiguration")]
    public class EquipmentConfiguration : Entity
    {
        [BsonElement("id")]
        public long EquipmentId { get; set; }
        [BsonElement("type")]
        public string Type { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("config")]
        public string Configuration { get; set; }
        [BsonElement("ip")]
        public string IpAddress { get; set; }
        [BsonElement("drv")]
        public string Driver { get; set; }
    }
}
