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
    public class LineController : ApiController, ILineApi
    {
        ILineManager _lineManager;

        //ctor
        public LineController(ILineManager lineManager)
        {
            _lineManager = lineManager;
        }

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
                customerLines = _lineManager.GetCustomerLines(idCard);
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
                addedLine = _lineManager.AddNewLine(lineToAdd, customerId);
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
                editedLine = _lineManager.DeactivateLine(lineId);
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
    }
}
