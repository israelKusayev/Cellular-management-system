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
        //ctor
        public PackageRepository(DbContext context) : base(context)
        {

        }

        /// <summary>
        /// Get the packages that the company created by itself
        /// </summary>
        /// <returns>List of packages if succeeded otherwise null</returns>
        public List<Package> GetPackageTemplate()
        {
            return  CellularContext.PackagesTable.Where((p) => p.IsPackageTemplate == true).ToList();
        }

        public Package GetPackageWithFriends(int packageId)
        {
            return CellularContext.PackagesTable.Where((p) => p.PackageId == packageId).Include(x => x.Friends).SingleOrDefault();
        }


        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
