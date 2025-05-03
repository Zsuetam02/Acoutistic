using System;
using System.Collections.Generic;
using System.IO.Packaging;
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
using MySql.Data.MySqlClient;

using System.Collections;

namespace Acoutistic
{
    /// <summary>
    /// Interaction logic for KeyChoicePage.xaml
    /// </summary>
    public partial class KeyChoicePage : Page
    {
        public string Information { get; set; }
        public int keyId { get; set; }

        static string ConnectionString = "Server=localhost;Database=sys;User Id=root;Password=root";

        List<string> chordFileNames;
        List<string> chordNames;
        List<string> songs;

        public KeyChoicePage()
        {
            InitializeComponent();

        }

        public void InitializeInformation(string information)
        {
            //NO logic is done here
            Information = information;

            displayLabel.Content = Information;
            NavigateID();
            LoadChordsAndSongs(keyId);
            LoadImagesToPage();
            LoadImagesLabelsToPage();
            LoadSongsToPage();
        }

        private void startingPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartingPage());
        }

        private void NavigateID()
        {
            //what cant be understood, can be hardcoded, youre welcome
            switch(Information)
            {
                case "C major": keyId = 8; break;
                case "D major": keyId = 10; break;
                case "E major": keyId = 12; break;
                case "F major": keyId = 7; break;
                case "G major": keyId = 9; break;
                case "A major": keyId = 11; break;
                case "B major": keyId = 13; break;

                case "C minor": keyId = 20; break;
                case "D minor": keyId = 22; break;
                case "E minor": keyId = 24; break;
                case "F minor": keyId = 19; break;
                case "G minor": keyId = 21; break;
                case "A minor": keyId = 23; break;
                case "B minor": keyId = 25; break;
            }
        }

        private void LoadChordsAndSongs(int keyId)
        {
            //well just fill lists with desired elements from database
            chordNames = new List<string>();
            chordFileNames = new List<string>();
            songs = new List<string>();

            List<int> chordIds = GetChordIdsForKey(keyId);

            foreach (int chordId in chordIds)
            {
                string chordName = GetChordNameById(chordId);
                string chordImageFilename = GetChordImageFilenameById(chordId);

                chordNames.Add(chordName);
                chordFileNames.Add(chordImageFilename);
            }

            songs = GetSongNamesByKeyId(keyId);
        }

        private List<int> GetChordIdsForKey(int keyId)
        {
            //read name of method bozo
            List<int> chordIds = new List<int>();
           
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT ChordId FROM keychord WHERE KeyId = @keyId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@keyId", keyId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chordIds.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            return chordIds;
        }

        private string GetChordNameById(int chordId)
        {
            //same here
            string chordName = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT ChordName FROM chrods WHERE ChordId = @chordId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@chordId", chordId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        chordName = result.ToString();
                    }
                }
            }

            return chordName;
        }

        private string GetChordImageFilenameById(int chordId)
        {
            //do i have to repeat myself
            string imageFilename = null;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT ImageName FROM chrods WHERE ChordId = @chordId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@chordId", chordId);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        imageFilename = result.ToString();
                    }
                }
            }

            return imageFilename;
        }

        private List<string> GetSongNamesByKeyId(int keyId)
        {
            //damn
            List<string> songNames = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT SongName FROM keysongs WHERE KeyId = @keyId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@keyId", keyId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            songNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return songNames;
        }

        private void LoadImagesToPage()
        {
            //very complicated/sophisticated algorithm here, just throwing a rock to a puddle
            imageOne.Source = new BitmapImage(new Uri(chordFileNames[0], UriKind.RelativeOrAbsolute));
            imageTwo.Source = new BitmapImage(new Uri(chordFileNames[1], UriKind.RelativeOrAbsolute));
            imageThree.Source = new BitmapImage(new Uri(chordFileNames[2], UriKind.RelativeOrAbsolute));
            imageFour.Source = new BitmapImage(new Uri(chordFileNames[3], UriKind.RelativeOrAbsolute));
            imageFive.Source = new BitmapImage(new Uri(chordFileNames[4], UriKind.RelativeOrAbsolute));
            imageSix.Source = new BitmapImage(new Uri(chordFileNames[5], UriKind.RelativeOrAbsolute));
            imageSeven.Source = new BitmapImage(new Uri(chordFileNames[6], UriKind.RelativeOrAbsolute));
        }

        private void LoadImagesLabelsToPage()
        {
            //i dont even want to comment these, it too much work
            imageOneLab.Content = chordNames[0];
            imageTwoLab.Content = chordNames[1];
            imageThreeLab.Content = chordNames[2];
            imageFourLab.Content = chordNames[3];
            imageFiveLab.Content = chordNames[4];
            imageSixLab.Content = chordNames[5];
            imageSevenLab.Content = chordNames[6];
        }

        private void LoadSongsToPage()
        {
            ///super triple comment
            songOne.Content = songs[0];
            songTwo.Content = songs[1];
        }
    }
}

