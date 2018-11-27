using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Employee
    {
        public Employee()
        {
            Customers = new List<Customer>();
        }
        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}
