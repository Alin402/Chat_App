using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.App.MessageActions.ConcreteMessageActions
{
    public class MessageActionRoom : IMessageAction
    {
        private AppMessages messages;

        public MessageActionRoom(AppMessages messages)
        {
            this.messages = messages;
        }

        public void Execute(Message message, Socket handler)
        {
            if (message.RoomID != null)
            {
                _ = Task.Run(async () => await messages.SendMessageToRoom(message));
            }
        }
    }
}
