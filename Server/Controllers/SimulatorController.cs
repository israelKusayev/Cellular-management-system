using Common.Exeptions;
using Common.ModelsDTO;
using Crm.Dal.Dals;
using Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class SimulatorController : ApiController, ISimulatorApi
    {
        SimulatorDal _simulatorDbManager;

        public SimulatorController()
        {
            _simulatorDbManager = new SimulatorDal();
        }

        [HttpPost]
        [Route("api/simulator")]
        public IHttpActionResult Simulate(SimulateDTO simulateDTO)
        {
            if (simulateDTO == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, "Sorry, we could not get the data"));
            }

            try
            {
                _simulatorDbManager.SimulateCallsOrSms(simulateDTO);
                return Ok();
            }
            catch (EmptyException e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NoContent, e.Message));

            }
            catch (FaildToConnectDbExeption e)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Something went wrong"));
            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Something went wrong"));
            }



        }
    }
}
