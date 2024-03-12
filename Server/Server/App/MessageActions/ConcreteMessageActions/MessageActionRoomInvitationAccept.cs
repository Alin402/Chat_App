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
    public class MessageActionRoomInvitationAccept : IMessageAction
    {
        private AppRooms? rooms;
        private AppMessages? messages;
        private App? appServer;

        public MessageActionRoomInvitationAccept(AppRooms? rooms, AppMessages? messages, App? appServer)
        {
            this.rooms = rooms;
            this.messages = messages;
            this.appServer = appServer;
        }

        public void Execute(Message message, Socket handler)
        {
            var messageToSocket = appServer?.ConnectedSockets.FirstOrDefault(x => x.Value == message?.To).Key;
            // create new room
            Room newRoom = new Room
            {
                ID = IDGenerator.GenerateRandomID(),
                Name = $"Room with {message?.From?.Name} and {message?.To?.Name}",
                User1 = message?.From,
                User2 = message?.To,
                Client1 = handler,
                Client2 = messageToSocket,
            };

            if (rooms != null )
            {
                rooms.AddRoom(newRoom);
            }

            // send acceptation message to the user
            string jsonMsg = JsonSerializer.Serialize(message);
            _ = Task.Run(async () => await messages.SendMessage(messageToSocket, jsonMsg));
        }
    }
}
