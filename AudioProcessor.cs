using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;
using System.Windows;
using System.Windows.Threading;
using MathNet.Numerics;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using NAudio.Dsp;
using System.Runtime.InteropServices;
using System.Data;

namespace Acoutistic
{
    public class AudioProcessor
    {
        private WaveInEvent waveIn; //event that handles bytes coming from microphone
        public List<float> recordedData; //data captured
        private bool isRecording; //state of program

        private int sampleRate = 44100; //sampling frequency 
        private System.Numerics.Complex[] fftResult; //result of Fast Fourier Transform
        private int rangeOfFourier = 8000; //window to calculate fourier from

        private int samplingPower = 10; //how many times per second operation is gonna take place

        private List<int> indexes; //indexes worth doing fourier for
        public List<double> frequencies; //captured frequencies

        public AudioProcessor()
        {
            recordedData = new List<float>();
            isRecording = false;
        }
        public void StartRecording()
        {
            //Method to initialize gathering data through subscribing to WaveIn_DataAvailable
            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(sampleRate, 1);
            waveIn.StartRecording();
            recordedData.Clear();
            isRecording = true;
        }

        public void StopRecording()
        {
            //well, just unsubscribing
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                isRecording = false;

                ProcessRecordedData();
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (isRecording)
            {
                //new data? ill devour
                recordedData.AddRange(ConvertToFloatArray(e.Buffer));
            }
        }

        private void ProcessRecordedData()
        {
            //troubleshooting
            WaveFormat waveFormat = new WaveFormat(sampleRate, 16, 1);
            string outputPath = "kox.wav";

            using (WaveFileWriter waveWriter = new WaveFileWriter(outputPath, waveFormat))
            {
                byte[] byteData = ConvertToByteArray(recordedData);
                waveWriter.Write(byteData, 0, byteData.Length);
            }
            StreamWriter writer = new StreamWriter("matlabfile.txt");
            string msg = string.Join(" ", recordedData);
            writer.WriteLine(msg);
            writer.Close();

            //ACTUALL PROCESSING
            FourierFromPower();

            //DISPLAY
            string message = string.Join(", ", frequencies);
            //NonBlockingMB.Show($"Gathered Frequencies: {message}");

        }

        private float[] ConvertToFloatArray(byte[] byteArray)
        {
            float[] floatArray = new float[byteArray.Length / 2];
            for (int i = 0; i < floatArray.Length; i++)
            {
                floatArray[i] = BitConverter.ToInt16(byteArray, i * 2) / 32768f;
            }
            return floatArray;
        }

        private byte[] ConvertToByteArray(List<float> floatList)
        {
            byte[] byteArray = new byte[floatList.Count * 2]; // assuming 16-bit PCM encoding

            for (int i = 0; i < floatList.Count; i++)
            {
                short sampleValue = (short)(floatList[i] * short.MaxValue);
                byte[] sampleBytes = BitConverter.GetBytes(sampleValue);
                Array.Copy(sampleBytes, 0, byteArray, i * 2, 2);
            }

            return byteArray;
        }

        private double[] HanningWindow(int length)
        {
            double[] window = new double[length];
            for (int i = 0; i < length; i++)
            {
                window[i] = 0.5 * (1 - Math.Cos(2 * Math.PI * i / (length - 1)));
            }
            return window;
        }

        private int FindDominantIndex(System.Numerics.Complex[] fftResult)
        {
            double maxMagnitude = 0;
            int maxIndex = 0;

            for (int i = 0; i < fftResult.Length / 2; i++)
            {
                //only positive frequencies
                double magnitude = fftResult[i].Magnitude;

                if (magnitude > maxMagnitude)
                {
                    maxMagnitude = magnitude;
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private double FindDominantFrequency(int maxIndex, System.Numerics.Complex[] fftResult)
        {
            return maxIndex * sampleRate / fftResult.Length;
        }

        private void ShowMessageBox(string message)
        {
            //casual non appthread messagebox bc why not
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message);
            });
        }

        private float CalculateEnergy(List<float> signal)
        {
            //As it says, it calculates energy of a signal that are passed to the method
            float energy = 0.0f;
            for(int i = 0; i < signal.Count; i++)
            {
                energy += (float)Math.Pow(signal[i], 2);
            }
            return energy;
        }

        private float CalculatePower(List<float> signal, float duration)
        {
            //Power = Energy / time, here power is in amplitude/samples
            float energy = CalculateEnergy(signal);
            return energy / duration;
        }

        private List<List<float>> DivideSignal()
        {
            //here main signal is changed into subwindows passed to examination

            int windowLength = sampleRate / samplingPower;
            List<List<float>> windows = new List<List<float>>();


            for(int i = 0; i < recordedData.Count - windowLength; i += windowLength)
            {
                List<float> window = new List<float>();
                for (int j = 0; j < windowLength; j++)
                {
                    window.Add(recordedData[i + j]);
                }
                windows.Add(window);
            }
            return windows;
        }

        private List<float> ComputePowerSignal()
        {
            //take these windows and compute power
            int windowLength = sampleRate / samplingPower;
            List<List<float>> signals = DivideSignal();
            List<float> powers = new List<float>();
            foreach(List<float> signal in signals)
            {
                //power in samples
                powers.Add(CalculatePower(signal, windowLength));
            }
            return powers;
        }

        private float CalculateAveragePower()
        {
            return CalculatePower(recordedData, recordedData.Count);
        }

        private List<int> FindIndexesFromPower()
        {
            int windowLength = sampleRate / samplingPower;
            float avg = CalculateAveragePower();
            List<int> indexes = new List<int>();
            List<float> powers = ComputePowerSignal();
            float previousPower = powers[0];
            for(int i = 0; i < powers.Count; i++)
            {
                //based on this super advanced filter just add indexes that we suspect notes
                if(powers[i] > avg * 1.2 && powers[i] > previousPower)
                {
                    indexes.Add(i * windowLength);
                }
                previousPower = powers[i];
            }
            return indexes;
        }

        private void FourierFromPower()
        {
            //pretty self explonatory :)

            indexes = FindIndexesFromPower();
            frequencies = new List<double>();
            fftResult = new System.Numerics.Complex[rangeOfFourier];
            foreach (int peak in indexes)
            {
                //Saving only wanted window of data
                double[] range = new double[rangeOfFourier];
                for (int j = peak, k = 0; j < peak + rangeOfFourier; j++, k++)
                {
                    if (j == recordedData.Count - 1)
                        break;

                    if (j < 0)
                        continue;

                    range[k] = recordedData[j];
                }

                //Applying Hanning window (skyscraper style)
                Complex32[] rawDataPartial = range.Select(x => new Complex32((float)x, 0)).ToArray();
                var windowPartial = HanningWindow(rawDataPartial.Length);
                for (int j = 0; j < rawDataPartial.Length; j++)
                {
                    rawDataPartial[j] = Complex32.Multiply(rawDataPartial[j], (float)windowPartial[j]);
                }
                Fourier.Forward(rawDataPartial, FourierOptions.Matlab);

                //Convinient array copying
                for (int j = 0; j < fftResult.Length; j++)
                {
                    fftResult[j] = new System.Numerics.Complex(rawDataPartial[j].Real, rawDataPartial[j].Imaginary);
                }

                double frequency = FindDominantFrequency(FindDominantIndex(fftResult), fftResult);
                if (frequency < 670)
                {
                    frequencies.Add(frequency);
                }


            }
        }
    }
}
