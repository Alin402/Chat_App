using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Server.Models;

namespace Server.App.MessageActions.ConcreteMessageActions
{
    public class MessageActionRoomInvitation : IMessageAction
    {
        private App appServer;
        private AppMessages messages;

        public MessageActionRoomInvitation(App appServer, AppMessages messages)
        {
            this.appServer = appServer;
            this.messages = messages;
        }

        public void Execute(Message message, Socket handler)
        {
            try
            {
                Socket client = appServer.ConnectedSockets.FirstOrDefault(x => x.Value == message.To).Key;

                if (client != null)
                {
                    string jsonMsg = JsonSerializer.Serialize(message);
                    _ = Task.Run(async () => await messages.SendMessage(client, jsonMsg));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
