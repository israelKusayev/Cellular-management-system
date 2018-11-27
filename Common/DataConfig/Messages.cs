using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataConfig
{
    public static class Messages
    {
        public static Dictionary<MessageType, string> messageFor = new Dictionary<MessageType, string>
        {
            {MessageType.GeneralDbFaild, "The attempt to communicate with the database failed"},

        };
    }
}
