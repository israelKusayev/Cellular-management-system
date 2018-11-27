using Common.Enums;
using Common.Models;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
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

        public CellularContext CellularContext
        {
            get { return Context as CellularContext; }
        }
    }
}
