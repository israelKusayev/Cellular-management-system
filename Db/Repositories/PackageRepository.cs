using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories
{
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        public PackageRepository(DbContext context) : base(context)
        {

        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }

        public List<Package> GetPackageTemplate()
        {
            return  CellularContext.PackagesTable.Where((p) => p.IsPackageTemplate == true).ToList();
        }

        public Package GetPackageWithFriends(int packageId)
        {
            return CellularContext.PackagesTable.Where((p) => p.PackageId == packageId).Include(x => x.Friends).SingleOrDefault();
        }
    }
}
