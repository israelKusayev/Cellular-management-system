using Common.Exeptions;
using Common.Models;
using Server.Interfaces;
using Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers.CrmControllers
{
    public class PackageController : ApiController , IPackageApi
    {
        PackageManager _packageManager;
        public PackageController()
        {
            _packageManager = new PackageManager();
        }

        [HttpGet]
        [Route("api/crm/package")]
        public IHttpActionResult GetPackageTamplate()
        {
            List<Package> packages;
            try
            {
                packages = _packageManager.GetPackageTemplates();
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
        } //v

        [HttpGet]
        [Route("api/crm/package/{lineId}")]
        public IHttpActionResult GetPackage(int lineId)
        {
            Package returnedPackage;
            try
            {
                returnedPackage = _packageManager.GetPackage(lineId);
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
        } //v

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
                addedPackage = _packageManager.AddPackageToLine(lineId, package);
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

        } //v

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
                _packageManager.RemoveLineFromTemplatePackage(lineId);
                editedPackage = _packageManager.EditPackage(packageId, lineId, packageToEdit);
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
                removedPackage = _packageManager.RemovePackageFromLine(lineId);
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
        } //v

        //---------------//

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
                addedFriends = _packageManager.AddFriends(PackageId, friendsToAdd);
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
        } //v

        [HttpPut]
        [Route("api/crm/package/friends/{PackageId}")]
        public IHttpActionResult EditFriends(int PackageId, Friends friendsToEdit)
        {
            Friends edittedFriends = null;
            try
            {
                edittedFriends = _packageManager.EditFriends(PackageId, friendsToEdit);
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
        } //v
    }
}
