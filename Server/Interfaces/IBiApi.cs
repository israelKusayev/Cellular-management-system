using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    interface IBiApi
    {
        IHttpActionResult LoginManager(LoginDTO loginDTO);
        IHttpActionResult GetMostProfitableCustomers(); //list<profilablrCustomerDTO> include first name last name id and profit
        IHttpActionResult GetMostCallingToCenterCustomers();//list<customer>
        IHttpActionResult GetOpinionLeadersCustomers(); //list<customer>
        IHttpActionResult GetLinesAtRiskOfAbandonment();//list<customer>
        IHttpActionResult GetBestSellerEmployees(); //list<employee> customer collection after filter
        IHttpActionResult GetGroupsOfFreindsWhoTalkEachOther();
    }
}
