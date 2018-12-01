using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RepositoryInterfaces
{
    public interface IPackageRepository : IRepository<Package>
    {
        List<Package> GetPackageTemplate();
        Package GetPackageWithFriends(int packageId);
    }
}
