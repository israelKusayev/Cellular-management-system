using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ModelsDTO
{
    public class MostCalledNumberCustomerDTO
    {
        public Customer Customer { get; set; }
        public string FirstNumber { get; set; }
        public string SecondNumber { get; set; }
        public string ThirdNumber { get; set; }
    }
}
