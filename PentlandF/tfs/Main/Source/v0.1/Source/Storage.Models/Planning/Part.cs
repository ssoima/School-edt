using NextLAP.IP1.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NextLAP.IP1.Models.Planning
{
    /// <summary>
    /// Parts (== Teile) describe an object which can be built into a car. Parts can be different things. A Part can be either something which is 
    /// refered to as a material (== Material) or can be an assembled part that consists of a Parts list.
    /// 
    /// </summary>
    public class Part : ExtendedTimedEntity
    {
        /// <summary>
        /// The part number. You can use it as a complete part number (ie. 8K0200002) or as a part of the composite
        /// where the complete part number is based on the concatenation of its parent's Number property (ie. 002) and parent has 200 and that parent has 8K0.
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// A long name which is adding more info than just the Name property which might be an cryptic name
        /// </summary>
        public string LongName { get; set; }
        /// <summary>
        /// Can be
        /// - Component (ie. Center Console)
        /// - Part (ie. Gear stick)
        /// - Material (ie. Audio cable)
        /// - Colored Part (ie. Comfort Seats gray)
        /// </summary>
        public string PartType { get; set; }
        /// <summary>
        /// Part group could be anything (ie. Engine, Door, Seat, Gear)
        /// </summary>
        public string PartGroup { get; set; }
        public virtual Part ParentPart { get; set; }
        public long? ParentPartId { get; set; }
        /// <summary>
        /// A list of parts this part consists of.
        /// </summary>
        public virtual ICollection<Part> PartsList { get; set; }

        public virtual ICollection<TaskAssignment> AssignedTasks { get; set; }
    }
}