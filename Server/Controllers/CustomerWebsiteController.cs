using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class CustomerWebsiteController : ApiController
    {
        [HttpGet]
        [Route("api/customerWebsite/getData/{idCard}")]
        public IHttpActionResult GetData(string idCard)
        {
            return Ok(1);
        }
    }
}
