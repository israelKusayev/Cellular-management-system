using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories
{
    class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CellularContext context):base(context)
        {

        }
        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
