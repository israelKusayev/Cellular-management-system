using Common.Models;
using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RepositoryInterfaces
{
    interface ILoginEmployeeRepository
    {
        Employee Login(LoginDTO loginEmployee);
    }
}
