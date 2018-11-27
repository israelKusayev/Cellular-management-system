using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logger
{
    public class FileLogger : IPathLogger
    {
        public void Log(string msg, string path)
        {
            if(!Directory.Exists(@"c:\Cellular\Log"))
            {
                Directory.CreateDirectory(@"c:\Cellular\Log");
            }

            using (StreamWriter file = new StreamWriter(@"c:\Cellular\Log\"+path, true))
            {
                file.WriteLineAsync(DateTime.Now +": " + msg);
            }
        }

        public void Log(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
