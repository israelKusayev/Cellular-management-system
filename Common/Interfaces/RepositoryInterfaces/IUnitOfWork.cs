using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IRepository<Tentity> GetRepository<Tentity>() where Tentity : class;
        ICustomerRepository Customer { get; }
        IPaymentRepository Payment { get; }
        ICallRepository Call { get; }
        ISmsRepository Sms { get; }
        ILineRepository Line { get; }
        IPackageRepository Package { get; }
        IEmployeeRepository Employee { get; }
        IFriendsRepository Friends { get; }
        int Complete();
    }
}
