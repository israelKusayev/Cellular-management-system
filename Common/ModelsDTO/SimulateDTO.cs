using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ModelsDTO
{
    public class SimulateDTO
    {
        public string IdentityCard { get; set; }
        public int LineId { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
        public int NumberOfCallsOrSms { get; set; }
        public bool IsSms { get; set; }
        public int CallToCenter { get; set; }
        public SimulateSendTo SendTo { get; set; }
    }
}
