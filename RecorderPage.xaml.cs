using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;

namespace Acoutistic
{
    /// <summary>
    /// Interaction logic for RecorderPage.xaml
    /// </summary>
    public partial class RecorderPage : Page
    {
        private AudioProcessor audio;
        private List<Label> labels;
        public RecorderPage()
        {
            InitializeComponent();
            audio = new AudioProcessor();
            labels = new List<Label> { firste, firstB, firstG, firstD, firstA, firstE, seconde, secondB, secondG, secondD, secondA, secondE };
        }

        private void startingPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartingPage());
        }

        private void metronomePageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MetronomePage());
        }

        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            recordButton.Background = Brushes.Red;
            audio.StartRecording();
        }

        private void stopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            recordButton.Background = Brushes.Azure;
            audio.StopRecording();
            HideFirstTab();
            HideSecondTab();
            ClearLabels();
            Transcriber transcriber = new Transcriber(audio.frequencies);
            transcriber.ExtractNotes();
            //troubleshooting
            List<string> checkStr = new List<string>() { "A", "D", "G", "B" };
            List<int> checkFret = new List<int>() { 1, 2, 3, 4 };
            WriteLinesForTabs(transcriber.stringInOrder, transcriber.tabPositionInOrder);
            Thread.Sleep(1000);
        }

        private void WriteLinesForTabs(List<string> strInOrder, List<int> frets)
        {
            for(int i = 0; i < strInOrder.Count; i++)
            {
                if (!IsTextOverflowing(labels[0]) || !IsTextOverflowing(labels[1]) || !IsTextOverflowing(labels[2]) || !IsTextOverflowing(labels[3]) || !IsTextOverflowing(labels[4]) || !IsTextOverflowing(labels[5]))
                {
                    for (int j = 0; j <= 5; j++)
                    {
                        labels[j].Visibility = Visibility.Visible;
                        separatesOne.Visibility = Visibility.Visible;
                        labels[j].Content += "-----------";
                    }
                    switch (strInOrder[i])
                    {
                        case "1": labels[0].Content += frets[i].ToString(); break;
                        case "2": labels[1].Content += frets[i].ToString(); break;
                        case "3": labels[2].Content += frets[i].ToString(); break;
                        case "4": labels[3].Content += frets[i].ToString(); break;
                        case "5": labels[4].Content += frets[i].ToString(); break;
                        case "6": labels[5].Content += frets[i].ToString(); break;
                    }

                }
                else
                {
                    for (int j = 6; j <= 11; j++)
                    {
                        labels[j].Visibility = Visibility.Visible;
                        separatesTwo.Visibility = Visibility.Visible;
                        labels[j].Content += "-----------";
                    }
                    switch (strInOrder[i])
                    {
                        case "1": labels[6].Content += frets[i].ToString(); break;
                        case "2": labels[7].Content += frets[i].ToString(); break;
                        case "3": labels[8].Content += frets[i].ToString(); break;
                        case "4": labels[9].Content += frets[i].ToString(); break;
                        case "5": labels[10].Content += frets[i].ToString(); break;
                        case "6": labels[11].Content += frets[i].ToString(); break;
                    }
                    if (IsTextOverflowing(labels[6]) || IsTextOverflowing(labels[7]) || IsTextOverflowing(labels[8]) || IsTextOverflowing(labels[9]) || IsTextOverflowing(labels[10]) || IsTextOverflowing(labels[11]))
                    {
                        MessageBox.Show("Unfortunately part of recording didn't fit into display screen");
                    }
                }    
            }
        }

        private void ClearLabels()
        {
            foreach (var label in labels)
            {
                label.Content = label.Name.Substring(label.Name.Length - 1, 1) + "   ";
            }
        }

        private void HideFirstTab()
        {
            for(int i = 0; i <= 5; i++)
            {
                labels[i].Visibility = Visibility.Hidden;
            }
            separatesOne.Visibility = Visibility.Hidden;
        }

        private void HideSecondTab()
        {
            for (int i = 6; i <= 11; i++)
            {
                labels[i].Visibility = Visibility.Hidden;
            }
            separatesTwo.Visibility = Visibility.Hidden;
        }

        private bool IsTextOverflowing(System.Windows.Controls.Label label)
        {
            var formattedText = new System.Windows.Media.FormattedText(
        label.Content.ToString(),
        System.Globalization.CultureInfo.CurrentCulture,
        FlowDirection.LeftToRight,
        new System.Windows.Media.Typeface(label.FontFamily, label.FontStyle, label.FontWeight, label.FontStretch),
        label.FontSize,
        System.Windows.Media.Brushes.Black);

            return formattedText.Width > label.ActualWidth;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.DefaultExt = ".txt";
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";


            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                SaveDataToFile(filePath);

                MessageBox.Show($"File saved to: {filePath}");
            }
            else
            {
                MessageBox.Show("Operation canceled");
            }
        }

        private void SaveDataToFile(string filepath)
        {
            StreamWriter writer = new StreamWriter(filepath);
            for(int i = 0; i < labels.Count; i++)
            {
                string content = labels[i].Content.ToString();
                if(labels[i] != null && content.Length > 5)
                {
                    writer.WriteLine(labels[i].Content);
                }
            }
            writer.Close();
        }
    }
}
