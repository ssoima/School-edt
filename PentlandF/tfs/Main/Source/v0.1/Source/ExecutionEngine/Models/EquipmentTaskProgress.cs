using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    //[CollectionName("equipmentProgress")]
    public class EquipmentTaskProgress  // : Entity // for now leave it as a pure nested document
    {
        [BsonElement("id")]
        public long StationTaskAssignmentId { get; set; }
        [BsonElement("wsId")]
        public long WorkstationId { get; set; }
        [BsonElement("preId")]
        public long? PredecessorId { get; set; }
        [BsonElement("tId")]
        public long TaskId { get; set; }
        [BsonElement("t")]
        public string Task { get; set; }
        [BsonElement("pId")]
        public long PartId { get; set; }
        [BsonElement("p_no")]
        public string PartNumber { get; set; }
        [BsonElement("p")]
        public string PartName { get; set; }
        [BsonElement("img")]
        public string TaskImageUrl { get; set; }
        [BsonElement("comment")]
        public string Comment { get; set; }
        [BsonElement("progress")]
        public bool InProgress { get; set; }
        [BsonElement("complete")]
        public bool Completed { get; set; }
        [BsonElement("eq_req")]
        public bool IsEquipmentRequired { get; set; }
        [BsonElement("eq")]
        public EquipmentConfiguration Equipment { get; set; }
        [BsonElement("drv_conf")]
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, string> Configuration { get; set; }
        [BsonElement("ack_res")]
        public string AckResult { get; set; }
        [BsonElement("io")]
        public bool Success { get; set; }
        [BsonElement("time")]
        public int Time { get; set; } // in milliseconds
    }
}
