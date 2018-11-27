using Common.ModelsDTO;
using Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class ReceiptController : ApiController
    {
        ReceiptManager _receiptManager;
        public ReceiptController()
        {
            _receiptManager = new ReceiptManager();

        }

        [HttpGet]
        [Route("api/receipt/generatePayments")]
        public IHttpActionResult GeneratePaymentToAllLines()
        {
            _receiptManager.GeneratePaymentsToAllLines(DateTime.Now);
            return null;
        }

        //------------------//

        [HttpGet]
        [Route("api/receipt/{idCard}/{year}/{month}")]
        public IHttpActionResult GetCustomerReceipt(string idCard, int year, int month)
        {
            List<LineReceiptDTO> _lineReceipts;
            DateTime date = new DateTime(year, month, 1);
            try
            {
                _lineReceipts = _receiptManager.GetCustomerReceipt(idCard, date);

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (_lineReceipts != null)
            {
                return Ok(_lineReceipts);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate receipts for the requested customer"));

            }
        }


    }
}
