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

namespace Db.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(DbContext context) : base(context)
        {

        }
        public virtual Customer GetActiveCustomerByIdCard(string idCard)
        {
            return CellularContext.CustomerTable.SingleOrDefault(c => c.IdentityCard == idCard && c.IsActive == true);
        }

        public Customer GetCustomerWithLinesAndPayments(string idCard)
        {
            return CellularContext.CustomerTable.Where(c => c.IdentityCard == idCard && c.IsActive == true).IncludeFilter(l => l.Lines.Where(v => v.Status == LineStatus.Used)).IncludeFilter(l => l.Lines.Where(v => v.Status == LineStatus.Used).Select(x => x.Payments)).SingleOrDefault();
        }

        public Customer GetActiveCustomerWithLines(string idCard)
        {
            return CellularContext.CustomerTable.Where(c => c.IdentityCard == idCard && c.IsActive == true).IncludeFilter(l => l.Lines.Where(x => x.Status == LineStatus.Used)).SingleOrDefault();
        }

        public Customer GetCustomerWithTypeAndLines(int customerId)
        {
            return CellularContext.CustomerTable.Where((c) => c.CustomerId == customerId).Include(t => t.CustomerType).Include(l => l.Lines).SingleOrDefault();
        }

        public Customer GetCustomerWithTypeLinesAndPayment(string idCard)
        {
            return CellularContext.CustomerTable.Where((c) => c.IdentityCard == idCard).Include((l) => l.Lines.Select(x => x.Payments)).Include(c => c.CustomerType).SingleOrDefault();
        }

        public Customer GetActiveCustomerWithLinesAndPackages(string idCard)
        {
            return CellularContext.CustomerTable.Where(c => c.IdentityCard == idCard && c.IsActive == true).IncludeFilter(l => l.Lines.Where(x => x.Status == LineStatus.Used)).IncludeFilter(l => l.Lines.Where(x => x.Status == LineStatus.Used).Select(p=>p.Package)).SingleOrDefault();
        }

        public List<Customer> GetMostCallToCenterCustomers(DateTime requestedTime)
        {
            int month = requestedTime.Month;
            int year = requestedTime.Year;
            return CellularContext.CustomerTable.Where(c => c.JoinDate.Value.Year == year && c.JoinDate.Value.Month == month).OrderByDescending(c => c.CallsToCenter).Take(10).ToList();
        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
