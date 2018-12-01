using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class FoundLineExeption : Exception
    {
        public FoundLineExeption()
        {

        }

        public FoundLineExeption(string name, string type)
            : base(String.Format($"{type} is already exists to {name}"))
        {

        }
    }
}
