﻿using Common.Exeptions;
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
    public class PackageController : ApiController, IPackageApi
    {
        IPackageManager _packageManager;

        //ctor
        public PackageController(IPackageManager packageManager)
        {
            _packageManager = packageManager;
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
        } 

        [HttpGet]
        [Route("api/crm/package/{lineId}")]
        public IHttpActionResult GetPackage(int lineId)
        {
            if (lineId < 1)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry,Line id Can not be smaller than 1"));
            }

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
        } 

        [HttpPost]
        [Route("api/crm/package/{lineId}")]
        public IHttpActionResult AddPackageToLine(int lineId, Package package)
        {
            if (lineId < 1)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry,Line id Can not be smaller than 1"));
            }

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
            if (lineId < 1)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry,Line id Can not be smaller than 1"));
            }

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
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Sorry, we could not remove the package"));
            }
        } 

        //---------------//

        [HttpPost]
        [Route("api/crm/package/friends/{PackageId}")]
        public IHttpActionResult AddFriends(int PackageId, Friends friendsToAdd)
        {
            if (PackageId < 1)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry,package id Can not be smaller than 1"));
            }

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
        } 

        [HttpPut]
        [Route("api/crm/package/friends/{PackageId}")]
        public IHttpActionResult EditFriends(int PackageId, Friends friendsToEdit)
        {
            if (PackageId < 1)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry,package id Can not be smaller than 1"));
            }

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
        } 
    }
}
