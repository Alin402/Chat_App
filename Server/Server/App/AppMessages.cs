using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Server.App.MessageActions.ConcreteMessageActions;
using Server.App.MessageActions;
using Server.Models;
using static Server.App.App;
using Server.App.Rooms;

namespace Server.App
{
    public class AppMessages
    {
        public App AppServer { get; set; }
        public AppRooms Rooms { get; set; }

        private const int MAX_MESSAGE_LENGTH = 4096;

        public AppMessages(App appServer)
        {
            AppServer = appServer;
            Rooms = new AppRooms(this, new List<Room>());

            MessageTypesActions.MessageTypesActionsValuePair.Add(MessageTypes.CONN, new MessageActionConnection(appServer, this));
            MessageTypesActions.MessageTypesActionsValuePair.Add(MessageTypes.GENERAL, new MessageActionGeneral(this));
            MessageTypesActions.MessageTypesActionsValuePair.Add(MessageTypes.ROOM, new MessageActionRoom(this));
            MessageTypesActions.MessageTypesActionsValuePair.Add(MessageTypes.ROOM_INVITATION, new MessageActionRoomInvitation(appServer, this));
            MessageTypesActions.MessageTypesActionsValuePair.Add(MessageTypes.ROOM_INVITATION_ACCEPT, new MessageActionRoomInvitationAccept(Rooms, this, appServer));
            MessageTypesActions.MessageTypesActionsValuePair.Add(MessageTypes.ROOM_INVITATION_DENY, new MessageActionRoomInvitationDeny(this, appServer));
        }

        public async Task ReceiveMessages(Socket handler)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[MAX_MESSAGE_LENGTH];
                    int received = await handler.ReceiveAsync(buffer, SocketFlags.None);

                    if (received == 0)
                    {
                        Console.WriteLine("Client disconnected");
                        break;
                    }

                    string response = Encoding.UTF8.GetString(buffer, 0, received);
                    var message = JsonSerializer.Deserialize<Message>(response);

                    if (message?.Type != null)
                    {
                        IMessageAction action = MessageTypesActions.MessageTypesActionsValuePair[message.Type];
                        action.Execute(message, handler);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling connection: {ex.Message}");
            }
            finally
            {
                Message disconnectMessage = new Message()
                {
                    From = new User() { Name = "admin" },
                    Type = "conn",
                    Content = $"{AppServer.ConnectedSockets[handler]?.Name} has left the chat"
                };
                await Task.Run(async () => await BroadcastMessage(disconnectMessage, handler));
                Console.WriteLine("User has left");
                AppServer.ConnectedSockets.TryRemove(handler, out _);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        public async Task BroadcastMessage(Message message, Socket sender)
        {
            if (AppServer.ConnectedSockets.Count > 1)
            {
                foreach (var client in AppServer.ConnectedSockets.Where(c => c.Key != sender))
                {
                    try
                    {
                        string jsonMsg = JsonSerializer.Serialize(message);
                        await SendMessage(client.Key, jsonMsg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error broadcasting message: {ex.Message}");
                    }
                }
            }
        }

        public async Task SendMessageToRoom(Message message)
        {
            try
            {
                Room? room = Rooms.Rooms.Find((r) => r.ID == message.RoomID);
                if (room != null)
                {
                    string msg = JsonSerializer.Serialize(message);
                    if (message.From?.Id == room.User1?.Id)
                    {
                        await SendMessage(room.Client2, msg);
                    } else
                    {
                        await SendMessage(room.Client1, msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task SendMessage(Socket client, string message)
        {
            try
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(messageBytes, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
