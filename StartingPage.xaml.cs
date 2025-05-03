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

namespace Acoutistic
{
    /// <summary>
    /// Interaction logic for StartingPage.xaml
    /// </summary>
    public partial class StartingPage : Page
    {
        public StartingPage()
        {
            InitializeComponent();
            keyChosing.Items.Add("C major");
            keyChosing.Items.Add("D major");
            keyChosing.Items.Add("E major");
            keyChosing.Items.Add("F major");
            keyChosing.Items.Add("G major");
            keyChosing.Items.Add("A major");
            keyChosing.Items.Add("B major");

            keyChosing.Items.Add("C minor");
            keyChosing.Items.Add("D minor");
            keyChosing.Items.Add("E minor");
            keyChosing.Items.Add("F minor");
            keyChosing.Items.Add("G minor");
            keyChosing.Items.Add("A minor");
            keyChosing.Items.Add("B minor");

        }

        private void metronomePageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MetronomePage()); 
        }

        private void recorderPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecorderPage());
        }

        private void confirmChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            KeyChoicePage page = new KeyChoicePage();
            page.InitializeInformation(keyChosing.SelectedItem.ToString());
            NavigationService.Navigate(page);

        }
    }
}
