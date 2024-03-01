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
using MahApps.Metro.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            send_message_textbox.Focus();

            TextBlock message = new TextBlock();
            message.Text = "Hello World";

            chat_panel.Children.Add(message);

            TextBlock message2 = new TextBlock();
            message2.Text = "New Hello World";
            chat_panel.Children.Add(message2);
            message2.Margin = new Thickness(0, 20, 0, 0);
        }
    }
}
