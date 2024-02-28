using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Server.Models;

namespace Server
{
    public class App
    {
        public IPAddress Address {  get; set; }
        public int Port { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public event EventHandler<User> OnNewUserConnected;

        private ConcurrentDictionary<Socket, User> ConnectedSockets = new ConcurrentDictionary<Socket, User>();

        private const int MAX_MESSAGE_LENGTH = 4096;

        public App(string ipAddressString, int port)
        {
            Address = IPAddress.Parse(ipAddressString);
            Port = port;

            EndPoint = new IPEndPoint(Address, Port);

            OnNewUserConnected += App_OnNewUserConnected;
        }

        private void App_OnNewUserConnected(object? sender, User user)
        {
            Console.WriteLine($"{user.Name} has joined the chat");
        }

        public async Task RunAsync()
        {
            Socket listener = new Socket(
                EndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(EndPoint);

            listener.Listen();
            Console.WriteLine($"Server running on port {Port}");
            
            // Listen for connections
            while (true)
            {
                var handler = await listener.AcceptAsync();

                ConnectedSockets.TryAdd(handler, null);

                // Listen for messages from each connection
                _ = Task.Run(async () => await ReceiveMessages(handler));
            }
        }

        private async Task ReceiveMessages(Socket handler)
        {
            User user = null;
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

                    user = message.From;

                    if (message.Type == MessageTypes.CONN)
                    {
                        OnNewUserConnected?.Invoke(this, message.From);
                        message.Content = $"{message.From.Name} has joined the chat";
                        message.From = new User { Name = "admin" };
                        _ = Task.Run(async () => await BroadcastMessage(message, handler));
                    } else if (message.Type == MessageTypes.GENERAL)
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
                    From = new User() {  Name = "admin" },
                    Type = "con",
                    Content = $"{user?.Name} has left the chat"
                };
                await Task.Run(async () => await BroadcastMessage(disconnectMessage, handler));
                Console.WriteLine("User has left");
                ConnectedSockets.TryRemove(handler, out _);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        private async Task BroadcastMessage(Message message, Socket sender)
        {
            if (ConnectedSockets.Count > 1)
            {
                foreach (var client in ConnectedSockets.Where(c => c.Key != sender))
                {
                    try
                    {
                        string jsonMsg = JsonSerializer.Serialize(message);
                        SendMessage(client.Key, jsonMsg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error broadcasting message: {ex.Message}");
                    }
                }
            }
        }

        async void SendMessage(Socket client, string message)
        {
            try
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(messageBytes, SocketFlags.None);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message); 
            }
        }
    }
}
