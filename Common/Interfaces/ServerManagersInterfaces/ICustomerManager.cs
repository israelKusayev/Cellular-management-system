using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.ServerManagersInterfaces
{
    public interface ICustomerManager
    {
        Customer GetActiveCustomer(string idCard);
        double GetCustomerValue(string idCard);
        Customer AddNewCustomer(Customer newCustomer);
        Customer EditCustomer(Customer customerToEdit);
        Customer DeactivateCustomer(string idCard);
    }
}
