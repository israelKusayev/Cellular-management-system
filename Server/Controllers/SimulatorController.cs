using Common.Exeptions;
using Common.ModelsDTO;
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
    public class SimulatorController : ApiController, ISimulatorApi
    {
        SimulatorManager _simulatorManager;
        public SimulatorController()
        {
            _simulatorManager = new SimulatorManager();
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
                _simulatorManager.SimulateCallsOrSms(simulateDTO);
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
