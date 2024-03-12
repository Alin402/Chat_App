using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.App.MessageActions.ConcreteMessageActions
{
    public class MessageActionGeneral : IMessageAction
    {
        private AppMessages messages;

        public MessageActionGeneral(AppMessages messages)
        {
            this.messages = messages;
        }

        public void Execute(Message message, Socket handler)
        {
            // Broadcast message to all connected sockets
            _ = Task.Run(async () => await messages.BroadcastMessage(message, handler));
        }
    }
}
