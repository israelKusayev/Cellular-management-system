using Common.Models;
using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.ServerManagersInterfaces
{
    public interface IBiManager
    {
        Employee Login(LoginDTO loginEmployee);
        List<MostCallCustomerDTO> GetMostCallingToCenterCustomers();
        List<EmployeeBiDTO> GetBestSellerEmployees();
        List<ProfitableCustomerDTO> GetMostProfitableCustomers();
    }
}
