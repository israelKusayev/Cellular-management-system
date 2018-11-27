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

namespace Server.Controllers.CrmControllers
{
    public class EmployeeController : ApiController, IEmployeeLoginApi
    {
        IEmployeeManager _employeeManager;

        public EmployeeController(IEmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        [HttpPost]
        [Route("api/crm/login")]
        public IHttpActionResult Login(LoginDTO loginEmployee)
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
                registeredEmployee = _employeeManager.Login(loginEmployee);
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
    }
}
