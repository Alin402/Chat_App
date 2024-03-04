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

        public event EventHandler<OnNewUserConnectedEventArgs> OnNewUserConnected;

        public class OnNewUserConnectedEventArgs : EventArgs
        {
            public User UserProperty { get; set; }
            public Socket Handler { get; set; }

            public OnNewUserConnectedEventArgs(User userProperty, Socket handler)
            {
                UserProperty = userProperty;
                Handler = handler;
            }
        }

        public event EventHandler OnConnectedSocketsChange;

        public ConcurrentDictionary<Socket, User> ConnectedSockets = new ConcurrentDictionary<Socket, User>();

        private bool IsCheckConnectedSocketsChanged = true;

        private const int CHECK_FOR_NEW_CONNECTIONS_RATE = 200;

        public AppMessages Messages {  get; private set; }

        public App(string ipAddressString, int port)
        {
            Address = IPAddress.Parse(ipAddressString);
            Port = port;

            EndPoint = new IPEndPoint(Address, Port);

            OnNewUserConnected += App_OnNewUserConnected;
            Messages = new AppMessages(this);
        }

        public void RaiseNewConnectedUserEvent(object sender, OnNewUserConnectedEventArgs args)
        {
            OnNewUserConnected?.Invoke(sender, args);
        }

        private async Task CheckIfConnectedSocketsChanged()
        {
            try
            {
                int connectedSocketsLength = ConnectedSockets.Count;
                while (IsCheckConnectedSocketsChanged)
                {
                    if (ConnectedSockets.Count != connectedSocketsLength)
                    {
                        OnConnectedSocketsChange?.Invoke(this, EventArgs.Empty);
                        connectedSocketsLength = ConnectedSockets.Count;
                    }
                    await Task.Delay(CHECK_FOR_NEW_CONNECTIONS_RATE);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void App_OnNewUserConnected(object? sender, OnNewUserConnectedEventArgs args)
        {
            Console.WriteLine($"{args.UserProperty.Name} has joined the chat");
            if (ConnectedSockets[args.Handler] == null )
            {
                ConnectedSockets.TryAdd(args.Handler, args.UserProperty);
            }

            ConnectedSockets[args.Handler] = args.UserProperty;
        }

        private void App_OnConnectedSocketsChange(object? sender, EventArgs args)
        {
            _ = Task.Run(async () => await Messages.BroadcastConnectedUsers());
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

            OnConnectedSocketsChange += App_OnConnectedSocketsChange;

            Task listenTask = Task.Run(async () => await ListenAsync(listener));
            Task checkIfConnectedSocketsChange = Task.Run(async () => await CheckIfConnectedSocketsChanged());

            await Task.WhenAll(listenTask, checkIfConnectedSocketsChange);

            OnConnectedSocketsChange -= App_OnConnectedSocketsChange;
        }

        private async Task ListenAsync(Socket listener)
        {
            try
            {
                // Listen for connections
                while (true)
                {
                    var handler = await listener.AcceptAsync();

                    ConnectedSockets.TryAdd(handler, null);

                    // Listen for messages from each connection
                    _ = Task.Run(async () => await Messages.ReceiveMessages(handler));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
