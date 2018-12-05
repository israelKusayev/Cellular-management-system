using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RepositoryInterfaces
{
    public interface IEmployeeRepository:IRepository<Employee>
    {
        Employee GetEmployeeByUserName(string userName);
        List<Employee> GetBestSellerEmployees(DateTime requestedDate);
    }
}
