using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.App.MessageActions
{
    public interface IMessageAction
    {
        void Execute(Message message, Socket handler);
    }
}
