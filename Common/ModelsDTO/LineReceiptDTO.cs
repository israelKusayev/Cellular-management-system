using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ModelsDTO
{
    public class LineReceiptDTO : IPayment
    {
        public LineReceiptDTO(IPayment payment)
        {
            LineTotalPrice = payment.LineTotalPrice;
            Date = payment.Date;
            PackagePrice = payment.PackagePrice;
            PackageMinute = payment.PackageMinute;
            PackageSms = payment.PackageSms;
            UsageCall = payment.UsageCall;
            UsageSms = payment.UsageSms;
            CustomerType = payment.CustomerType;
            SmsBeyondPackageLimit = payment.SmsBeyondPackageLimit;
        }
        public LineReceiptDTO()
        {

        }
        public string CustomerName { get; set; }
        public string LineNumber { get; set; }

        public TimeSpan LeftMinutes { get; set; }
        public int LeftSms { get; set; }

        public int MinutesUsagePrecent { get; set; }
        public int SmsUsagePrecent { get; set; }

        public double PricePerMinute { get; set; }
        public double PricePerSms { get; set; }

        public double ExceptionalMinutesPrice { get; set; }
        public double ExceptionalSmsPrice { get; set; }


        public CustomerType CustomerType { get; set; }
        public DateTime Date { get; set; }
        public double LineTotalPrice { get; set; }
        public double PackagePrice { get; set; }

        public TimeSpan MinutesBeyondPackageLimit { get; set; }
        public int SmsBeyondPackageLimit { get; set; }

        public int PackageMinute { get; set; }
        public int PackageSms { get; set; }

        public int UsageCall { get; set; }
        public int UsageSms { get; set; }
    }
}
