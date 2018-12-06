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

        //ctor
        public BiController(IBiManager biManager)
        {
            _biManager = biManager;
        }

        [HttpPost]
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
            List<EmployeeBiDTO> employees;
            try
            {
                employees = _biManager.GetBestSellerEmployees();

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (employees != null)
            {
                return Ok(employees);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate best seller employees report"));
            }
        }

        [HttpGet]
        [Route("api/bi/linesAtRiskOfAbandonment")]
        public IHttpActionResult GetCustomersAtRiskOfAbandonment()
        {
            List<CustomerBiDTO> customers;
            try
            {
                customers = _biManager.GetCustomersAtRiskOfAbandonment();

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customers != null)
            {
                return Ok(customers);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate customers at risk of abandonment report"));
            }
        }

        [HttpGet]
        [Route("api/bi/mostCallingToCenterCustomers")] 
        public IHttpActionResult GetMostCallingToCenterCustomers()
        {
            List<MostCallCustomerDTO> customers;
            try
            {
                customers = _biManager.GetMostCallingToCenterCustomers();

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customers != null)
            {
                return Ok(customers);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate most calling to center customer report"));
            }
        }

        [HttpGet]
        [Route("api/bi/mostProfitableCustomers")] 
        public IHttpActionResult GetMostProfitableCustomers()
        {
            List<ProfitableCustomerDTO> customers;
            try
            {
                customers = _biManager.GetMostProfitableCustomers();

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customers != null)
            {
                return Ok(customers);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate most profitable customers report"));
            }
        }

        [HttpGet]
        [Route("api/bi/opinionLeadersCustomers")] 
        public IHttpActionResult GetOpinionLeadersCustomers()
        {
            List<CustomerBiDTO> customers;

            try
            {
                customers = _biManager.GetOpinionLeadersCustomers();
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customers != null)
            {
                return Ok(customers);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate most calling to center customer report"));
            }
        }

        [HttpGet]
        [Route("api/bi/GroupsOfFreindsWhoTalkEachOther")]
        public IHttpActionResult GetGroupsOfFreindsWhoTalkEachOther()
        {
            List<GroupDTO> groups;

            try
            {
                groups = _biManager.GetGroupsOfFreindsWhoTalkEachOther();
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (groups != null)
            {
                return Ok(groups);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we were unable to generate optional groups report"));
            }
        }


    }
}
