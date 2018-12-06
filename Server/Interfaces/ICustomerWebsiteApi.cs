using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    public interface ICustomerWebsiteApi
    {
        IHttpActionResult GetCustomerLines(string idCard);
        IHttpActionResult GetLineDetails(string lineNumber);
    }
}
