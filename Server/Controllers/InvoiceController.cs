using Common.ModelsDTO;
using Invoice.Dal.Dals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class InvoiceController : ApiController
    {
        InvoiceDal _invoiceManager;
        public InvoiceController()
        {
            _invoiceManager = new InvoiceDal();

        }

        [HttpGet]
        [Route("api/invoice/{idCard}/{date}")]
        //[Route("api/invoice/get")]
        IHttpActionResult GetCustomerInvoice(string idCard,DateTime date)
        {
            List<LineInvoiceDTO> invoice = _invoiceManager.GetCustomerInvoice(idCard, date);
            return Ok();
        }
    }
}
