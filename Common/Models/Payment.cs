using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Payment : IPayment
    {
        public int PaymentId { get; set; }

        public int LineId { get; set; }
        public Line Line { get; set; }

        public double LineTotalPrice { get ; set ; }
        public DateTime Date { get ; set ; }
        public double PackagePrice { get ; set ; }
        public int PackageMinute { get ; set ; }
        public int PackageSms { get ; set ; }
        public int UsageCall { get ; set ; }
        public int UsageSms { get ; set ; }
        public int MinutesBeyondPackageLimit { get; set; }
        public int SmsBeyondPackageLimit { get ; set; }
        public CustomerType CustomerType { get ; set ; }
    }

}
