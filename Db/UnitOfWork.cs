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
            Sms = new SmsRepository(_context);
            Call = new CallRepository(_context);
            Payment = new PaymentRepository(_context);
        }

        public ICustomerRepository Customer { get; private set; }
        public ILineRepository Line { get; private set; }
        public ISmsRepository Sms { get; private set; }
        public ICallRepository Call { get; private set; }
        public IPaymentRepository Payment { get; private set; }

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
