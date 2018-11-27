using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Repositories
{
    class LineRepository : Repository<Line>, ILineRepository
    {
        public LineRepository(CellularContext context) : base(context)
        {

        }
    }
}
