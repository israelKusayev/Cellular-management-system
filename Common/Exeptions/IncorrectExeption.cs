using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class IncorrectExeption : Exception
    {
        public IncorrectExeption()
        {

        }

        public IncorrectExeption(string name)
             : base(String.Format($"{name} is incorrect"))
        {

        }
    }
}
