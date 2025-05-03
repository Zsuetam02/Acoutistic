using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Acoutistic
{
    internal class NonBlockingMB
    {
        public static void Show(string message)
        {
            //just msbox but on another thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = new Window
                {
                    Title = "Message",
                    Width = 300,
                    Height = 100,
                    Content = new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap },
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Application.Current.MainWindow,
                };

                window.ShowDialog();
            });
        }
    }
}
