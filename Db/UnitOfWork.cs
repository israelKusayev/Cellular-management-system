using Common.RepositoryInterfaces;
using Db.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext, new()
    {
        private readonly DbContext _context;
        public UnitOfWork()
        {
            _context = new TContext();
          
            Customer = new CustomerRepository(_context);
            Line = new LineRepository(_context);
            Sms = new SmsRepository(_context);
            Call = new CallRepository(_context);
            Payment = new PaymentRepository(_context);
            Package = new PackageRepository(_context);
            Friends = new FriendsRepository(_context);
            Employee = new EmployeeRepository(_context);
        }

        public ICustomerRepository Customer { get; private set; }
        public ILineRepository Line { get; private set; }
        public ISmsRepository Sms { get; private set; }
        public ICallRepository Call { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public IPackageRepository Package { get; private set; }
        public IFriendsRepository Friends { get; private set; }
        public IEmployeeRepository Employee { get; private set; }

        public int Complete()
        {
            
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        //public IRepository<Tentity> GetRepository<Tentity>() where Tentity : class
        //{
        //    if (_repositories.Keys.Contains(typeof(Tentity)))
        //    {
        //        return _repositories[typeof(Tentity)] as IRepository<Tentity>;
        //    }
        //    var repository = new Repository<Tentity>
        //}
    }
}
