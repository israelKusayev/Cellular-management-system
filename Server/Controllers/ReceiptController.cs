using Common.Interfaces.ServerManagersInterfaces;
using Common.ModelsDTO;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class ReceiptController : ApiController, IReceiptApi
    {
        IReceiptManager _receiptManager;

        //ctor
        public ReceiptController(IReceiptManager receiptManager)
        {
            _receiptManager = receiptManager;
        }

        [HttpGet]
        [Route("api/receipt/generatePayments")]
        public IHttpActionResult GeneratePaymentToAllLines()
        {
            if (_receiptManager.GeneratePaymentsToAllLines(DateTime.Now) != null)
            {
                return Ok();
            }
            return BadRequest();
        }

        //------------------//

        [HttpGet]
        [Route("api/receipt/{idCard}/{year}/{month}")]
        public IHttpActionResult GetCustomerReceipt(string idCard, int year, int month)
        {
            List<LineReceiptDTO> lineReceipts;
            DateTime date = new DateTime(year, month, 1);
            try
            {
                lineReceipts = _receiptManager.GetCustomerReceipt(idCard, date);

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (lineReceipts != null && lineReceipts.Count != 0)
            {
                return Ok(lineReceipts);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate receipts for the requested customer"));
            }
        }
    }
}
