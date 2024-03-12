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
using Client.UserControls;

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
            user_name_text.Text = ConnectedUser.Name;
            send_message_textbox.Focus();
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
                client.OnReceiveConnectedUsers += Client_OnReceiveConnectedUsers;

                _ = Task.Run(async () => await client.ListenAsync(clientSocket));
            } catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Client_OnReceiveConnectedUsers(object? sender, List<User> e)
        {
            Dispatcher.Invoke(() =>
            {
                AddUsersToComboBox(e);
            });
        }

        private void AddUsersToComboBox(List<User> users)
        {
            // placeholder item
            ComboBoxItem placeholderItem = new ComboBoxItem();
            placeholderItem.Content = "Connected Users";
            placeholderItem.IsEnabled = false;

            List<ComboBoxItem> newComboBoxItems = new List<ComboBoxItem>() { placeholderItem };

            foreach (User user in users)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Content = user.Name;
                newComboBoxItems.Add(newItem);
            }

            connected_users_combobox.Items.Clear();

            foreach (ComboBoxItem item in newComboBoxItems)
            {
                connected_users_combobox.Items.Add(item);
            }

            connected_users_combobox.SelectedIndex = 0;
        }

        private void ReceiveMessageHandler(object sender, Message message)
        {
            Dispatcher.Invoke(() =>
            {
                AddMessageToChatPanel(message, false);
            });
        }

        private void AddMessageToChatPanel(Message message, bool isMessageFromYou)
        {
            RowDefinition newRow = new RowDefinition();
            newRow.Height = GridLength.Auto;
            chat_panel.RowDefinitions.Add(newRow);

            ChatMessage chatMessage = new ChatMessage();
            chatMessage.CustomContent = message.Content.ToString();
            chatMessage.CustomUserName = message.From.Name;
            chatMessage.IsMessageFromYou = isMessageFromYou;
            Grid.SetRow(chatMessage, chat_panel.RowDefinitions.Count - 1);
            chat_panel.Children.Add(chatMessage);

            chat_panel_scroll_viewer.ScrollToVerticalOffset(
                chat_panel_scroll_viewer.ScrollableHeight + 
                chat_panel.RowDefinitions[chat_panel.RowDefinitions.Count - 1].MaxHeight);
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

                AddMessageToChatPanel(newMessage, true);
            }
        }
    }
}
