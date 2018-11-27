using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Sms
    {
        public int SmsId { get; set; }

        public int LineId { get; set; }
        public Line Line { get; set; }

        public DateTime DataOfMessage { get; set; }
        public string DestinationNumber { get; set; }
    }
}
