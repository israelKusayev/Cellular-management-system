using Common.RepositoryInterfaces;
using Db.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CellularContext _context;

        public UnitOfWork(CellularContext context)
        {
            _context = context;
            Customer = new CustomerRepository(_context);
            Line = new LineRepository(_context);
            Package = new PackageRepository(_context);
            Friends = new FriendsRepository(_context);
        }

        public ICustomerRepository Customer { get; private set; }
        public ILineRepository Line { get; private set; }
        public IPackageRepository Package { get; private set; }
        public IFriendsRepository Friends { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
