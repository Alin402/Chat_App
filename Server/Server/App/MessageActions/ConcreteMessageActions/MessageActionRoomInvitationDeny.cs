using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Server.App.Rooms;
using Server.App.Utils;
using Server.Models;

namespace Server.App.MessageActions.ConcreteMessageActions
{
    public class MessageActionRoomInvitationDeny : IMessageAction
    {
        private AppMessages? messages;
        private App? appServer;

        public MessageActionRoomInvitationDeny(AppMessages? messages, App? appServer)
        {
            this.messages = messages;
            this.appServer = appServer;
        }

        public void Execute(Message message, Socket handler)
        {
            var messageToSocket = appServer?.ConnectedSockets.FirstOrDefault(x => x.Value == message?.To).Key;

            // send acceptation message to the user
            string jsonMsg = JsonSerializer.Serialize(message);
            _ = Task.Run(async () => await messages.SendMessage(messageToSocket, jsonMsg));
        }
    }
}
