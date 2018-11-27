using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    interface ICustomerApi
    {
        IHttpActionResult GetCustomer(string idCard);
        IHttpActionResult AddNewCustomer(Customer newCustomer);
        IHttpActionResult EditCustomerDetails(Customer customerToEdit);
        IHttpActionResult DeactivateCustomer(string idCard);
    }
}
