using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exeptions
{
    public class AlreadyExistExeption: Exception
    {
        public AlreadyExistExeption()
        {

        }

        public AlreadyExistExeption(string name)
            : base(String.Format($"{name} is already exists"))
        {

        }

        public AlreadyExistExeption(string name,string type)
    : base(String.Format($"{type} is already exists to {name}"))
        {

        }
    }
}
