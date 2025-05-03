using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Threading;

namespace Acoutistic
{
    internal class Metronome
    {
        SoundPlayer player;
        string filename;
        public static bool isActive;

        //2 neurons are enough to understand what it does so i wont proceed with comments

        public SoundPlayer Player
        {
            get { return player; }
        }

        private string Filename
        {
            get { return filename; }
        }

        public static bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public Metronome(string filename)
        {
            this.filename = filename;
            player = new SoundPlayer(filename);
        }

        public void Play()
        {
            Player.Play();
        }

        public void Loop(int bpm)
        {
            {
                while (true)
                {
                    if (!IsActive)
                    {
                        Play();
                        Thread.Sleep(60000 / bpm);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

    }
}
