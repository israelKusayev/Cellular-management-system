﻿using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    interface IEmployeeLoginApi
    {
        IHttpActionResult Login(LoginDTO loginEmployee);
    }
}
