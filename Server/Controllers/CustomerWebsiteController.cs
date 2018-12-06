﻿using Common.Interfaces.ServerManagersInterfaces;
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
        [Route("api/customerWebsite/getPackageRecommendation/{lineId}")]
        public IHttpActionResult GetLineDetails(int lineId)
        {
            LineWebsiteDTO lineDetails;
            try
            {
                lineDetails = _customerWebsiteManager.GetLineDetails(lineId);

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (lineDetails != null)
            {
                return Ok(lineDetails);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to find the line"));
            }
        }

        [HttpGet]
        [Route("api/customerWebsite/getPackageRecommendation/{lineId}")]
        public IHttpActionResult GetPackageRecommendation(int lineId)
        {
            return Ok(1);
        }
    }
}
