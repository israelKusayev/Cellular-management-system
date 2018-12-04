using Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Customer
    {
        public Customer()
        {
            Lines = new List<Line>();
        }
        public int CustomerId { get; set; }

        public int CustomerTypeId { get; set; }
        public CustomerType CustomerType { get; set; }

        public string IdentityCard { get; set; }

        public int EmplyeeId { get; set; }
        public Employee Employee { get; set; }

        public ICollection<Line> Lines { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }
        public int CallsToCenter { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinDate { get; set; }

    }
}
