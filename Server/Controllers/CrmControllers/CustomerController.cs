using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Models;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers.CrmControllers
{
    public class CustomerController : ApiController, ICustomerApi
    {
        private ICustomerManager _customerManager;

        //ctor
        public CustomerController(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        [HttpGet]
        [Route("api/crm/customer/{idCard}")]
        public IHttpActionResult GetCustomer(string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing"));
            }
            Customer customer;
            try
            {
                customer = _customerManager.GetActiveCustomer(idCard);

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customer != null)
            {
                return Ok(customer);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer not found"));
            }

        }

        [HttpGet]
        [Route("api/crm/customer/customerValue/{idCard}")]
        public IHttpActionResult GetCustomerValue(string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing"));
            }
            double customerValue;
            try
            {
                customerValue = _customerManager.GetCustomerValue(idCard);
            }
            catch (KeyNotFoundException e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }
            return Ok(customerValue);

        }

        [HttpPost]
        [Route("api/crm/customer")]
        public IHttpActionResult AddNewCustomer(Customer newCustomer)
        {
            if (newCustomer == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }

            Customer addedCustomer;
            try
            {
                addedCustomer = _customerManager.AddNewCustomer(newCustomer);
            }
            catch (AlreadyExistExeption e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NonAuthoritativeInformation, e.Message));
            }
            catch (FaildToConnectDbExeption e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something went wrong"));
            }

            if (addedCustomer != null)
            {
                return Ok(addedCustomer);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User has not been added, please try again"));
            }
        }

        [HttpPut]
        [Route("api/crm/customer")]
        public IHttpActionResult EditCustomerDetails(Customer customerToEdit)
        {
            if (customerToEdit == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }

            Customer EditedCustomer;

            try
            {
                EditedCustomer = _customerManager.EditCustomer(customerToEdit);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (EditedCustomer != null)
            {
                return Ok(EditedCustomer);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not found this customer"));
            }
        }

        [HttpDelete]
        [Route("api/crm/customer/{idCard}")]
        public IHttpActionResult DeactivateCustomer(string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing"));
            }

            Customer customerAfterDeactivate;
            try
            {
                customerAfterDeactivate = _customerManager.DeactivateCustomer(idCard);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customerAfterDeactivate != null)
            {
                return Ok(customerAfterDeactivate);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not deactivate this customer"));
            }

        }
    }
}
