using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PbLTest
{
    public class PblMessage
    {
        public int id { get; set; }
        public int sid { get; set; }
        public PblCmd cmd { get; set; }
        public PblColor color { get; set; }
        public string value { get; set; }
    }

    public enum PblCmd
    {
        Clear = 0,
        Fill = 1,
        Arrow = 2,
        Cross = 3,
        Text = 4,
        Smiley = 5,
        Check = 6,

        Button = 20
    }

    public enum PblColor
    {
        Green = 3,
        Yellow = 2,
        Red = 1
    }


}
