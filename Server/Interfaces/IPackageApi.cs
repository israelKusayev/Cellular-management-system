using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Server.Interfaces
{
    interface IPackageApi
    {
        IHttpActionResult GetPackageTamplate();
        IHttpActionResult GetPackage(int lineId);
        IHttpActionResult AddPackageToLine(int lineId, Package package);
        IHttpActionResult EditPackage(int packageId, int lineId, Package packageToEdit);
        IHttpActionResult RemovePackageFromLine(int lineId);
        IHttpActionResult AddFriends(int PackageId, Friends friendsToAdd);
        IHttpActionResult EditFriends(int PackageId, Friends friendsToEdit);
    }
}
