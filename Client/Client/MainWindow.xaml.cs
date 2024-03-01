using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.models;
using MahApps.Metro.Controls;
using Client.ClientApp;
using System.Net.Sockets;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public User ConnectedUser { get; set; }

        private const string IP_ADDRESS = "127.0.0.1";

        private const int PORT = 8080;
        public AppServer Client { get; set; }
        public Socket ClientSocket { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(User user)
        {
            InitializeComponent();
            ConnectedUser = user;
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AppServer client = new AppServer(IP_ADDRESS, PORT, ConnectedUser.Name);
                Client = client;

                Socket clientSocket = await client.ConnectServer();
                ClientSocket = clientSocket;

                await client.SendMessage(clientSocket, new Message()
                {
                    Content = "",
                    From = ConnectedUser,
                    Type = "conn"
                });

                client.OnReceiveMessage += ReceiveMessageHandler;

                _ = Task.Run(async () => await client.ListenAsync(clientSocket));
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ReceiveMessageHandler(object sender, Message message)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToChatPanel(message);
            });
        }

        private void AddMessageToChatPanel(Message message)
        {
            TextBlock newMessage = new TextBlock();
            newMessage.Text = $"{message.From.Name}: {message.Content}";

            RowDefinition newRow = new RowDefinition();
            newRow.Height = new GridLength(20);
            chat_panel.RowDefinitions.Add(newRow);

            Grid.SetRow(newMessage, chat_panel.RowDefinitions.Count - 1);
            chat_panel.Children.Add(newMessage);
        }

        private async void send_message_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string messageContent = send_message_textbox.Text;
                Message newMessage = new Message()
                {
                    Content = messageContent,
                    From = ConnectedUser,
                    Type = "general"
                };
                if (ClientSocket != null)
                {
                    await Client.SendMessage(ClientSocket, newMessage);
                    send_message_textbox.Text = "";
                }
            }
        }
    }
}
