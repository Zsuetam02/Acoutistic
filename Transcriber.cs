using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoutistic
{
    internal class Transcriber
    {
        private List<double> freq;
        public List<string> stringInOrder;
        public List<int> tabPositionInOrder;
        public Transcriber(List<double> frequencies)
        {
            freq = frequencies;
        }

        public void ExtractNotes()
        {
            stringInOrder = new List<string>();
            tabPositionInOrder = new List<int>();

            string connectionString = "Server=localhost;Database=sys;User Id=root;Password=root";
            string query = "SELECT Str, Freq, Fret FROM singlenotes";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        for (int i = 0; i < freq.Count; i++)
                        {
                            using (MySqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string noteName = reader["Str"].ToString();
                                    double actualFrequency = Convert.ToDouble(reader["Freq"]);
                                    int tabPosition = Convert.ToInt32(reader["Fret"]);

                                    double lowerBound = freq[i] - 3.0;
                                    double upperBound = freq[i] + 3.0;

                                    if (actualFrequency >= lowerBound && actualFrequency <= upperBound)
                                    {
                                        stringInOrder.Add(noteName);
                                        tabPositionInOrder.Add(tabPosition);
                                        break;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
              
            }
        }


    }
}
