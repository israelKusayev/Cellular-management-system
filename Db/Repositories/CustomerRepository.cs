using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Z.EntityFramework.Plus;
using Common.ModelsDTO;

namespace Db.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        //ctor
        public CustomerRepository(DbContext context) : base(context)
        {

        }

        public virtual Customer GetActiveCustomerByIdCard(string idCard)
        {
            return CellularContext.CustomerTable
                                  .SingleOrDefault(c => c.IdentityCard == idCard && c.IsActive == true);
        }

        public Customer GetCustomerWithLinesAndPayments(string idCard)
        {
            return CellularContext.CustomerTable
                                  .Where(c => c.IdentityCard == idCard && c.IsActive == true)
                                  .IncludeFilter(l => l.Lines.Where(v => v.Status == LineStatus.Used))
                                  .IncludeFilter(l => l.Lines.Where(v => v.Status == LineStatus.Used)
                                  .Select(x => x.Payments))
                                  .SingleOrDefault();
        }

        public Customer GetActiveCustomerWithLines(string idCard)
        {
            return CellularContext.CustomerTable
                                  .Where(c => c.IdentityCard == idCard && c.IsActive == true)
                                  .IncludeFilter(l => l.Lines.Where(x => x.Status == LineStatus.Used))
                                  .SingleOrDefault();
        }

        public Customer GetCustomerWithTypeAndLines(int customerId)
        {
            return CellularContext.CustomerTable
                                  .Where((c) => c.CustomerId == customerId)
                                  .Include(t => t.CustomerType)
                                  .Include(l => l.Lines)
                                  .SingleOrDefault();
        }

        public Customer GetCustomerWithTypeLinesAndPayment(string idCard)
        {
            return CellularContext.CustomerTable
                                  .Where((c) => c.IdentityCard == idCard)
                                  .Include((l) => l.Lines.Select(x => x.Payments))
                                  .Include(c => c.CustomerType)
                                  .SingleOrDefault();
        }

        public Customer GetActiveCustomerWithLinesAndPackages(string idCard)
        {
            return CellularContext.CustomerTable
                                  .Where(c => c.IdentityCard == idCard && c.IsActive == true)
                                  .IncludeFilter(l => l.Lines.Where(x => x.Status == LineStatus.Used))
                                  .IncludeFilter(l => l.Lines.Where(x => x.Status == LineStatus.Used).Select(p => p.Package))
                                  .SingleOrDefault();
        }


        public List<Customer> GetMostCallToCenterCustomers(DateTime requestedTime)
        {
            int month = requestedTime.Month;
            int year = requestedTime.Year;
            return CellularContext.CustomerTable
                                  .Where(c => c.JoinDate.Value.Year == year && c.JoinDate.Value.Month == month && c.CallsToCenter != 0)
                                  .OrderByDescending(c => c.CallsToCenter)
                                  .Take(10)
                                  .ToList();
        }

        public List<Customer> GetOpinionLeadersCustomers()
        {
            return CellularContext.CustomerTable
                                  .OrderByDescending(c => c.Lines.SelectMany(l => l.Calls)
                                  .Select(x => x.DestinationNumber)
                                  .Distinct()
                                  .Count())
                                  .Take(10)
                                  .ToList();
        }

        public List<Customer> GetMostProfitableCustomers()
        {
            List<Customer> customers = CellularContext.CustomerTable
                                                      .Include(l => l.Lines)
                                                      .Include(l => l.Lines.Select(x => x.Payments))
                                                      .ToList();

            List<Customer> mostProfitableCustomers = customers
                                                    .OrderByDescending(x => x.Lines.Sum(q => q.Payments.Sum(w => w.LineTotalPrice)) / x.Lines.Sum(a => a.Payments.Count))
                                                    .Take(10)
                                                    .ToList();

            return mostProfitableCustomers;
        }

        public List<Customer> GetCustomersAtRiskOfAbandonment()
        {

            var lines = CellularContext.LinesTable
                                       .IncludeFilter(l => l.Calls.OrderByDescending(s => s.DestinationNumber)
                                       .Distinct().Take(5))
                                       .ToList();

            List<Customer> customers = new List<Customer>();
            foreach (var line in lines)
            {
                int count = 0;
                foreach (var call in line.Calls)
                {
                    if (CellularContext.LinesTable.Where(x => x.LineNumber == call.DestinationNumber && x.Status == LineStatus.Removed).Any())
                    {
                        count++;
                    }
                }
                if (count >= 2)
                {
                    customers.Add(Get(line.CustomerId));
                    if (customers.Count == 10)
                    {
                        return customers;
                    }
                }
            }
            return customers;
        }

        public List<Customer> GetActiveCustomersWithLinesAndCalls()
        {
            return CellularContext.CustomerTable.Where(c=>c.IsActive == true).Include(c => c.Lines).Include(c => c.Lines.Select(x => x.Calls)).ToList();
        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
