using Common.Models;
using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.ServerManagersInterfaces
{
    public interface ICustomerWebsiteManager
    {
        Customer GetCustomerWithLines(string idCard);
        LineWebsiteDTO GetLineDetails(string lineNumber);
    }
}
