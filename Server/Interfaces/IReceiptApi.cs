using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    public interface IReceiptApi
    {
        IHttpActionResult GeneratePaymentToAllLines();
        IHttpActionResult GetCustomerReceipt(string idCard, int year, int month);
    }
}
