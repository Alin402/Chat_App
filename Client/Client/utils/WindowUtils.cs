using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.utils
{
    public class WindowUtils
    {
        public static void OpenNewWindowAndCloseCurrentOne(Window newWindow, Window currentWindow)
        {
            newWindow.Show();
            currentWindow.Close();
        }
    }
}
