using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Models;
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
    //[EnableCors(origins: "file:///C:/Users/Home/Downloads/index.html", headers: "*", methods: "*")]
    public class CustomerWebsiteController : ApiController, ICustomerWebsiteApi
    {
        ICustomerWebsiteManager _websiteManager;
        public CustomerWebsiteController(ICustomerWebsiteManager websiteManager)
        {
            _websiteManager = websiteManager;
        }

        [HttpGet]
        [Route("api/customerWebsite/getLines/{idCard}")]
        public IHttpActionResult GetData(string idCard)
        {
            Customer customer;
            try
            {
                customer = _websiteManager.GetCustomerLines(idCard);
            }
            catch (NotFoundException e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }
            var response = JsonConvert.SerializeObject(customer, Formatting.Indented,
                                new JsonSerializerSettings
                                {
                                      ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });
            return Ok(response);
        }
    }
}
