using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Models;
using Common.ModelsDTO;
using Newtonsoft.Json;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Script.Serialization;

namespace Server.Controllers
{
    public class CustomerWebsiteController : ApiController, ICustomerWebsiteApi
    {
        ICustomerWebsiteManager _customerWebsiteManager;

        //ctor
        public CustomerWebsiteController(ICustomerWebsiteManager customerWebsiteManager)
        {
            _customerWebsiteManager = customerWebsiteManager;
        }

        [HttpGet]
        [Route("api/customerWebsite/getCustomerLines/{idCard}")]
        public IHttpActionResult GetCustomerLines(string idCard)
        {
            Customer customer;
            try
            {
                customer = _customerWebsiteManager.GetCustomerWithLines(idCard);

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customer != null)
            {
                var response = JsonConvert.SerializeObject(customer, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
                return Ok(response);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to find the Customer"));
            }
        }

        [HttpGet]
        [Route("api/customerWebsite/getLineInfo/{linenumber}")]
        public IHttpActionResult GetLineDetails(string linenumber)
        {
            LineWebsiteDTO lineDetails;
            try
            {
                lineDetails = _customerWebsiteManager.GetLineDetails(linenumber);

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (lineDetails != null)
            {
                var response = JsonConvert.SerializeObject(lineDetails, Formatting.Indented,
                               new JsonSerializerSettings
                               {
                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                               });
                return Ok(response);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to find the line"));
            }
        }
    }
}
