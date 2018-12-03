using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Models;
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
    public class BiController : ApiController, IBiApi
    {
        IBiManager _biManager;

        public BiController(IBiManager biManager)
        {
            _biManager = biManager;
        }

        [HttpGet]
        [Route("api/bi/login")]
        public IHttpActionResult LoginManager(LoginDTO loginEmployee)
        {
            if (loginEmployee == null
                 || string.IsNullOrWhiteSpace(loginEmployee.UserName)
                 || string.IsNullOrWhiteSpace(loginEmployee.Password))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing, please make sure your username and password are filled out"));
            }

            Employee registeredEmployee;
            try
            {
                registeredEmployee = _biManager.Login(loginEmployee);
            }
            catch (IncorrectExeption e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, e.Message));
            }
            catch (FaildToConnectDbExeption e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something went wrong"));
            }

            if (registeredEmployee != null)
            {
                return Ok(registeredEmployee);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something went wrong"));
            }
        }

        [HttpGet]
        [Route("api/bi/bestSellerEmployees")]
        public IHttpActionResult GetBestSellerEmployees()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/bi/GroupsOfFreindsWhoTalkEachOther")]
        public IHttpActionResult GetGroupsOfFreindsWhoTalkEachOther()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/bi/linesAtRiskOfAbandonment")]
        public IHttpActionResult GetLinesAtRiskOfAbandonment()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/bi/mostCallingToCenterCustomers")]
        public IHttpActionResult GetMostCallingToCenterCustomers()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/bi/mostprofitablecustomers")]
        public IHttpActionResult GetMostprofitablecustomers()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("api/bi/opinionLeadersCustomers")]
        public IHttpActionResult GetOpinionLeadersCustomers()
        {
            throw new NotImplementedException();
        }


    }
}
