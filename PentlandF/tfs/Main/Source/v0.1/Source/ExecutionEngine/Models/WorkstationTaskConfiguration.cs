using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using MongoRepository;
using NextLAP.IP1.Common;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("workstationConfiguration")]
    public class WorkstationTaskConfiguration : Entity
    {
        [BsonElement("id")]
        public long StationTaskAssignmentId { get; set; }
        [BsonElement("sId")]
        public long StationId { get; set; }
        [BsonElement("s")]
        public string Station { get; set; }
        [BsonElement("wsId")]
        public long WorkstationId { get; set; }
        [BsonElement("ws")]
        public string Workstation { get; set; }
        [BsonElement("preId")]
        public long? PredecessorId { get; set; }
        [BsonElement("pId")]
        public long PartId { get; set; }
        [BsonElement("p_no")]
        public string PartNumber { get; set; }
        [BsonElement("p")]
        public string PartName { get; set; }
        [BsonElement("tId")]
        public long TaskId { get; set; }
        [BsonElement("t")]
        public string Task { get; set; }
        [BsonElement("img")]
        public string TaskImageUrl { get; set; }
        [BsonElement("show")]
        public bool ShowInTerminal { get; set; }
        [BsonElement("equip_req")]
        public bool IsEquipmentRequired { get; set; }
        [BsonElement("equipId")]
        public long? EquipmentId { get; set; }
        [BsonElement("drv_conf")]
        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, string> DriverConfiguration { get; set; }
    }
}
