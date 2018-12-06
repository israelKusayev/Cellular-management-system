using Common.ClientsModels;
using Common.Models;
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
        private static List<string> LineNumbers;
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
                var res = http.GetAsync("http://localhost:54377/api/customerWebsite/getLines/" + customerLoginDTO.IdentityCard).Result;
                if (res.IsSuccessStatusCode)
                {
                    Common.Models.Customer customer = res.Content.ReadAsAsync<Common.Models.Customer>().Result;
                     LineNumbers = new List<string>();
                    foreach (var item in customer.Lines)
                    {
                        LineNumbers.Add(item.LineNumber);
                    }
                    return RedirectToAction("SelectLine");
                }
                else
                {
                    string message = res.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                    return View("Error");
                }
            }
            //return View("Error");
        }

        [HttpGet]
        public ActionResult SelectLine()
        {
            ViewBag.numbers = LineNumbers;
            return View();
        }


        [HttpGet]
        public ActionResult ShowCustomerInfo(string lineNumber)
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