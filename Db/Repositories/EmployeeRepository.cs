using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
