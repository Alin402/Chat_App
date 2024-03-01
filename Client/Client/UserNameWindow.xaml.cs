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
using Client.utils;
using MahApps.Metro.Controls;

namespace Client
{
    /// <summary>
    /// Interaction logic for UserNameWindow.xaml
    /// </summary>
    public partial class UserNameWindow : MetroWindow
    {
        public UserNameWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            alias_textbox.Focus();
        }

        private void alias_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                WindowUtils.OpenNewWindowAndCloseCurrentOne(new MainWindow(), this);
            }
        }
    }
}
