using Common.Enums;
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
    public class LineRepository : Repository<Line>, ILineRepository
    {
        public LineRepository(CellularContext context) : base(context)
        {

        }

        public Line GetLineWithPackageAndFriends(int lineId)
        {
            return CellularContext.LinesTable.Where((l) => l.LineId == lineId && l.Status == LineStatus.Used).Include(y => y.Package).Include(x => x.Package.Friends).SingleOrDefault();
        }

        public Line GetLineByLineNumber(string lineNumber)
        {
            return CellularContext.LinesTable.SingleOrDefault((l) => l.LineNumber == lineNumber && l.Status == LineStatus.Used);
        }

        public Line LineNumberIsAvailable(string lineNumber)
        {
            return CellularContext.LinesTable.SingleOrDefault((l) => l.LineNumber == lineNumber
                                                     && (!(l.Status == LineStatus.Avaliable) && !(l.Status == LineStatus.Removed)));
        }

        public Line DeactivateLine(Line line)
        {

            line.Status = LineStatus.Removed;
            line.RemovedDate = DateTime.Now;
            return line;
        }

        public Line GetLineWithPackage(int lineId)
        {
            return CellularContext.LinesTable.Where(l => l.LineId == lineId).Include(p => p.Package).SingleOrDefault();
        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
