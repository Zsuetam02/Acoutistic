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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Acoutistic
{
    /// <summary>
    /// Interaction logic for MetronomePage.xaml
    /// </summary>
    public partial class MetronomePage : Page
    {
        bool buttonMetronome = false;
        int bpm;
        Metronome metronome;
        public MetronomePage()
        {
            InitializeComponent();
            
            metronome = new Metronome("metronomeSound.wav");

            bpm = 100;
            bpmSlider.Value = bpm;
        }

        private void MetronomePage_Loaded(object sender, RoutedEventArgs e)
        {
            metronomePage.DataContext = metronome;
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tBbpm.Text == null || tBbpm.Text == "0" || tBbpm.Text == "")
            {
                bpm = 100;
            }
            else
            {
                bpm = int.Parse(tBbpm.Text);
            }

            if (buttonMetronome == false)
            {
                buttonMetronome = true;
                Metronome.IsActive = false;
                await Task.Run(() =>
                {
                    metronome.Loop(bpm);
                });

            }
            else
            {
                buttonMetronome = false;
                Metronome.IsActive = true;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bpmSlider.Maximum = 300;
            if (bpmSlider.Value > 0)
            {
                bpm = (int)bpmSlider.Value;
                Metronome.IsActive = true;
                buttonMetronome = false;
                tBbpm.Text = bpm.ToString();
            }
        }

        private void tBbpm_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(tBbpm.Text) && int.TryParse(tBbpm.Text, out int parsedBpm) && parsedBpm > 0)
            {
                bpmSlider.Value = parsedBpm;
            }
           
            if(int.TryParse(tBbpm.Text, out int parsed) && parsed < 0)
            {
                MessageBox.Show("Wow, you really are something putting minus values as beats per minute, we shall change the definition of entropy!");
            }

        }
        private void recorderPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecorderPage());
        }

        private void startingPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartingPage());
        }
    }
}