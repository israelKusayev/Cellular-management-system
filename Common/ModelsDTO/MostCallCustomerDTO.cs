using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ModelsDTO
{
    public class MostCallCustomerDTO
    {
        public string IdentityCard { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CallsToCenter { get; set; }
    }
}
