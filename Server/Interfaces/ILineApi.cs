using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    interface ILineApi
    {
        IHttpActionResult GetCustomerLines(string idCard);
        IHttpActionResult AddNewLine(int customerId, Line lineToAdd);
        IHttpActionResult DeactivateLine(int lineId);
    }
}
