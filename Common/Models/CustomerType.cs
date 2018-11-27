using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CustomerType
    {
        public CustomerType()
        {
            Customers = new List<Customer>();
        }
        public int CustomerTypeId { get; set; }

        public CustomerTypeEnum CustomerTypeEnum { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public double MinutePrice { get; set; }
        public double SmsPrice { get; set; }
    }
}
