using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xabe.FFmpeg;

namespace VideosConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "MP4 files (*.mp4)|*.mp4|All files (*.*)|*.*"; // Встановлення фільтру для файлів
                openFileDialog.FilterIndex = 1; // Встановлення індексу вибраного фільтру
                openFileDialog.RestoreDirectory = true; // Відновлення каталогу після закриття діалогового вікна

                if (openFileDialog.ShowDialog() == DialogResult.OK) // Якщо користувач вибрав файл та натиснув ОК
                {
                    // Отримаємо шлях до вибраного файлу
                    string selectedFilePath = openFileDialog.FileName;

                    // Запис шляху до файлу в текстове поле
                    textBox1.Text = selectedFilePath;
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string rootDirectory = Directory.GetCurrentDirectory();

            string inputFileName = textBox1.Text; // Вказуємо повний шлях до файлу
            string outputFileName = textBox2.Text;
            
            string crf = textBox3.Text;
            string preset = comboBox1.SelectedItem.ToString();
            string bitrate = textBox4.Text;

            string outputPath = Path.Combine(rootDirectory, outputFileName);

            await ConvertVideo(inputFileName, outputPath, crf, preset, bitrate);
        }

        static async Task ConvertVideo(string inputPath, string outputPath, string crf, string preset, string bitrate)
        {
            try
            {
                FFmpeg.SetExecutablesPath(@"C:\ffmpeg\bin");
                var ffmpeg = FFmpeg.Conversions.New();
                //input
                ffmpeg.AddParameter($"-i {inputPath}");

                //specify and codec
                //ffmpeg.AddParameter("-c:v h264_nvenc");
                ffmpeg.AddParameter("-c:v libx264");

                //crf
                if(int.TryParse(crf, out int value))
                {
                    if(value>=0 && value<=51)
                    {
                        ffmpeg.AddParameter($"-crf {crf}");
                    }
                }
                
                //preset
                ffmpeg.AddParameter($"-preset {preset}");

                //bitrate
                if(int.TryParse(bitrate, out int value2))
                {
                    if(value2>100 && value2<10000)
                    {
                        ffmpeg.AddParameter($"-b:v {bitrate}k");
                    }
                }
                
                //output
                ffmpeg.AddParameter($"{outputPath}");

                MessageBox.Show("Your video will be converted in a few minutes");
                //progress
                ffmpeg.OnProgress += (sender, args) =>
                {
                    double progress = (double)args.Duration.Ticks / TimeSpan.FromSeconds(1).Ticks;
                    Console.WriteLine($"Progress: {progress}");
                };

                await ffmpeg.Start();

                Console.WriteLine("Conversion completed successfully");
                MessageBox.Show("Conversion completed successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong");
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex);
            }
        }
    }
}
