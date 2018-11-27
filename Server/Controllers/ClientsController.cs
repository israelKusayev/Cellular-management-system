using Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class ClientsController : ApiController
    {
        public ClientsController()
        {
            DbInitializer db = new DbInitializer();
            db.Seed();
        }
    }
}
