using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RepositoryInterfaces
{
    public interface ILineRepository : IRepository<Line>
    {
        Line GetLineWithPackageAndFriends(int lineId);
        Line GetLineByLineNumber(string lineNumber);
        Line LineNumberIsAvailable(string lineNumber);
        Line DeactivateLine(Line line);
        Line GetLineWithPackage(int lineId);
    }
}
