using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Package
    {
        public Package()
        {
            Lines = new List<Line>();
        }
        public int PackageId { get; set; }

        public Friends Friends { get; set; }

        public ICollection<Line> Lines { get; set; }

        public string PackageName { get; set; }
        public double TotalPrice { get; set; }
        public int MaxMinute { get; set; }
        public int MaxSms { get; set; }
        public bool PriorityContact { get; set; }
        public bool InsideFamilyCalles { get; set; }
        public bool IsPackageTemplate { get; set; }
    }
}
