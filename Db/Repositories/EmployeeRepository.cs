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
        //ctor
        public EmployeeRepository(DbContext context) : base(context)
        {

        }


        public Employee GetEmployeeByUserName(string userName)
        {
           return CellularContext.EmplyeesTable.SingleOrDefault((e) => e.UserName == userName);
        }

        /// <summary>
        /// Get the employees who joined the most customers in the requested month
        /// </summary>
        /// <param name="requestedDate"></param>
        /// <returns>List of employees if succeeded otherwise null</returns>
        public List<Employee> GetBestSellerEmployees(DateTime requestedDate)
        {
            List<Employee> allEmployees = CellularContext.EmplyeesTable
                                                         .IncludeFilter(c => c.Customers.Where(x=> x.JoinDate.Value.Year == requestedDate.Year && x.JoinDate.Value.Month == requestedDate.Month))
                                                         .ToList();

            List<Employee> employeesAfterOrderBy = allEmployees.OrderByDescending(c => c.Customers.Count)
                                                               .Take(10)
                                                               .ToList();

            List<Employee> employeesAfterClean = employeesAfterOrderBy.Where(c => c.Customers.Count != 0).ToList();

            return employeesAfterClean;
        }


        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
