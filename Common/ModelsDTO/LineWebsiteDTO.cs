using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ModelsDTO
{
    public class LineWebsiteDTO
    {
        public double TotalLinePrice { get; set; }
        public int TotalMinutes { get; set; }
        public int TotalSms { get; set; }
        public int TotalMinutesTopNumber { get; set; }
        public int TotalMinutesTop3Numbers { get; set; }
        public int TotalMinutesWithFamily { get; set; }
        public List<Package> RecommendPackages { get; set; }
    }
}
