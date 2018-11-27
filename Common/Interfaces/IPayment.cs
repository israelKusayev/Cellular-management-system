using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPayment
    {
        DateTime Date { get; set; }
        double LineTotalPrice { get; set; }
        double PackagePrice { get; set; }
        int PackageMinute { get; set; }
        int PackageSms { get; set; }
        int UsageCall { get; set; }
        int UsageSms { get; set; }
        int SmsBeyondPackageLimit { get; set; }
        CustomerType CustomerType { get; set; }
    }
}
