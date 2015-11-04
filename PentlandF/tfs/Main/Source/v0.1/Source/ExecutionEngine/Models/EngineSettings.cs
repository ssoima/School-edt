using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace NextLAP.IP1.ExecutionEngine.Models
{
    [CollectionName("engineSettings")]
    public class EngineSettings : Entity
    {
        [BsonElement("assignmentVersion")]
        public int CurrentAssignmentVersion { get; set; }
        [BsonElement("lastConveyancePosition")]
        public double LastConveyancePosition { get; set; }
    }
}
