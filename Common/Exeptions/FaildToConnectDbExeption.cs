using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class FaildToConnectDbExeption : Exception
    {
        public FaildToConnectDbExeption()
        {

        }

        public FaildToConnectDbExeption(string message)
            : base(message)
        {

        }
    }
}
