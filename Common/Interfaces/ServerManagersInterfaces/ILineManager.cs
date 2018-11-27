using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.ServerManagersInterfaces
{
    public interface ILineManager
    {
        List<Line> GetCustomerLines(string idCard);
        Line AddNewLine(Line lineToAdd, int customerId);
        Line DeactivateLine(int lineId);
    }
}
