using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories
{
    public class SmsRepository :Repository<Sms>, ISmsRepository
    {
        public SmsRepository(CellularContext context):base(context)
        {

        }
        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
