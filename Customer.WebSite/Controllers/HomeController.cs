using Customer.WebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Customer.WebSite.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(CustomerLoginDTO customerLoginDTO)
        {
            using (var http = new HttpClient())
            {
                var res = http.GetAsync("http://localhost:54377/api/customerWebsite/getData/" + customerLoginDTO.IdentityCard).Result;
                if (res.IsSuccessStatusCode)
                {
                    var s = res.Content.ReadAsAsync<int>().Result;
                    return RedirectToAction("ShowCustomerInfo");
                }
            }
            return View("Error");
        }

        [HttpGet]
        public ActionResult ShowCustomerInfo()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact with us.";
            return View();
        }
    }
}