using Common.Interfaces.ServerManagersInterfaces;
using Common.RepositoryInterfaces;
using Db;
using Db.Repositories;
using Server.Managers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Unity;

namespace Server
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
            GlobalConfiguration.Configure(WebApiConfig.Register);
            new CellularContext().InitDataBase();
        }
    }
}
