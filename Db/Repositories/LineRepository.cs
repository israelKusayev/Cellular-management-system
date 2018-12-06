using Common.Enums;
using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Db.Repositories
{
    public class LineRepository : Repository<Line>, ILineRepository
    {
        //ctor
        public LineRepository(DbContext context) : base(context)
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

        public Line GetLineByLineNumberWithPaymentsAndPackageAndFriends(string lineNumber)
        {
            return CellularContext.LinesTable.Where((l) => l.LineNumber == lineNumber && l.Status == LineStatus.Used).Include(y => y.Payments).Include(y => y.Package).Include(x => x.Package.Friends).SingleOrDefault();
        }

        public Line GetLineWithCalls(string lineNumber, DateTime requestedTime)
        {
            return  CellularContext.LinesTable.Where(l => l.LineNumber == lineNumber && l.Status == LineStatus.Used)
                                              .IncludeFilter(c => c.Calls.Where(d => d.DateOfCall.Year == requestedTime.Year && d.DateOfCall.Month == requestedTime.Month))
                                              .SingleOrDefault();
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

        public IEnumerable<Line> GetAllLinesWithAllEntities()
        {
            return CellularContext.LinesTable.Where((l) => l.Status == LineStatus.Used || l.Status == LineStatus.Removed).Include(x => x.Payments).Include(p => p.Package).Include(f => f.Package.Friends).Include(s => s.Calls).Include(m => m.Messages);
        }

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
