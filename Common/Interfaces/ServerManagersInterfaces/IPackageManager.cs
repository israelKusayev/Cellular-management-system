using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.ServerManagersInterfaces
{
    public interface IPackageManager
    {
        List<Package> GetPackageTemplates();
        Package GetPackage(int lineId);
        Package AddPackageToLine(int lineId, Package package);
        Package RemovePackageFromLine(int lineId);
        Package EditPackage(int packageId, int lineId, Package newPackage);
        bool RemoveLineFromTemplatePackage(int lineId);
        Friends AddFriends(int packageId, Friends friendsToAdd);
        Friends EditFriends(int packageId, Friends friendsToEdit);
    }
}
