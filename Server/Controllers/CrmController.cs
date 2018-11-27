using Common.Exeptions;
using Common.Models;
using Common.ModelsDTO;
using Crm.Dal.Dals;
using Db;
using Server.Interfaces;
using Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    //fix the stutus code
    public class CrmController : ApiController, ILoginApi, ILineApi, IPackageApi
    {
        LoginEmployeeDal _loginDbManager;
        CustomerDal _customerDbManage;
        LineDal _lineDbManager;
        PackageDal _packageDbManager;


        //---

        CustomerManager _customerManager = new CustomerManager();


        //Ctor
        public CrmController()
        {
            _loginDbManager = new LoginEmployeeDal();
            _customerDbManage = new CustomerDal();
            _lineDbManager = new LineDal();
            _packageDbManager = new PackageDal();

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
                registeredEmployee = _loginDbManager.Login(loginEmployee);
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

        //------------------//

        //[HttpGet]
        //[Route("api/crm/customer/{idCard}")]
        //public IHttpActionResult GetCustomer(string idCard)
        //{
        //    if (string.IsNullOrWhiteSpace(idCard))
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing"));
        //    }
        //    Customer customer;
        //    try
        //    {
        //        customer = _customerManager.GetActiveCustomer(idCard);
        //    }
        //    catch (Exception)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
        //    }

        //    if (customer != null)
        //    {
        //        return Ok(customer);
        //    }
        //    else
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Customer not found"));
        //    }

        //}

        ///// <summary>
        ///// Get customer value
        ///// </summary>
        ///// <param name="idCard">Ccustomer identity card</param>
        //[HttpGet]
        //[Route("api/crm/customer/customerValue/{idCard}")]
        //public IHttpActionResult GetCustomerValue(string idCard)
        //{
        //    if (string.IsNullOrWhiteSpace(idCard))
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing"));
        //    }
        //    double customerValue;
        //    try
        //    {
        //        customerValue = _customerManager.GetCustomerValue(idCard);
        //    }
        //    catch (KeyNotFoundException e)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
        //    }
        //    catch (Exception)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
        //    }
        //    return Ok(customerValue);

        //}

        //[HttpPost]
        //[Route("api/crm/customer")]
        //public IHttpActionResult AddNewCustomer(Customer newCustomer)
        //{
        //    if (newCustomer == null)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
        //    }

        //    Customer addedCustomer;
        //    try
        //    {
        //        addedCustomer = _customerManager.AddNewCustomer(newCustomer);
        //    }
        //    catch (AlreadyExistExeption e)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NonAuthoritativeInformation, e.Message));
        //    }
        //    catch (FaildToConnectDbExeption e)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
        //    }
        //    catch (Exception e)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something went wrong"));
        //    }

        //    if (addedCustomer != null)
        //    {
        //        return Ok(addedCustomer);
        //    }
        //    else
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User has not been added, please try again"));
        //    }
        //}

        //[HttpPut]
        //[Route("api/crm/customer")]
        //public IHttpActionResult EditCustomerDetails(Customer customerToEdit)
        //{
        //    if (customerToEdit == null)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
        //    }

        //    Customer EditedCustomer;

        //    try
        //    {
        //        EditedCustomer = _customerDbManage.EditCustomer(customerToEdit);
        //    }
        //    catch (Exception)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
        //    }

        //    if (EditedCustomer != null)
        //    {
        //        return Ok(EditedCustomer);
        //    }
        //    else
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not found this customer"));
        //    }
        //}

        //[HttpDelete]
        //[Route("api/crm/customer/{idCard}")]
        //public IHttpActionResult DeactivateCustomer(string idCard)
        //{
        //    if (string.IsNullOrWhiteSpace(idCard))
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Some details are missing"));
        //    }

        //    Customer customerAfterDeactivate;
        //    try
        //    {
        //        customerAfterDeactivate = _customerDbManage.DeactivateCustomer(idCard);
        //    }
        //    catch (Exception)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
        //    }

        //    if (customerAfterDeactivate != null)
        //    {
        //        return Ok(customerAfterDeactivate);
        //    }
        //    else
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not deactivate this customer"));
        //    }

        //}

        //------------------------//

        [HttpGet]
        [Route("api/crm/line/{idCard}")]
        public IHttpActionResult GetCustomerLines(string idCard)
        {
            if (string.IsNullOrWhiteSpace(idCard))
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }

            List<Line> customerLines;
            try
            {
                customerLines = _lineDbManager.GetCustomerLines(idCard);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (customerLines != null)
            {
                return Ok(customerLines);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Could not found lines to this customer"));
            }
        }

        [HttpPost]
        [Route("api/crm/line/{customerId}")]
        public IHttpActionResult AddNewLine(int customerId, Line lineToAdd)
        {
            if (lineToAdd == null || lineToAdd.LineNumber == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }
            Line addedLine;
            try
            {
                addedLine = _lineDbManager.AddNewLine(lineToAdd, customerId);
            }
            catch (AlreadyExistExeption e) //this line already exists to this customer
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Found, e.Message));
            }
            catch (FoundLineExeption e) //this line already exists to another customer
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, e.Message));
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (addedLine != null)
            {
                return Ok(addedLine);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "We could not add this line, please try again"));
            }
        }

        [HttpDelete]
        [Route("api/crm/line/{lineId}")]
        public IHttpActionResult DeactivateLine(int lineId)
        {
            Line editedLine;
            try
            {
                editedLine = _lineDbManager.DeactivateLine(lineId);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (editedLine != null)
            {
                return Ok(editedLine);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, could not change the customer's details"));
            }
        }

        //---------------------//

        [HttpGet]
        [Route("api/crm/package")]
        public IHttpActionResult GetPackageTamplate()
        {
            List<Package> packages;
            try
            {
                packages = _packageDbManager.GetPackageTemplates();
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (packages != null)
            {
                return Ok(packages);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not find template packages"));
            }
        }

        [HttpGet]
        [Route("api/crm/package/{lineId}")]
        public IHttpActionResult GetPackage(int lineId)
        {
            Package returnedPackage;
            try
            {
                returnedPackage = _packageDbManager.GetPackage(lineId);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (returnedPackage != null)
            {
                return Ok(returnedPackage);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not find any packages for this line"));
            }
        }

        [HttpPost]
        [Route("api/crm/package/{lineId}")]
        public IHttpActionResult AddPackageToLine(int lineId, Package package)
        {
            if (package == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }

            Package addedPackage;
            try
            {
                addedPackage = _lineDbManager.AddPackageToLine(lineId, package);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (addedPackage != null)
            {
                return Ok(addedPackage);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not add the package"));
            }

        }

        [HttpPut]
        [Route("api/crm/package/{packageId}/{lineId}")]
        public IHttpActionResult EditPackage(int packageId, int lineId, Package packageToEdit)
        { //edit package and edit or remove friends
            if (packageToEdit == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "The server did not receive the data"));
            }

            Package editedPackage;
            try
            {
                _packageDbManager.RemoveLineFromTemplatePackage(lineId);
                editedPackage = _packageDbManager.EditPackage(packageId, lineId, packageToEdit);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (editedPackage != null)
            {
                return Ok(editedPackage);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not edit the package"));
            }
        }

        [HttpDelete]
        [Route("api/crm/package/{lineId}")]
        public IHttpActionResult RemovePackageFromLine(int lineId)
        {
            Package removedPackage;
            try
            {
                removedPackage = _lineDbManager.RemovePackageFromLine(lineId);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (removedPackage != null)
            {
                return Ok(removedPackage);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not rempve the package"));
            }
        }


        //-----------------//
        [HttpPost]
        [Route("api/crm/package/friends/{PackageId}")]
        public IHttpActionResult AddFriends(int PackageId, Friends friendsToAdd)
        {
            if (friendsToAdd == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }

            Friends addedFriends;
            try
            {
                addedFriends = _packageDbManager.AddFriends(PackageId, friendsToAdd);
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (addedFriends != null)
            {
                return Ok(addedFriends);
            }
            else
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "We could not add friends, please try again"));
            }
        }

        [HttpPut]
        [Route("api/crm/package/friends/{PackageId}")]
        public IHttpActionResult EditFriends(int PackageId, Friends friendsToEdit)
        {
            Friends edittedFriends = null;
            try
            {
                edittedFriends = _packageDbManager.EditFriends(PackageId, friendsToEdit);
            }
            catch (IncorrectExeption e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, e.Message));
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }

            if (edittedFriends != null)
            {
                return Ok(edittedFriends);
            }
            return Ok();//?
            //else
            //{
            //    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "We could not edit your friends, please try again"));
            //}
        }
    }
}
