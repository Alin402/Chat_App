using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Server.Models;
using static Server.App;

namespace Server
{
    public class AppMessages
    {
        public App AppServer {  get; set; }

        private const int MAX_MESSAGE_LENGTH = 4096;

        public AppMessages(App appServer)
        {
            AppServer = appServer;
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

                    if (message?.Type == MessageTypes.CONN)
                    {
                        OnNewUserConnectedEventArgs args = new(message.From, handler);
                        AppServer.RaiseNewConnectedUserEvent(this, args);

                        message.Content = $"{message.From.Name} has joined the chat";
                        message.From = new User { Name = "admin" };

                        _ = Task.Run(async () => await BroadcastMessage(message, handler));
                    }
                    else if (message?.Type == MessageTypes.GENERAL)
                    {
                        // Broadcast message to all connected sockets
                        _ = Task.Run(async () => await BroadcastMessage(message, handler));
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

        public async Task BroadcastConnectedUsers()
        {
            if (AppServer.ConnectedSockets.Count > 0)
            {
                try
                {
                    List<User> users = AppServer.ConnectedSockets.Values.ToList();

                    Message message = new Message()
                    {
                        Type = "receive_users",
                        From = new User() { Name = "admin" },
                        Content = users
                    };
                    string messageString = JsonSerializer.Serialize(message);
                    foreach (var client in AppServer.ConnectedSockets)
                    {
                        try
                        {
                            await SendMessage(client.Key, messageString);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error broadcasting message: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        async Task SendMessage(Socket client, string message)
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
