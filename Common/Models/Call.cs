using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Call
    {
        public int CallId { get; set; }

        public int LineId { get; set; }
        public Line Line { get; set; }

        public DateTime DateOfCall { get; set; }
        public int Duration { get; set; }
        public string DestinationNumber { get; set; }
    }
}
//nt result = line.Calls.GroupBy(x => x.DestinationNumber).Where(x => x.Count() == 1).Count();
