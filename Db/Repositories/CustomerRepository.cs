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
        public CustomerRepository(CellularContext context) : base(context)
        {

        }
        public Customer GetActiveCustomerByIdCard(string idCard)
        {
            return CellularContext.CustomerTable.SingleOrDefault(c => c.IdentityCard == idCard && c.IsActive == true);
        }

        public Customer GetCustomerWithLinesAndPayments(string idCard)
        {
            return CellularContext.CustomerTable.Where(c => c.IdentityCard == idCard && c.IsActive == true).IncludeFilter(l => l.Lines.Where(v => v.Status == LineStatus.Used).Select(x => x.Payments)).SingleOrDefault();
        }

        public Customer GetActiveCustomerWithLines(string idCard)
        {
            return CellularContext.CustomerTable.Where(c => c.IdentityCard == idCard && c.IsActive == true).IncludeFilter(l => l.Lines.Where(x=>x.Status == LineStatus.Used)).SingleOrDefault();
        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
