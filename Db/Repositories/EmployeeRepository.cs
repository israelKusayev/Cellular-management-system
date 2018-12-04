using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Db.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(DbContext context) : base(context)
        {

        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }

        public Employee GetEmployeeByUserName(string userName)
        {
           return CellularContext.EmplyeesTable.SingleOrDefault((e) => e.UserName == userName);
        }

        public List<Employee> GetBestSellerEmployees(DateTime requestedDate)
        {
            List<Employee> e = CellularContext.EmplyeesTable.OrderByDescending(c => c.Customers.Where(x => x.JoinDate.Value.Year == requestedDate.Year && x.JoinDate.Value.Month == requestedDate.Month).Count()).ToList();
            return e;
        }
    }
}
