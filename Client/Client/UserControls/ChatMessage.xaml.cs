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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.UserControls
{
    /// <summary>
    /// Interaction logic for ChatMessage.xaml
    /// </summary>
    public partial class ChatMessage : UserControl
    {
        public string CustomContent
        {
            get { return (string)GetValue(CustomContentProperty); }
            set { SetValue(CustomContentProperty, value); }
        }
        public static readonly DependencyProperty CustomContentProperty =
            DependencyProperty.Register(
                "CustomContent",
                typeof(string),
                typeof(ChatMessage));

        public string CustomUserName
        {
            get { return (string)GetValue(CustomUserNameProperty); }
            set { SetValue(CustomUserNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomUserNameProperty =
            DependencyProperty.Register("CustomUserName", typeof(string), typeof(ChatMessage));



        public bool IsMessageFromYou
        {
            get { return (bool)GetValue(IsMessageFromYouProperty); }
            set { SetValue(IsMessageFromYouProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMessageFromYou.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMessageFromYouProperty =
            DependencyProperty.Register("IsMessageFromYou", typeof(bool), typeof(ChatMessage));

        public ChatMessage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush fromYouBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#144272");
            SolidColorBrush normalBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#0A2647");
            if (IsMessageFromYou)
            {
                message_grid.Background = fromYouBrush;
                Grid.SetColumn(message_grid, 1);
            } else
            {
                message_grid.Background = normalBrush;
            }
        }
    }
}
