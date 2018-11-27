using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Line
    {
        public Line()
        {
            Calls = new List<Call>();
            Messages = new List<Sms>();
            Payments = new List<Payment>();
        }
        public int LineId { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Package Package { get; set; }

        public ICollection<Call> Calls { get; set; }
        public ICollection<Sms> Messages { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public string LineNumber { get; set; }
        public LineStatus Status { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreatedDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? RemovedDate { get; set; }
    }
}
