using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text.Json;
using System.Net;
using Client.models;
using System.Windows;

namespace Client.ClientApp
{
    public class AppServer
    {
        public IPAddress Address { get; set; }
        public int Port { get; set; }
        public IPEndPoint EndPoint {  get; set; }
        public string Alias { get; set; }
        public Socket ClientSocket {  get; set; }

        public event EventHandler<Message> OnReceiveMessage;

        private const int MAX_MESSAGE_SIZE = 4096;

        public AppServer(string addressString, int port, string alias)
        {
            Address = IPAddress.Parse(addressString);
            Port = port;
            Alias = alias;

            EndPoint = new IPEndPoint(Address, Port);
        }

        public async Task ListenAsync(Socket client)
        {
            await ReceiveMessages(client);
        }

        public async Task<Socket> ConnectServer()
        {
            Socket client = new Socket(
                EndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            ClientSocket = client;

            await client.ConnectAsync(EndPoint);
            return client;
        }

        async Task ReceiveMessages(Socket client)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[MAX_MESSAGE_SIZE];
                    int received = await client.ReceiveAsync(buffer, SocketFlags.None);

                    if (received == 0)
                    {
                        // Connection closed by the server
                        Console.WriteLine("Server disconnected");
                        break;
                    }

                    string response = Encoding.UTF8.GetString(buffer, 0, received);
                    var message = JsonSerializer.Deserialize<Message>(response);

                    OnReceiveMessage?.Invoke(this, message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
        }

        public async Task SendMessage(Socket client, Message message)
        {
            if (client == null)
            {
                throw new Exception("Client socket not initialized");
            }
            var messageString = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            await client.SendAsync(messageBytes, SocketFlags.None);
        }
    }
}
