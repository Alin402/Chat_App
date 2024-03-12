using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Models;
using static Server.App.App;

namespace Server.App.MessageActions.ConcreteMessageActions
{
    public class MessageActionConnection : IMessageAction
    {
        private App appServer;
        private AppMessages messages;

        public MessageActionConnection(App appServer, AppMessages messages)
        {
            this.appServer = appServer;
            this.messages = messages;
        }

        public void Execute(Message message, Socket handler)
        {
            OnNewUserConnectedEventArgs args = new(message.From, handler);
            appServer.RaiseNewConnectedUserEvent(this, args);

            message.Content = $"{message.From.Name} has joined the chat";
            message.From = new User { Name = "admin" };

            _ = Task.Run(async () => await messages.BroadcastMessage(message, handler));
        }
    }
}
