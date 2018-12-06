using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RepositoryInterfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer GetActiveCustomerByIdCard(string idCard);
        Customer GetCustomerWithLinesAndPayments(string idCard);
        Customer GetActiveCustomerWithLines(string idCard);
        Customer GetActiveCustomerWithLinesAndPackages(string idCard);
        Customer GetCustomerWithTypeAndLines(int customerId);
        Customer GetCustomerWithTypeLinesAndPayment(string idCard);
        List<Customer> GetActiveCustomersWithLinesAndCalls();

        List<Customer> GetMostCallToCenterCustomers(DateTime requestedTime);
        List<Customer> GetOpinionLeadersCustomers();
        List<Customer> GetMostProfitableCustomers();
        List<Customer> GetCustomersAtRiskOfAbandonment();
    }
}
