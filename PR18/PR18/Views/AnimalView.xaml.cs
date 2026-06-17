using PR18.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace PR18.Views
{
    /// <summary>
    /// Логика взаимодействия для AnimalView.xaml
    /// </summary>
    public partial class AnimalView : Window
    {
        public Animals CurrentAnimal { get; set; }

        private MediaPlayer _mediaPlayer = new MediaPlayer();

        public AnimalView(Animals animal)
        {
            InitializeComponent();

            CurrentAnimal = animal;
            this.DataContext = CurrentAnimal;

            DisplayAnimalImage();

            PlayAnimalSound();
        }

        private void DisplayAnimalImage()
        {
            if (CurrentAnimal?.Image != null && CurrentAnimal.Image.Length > 0)
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(CurrentAnimal.Image))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = memoryStream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        AnimalImage.Source = bitmap;
                    }
                }
                catch
                {
                    AnimalImage.Source = null;
                }
            }
        }

        private void AnimalImage_Click(object sender, RoutedEventArgs e)
        {
            PlayAnimalSound();
        }

        private void PlayAnimalSound()
        {
            if (CurrentAnimal != null && !string.IsNullOrEmpty(CurrentAnimal.Sound))
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));
                    string fullSoundPath = Path.Combine(projectDir, "Data", "Sounds", CurrentAnimal.Sound);

                    if (File.Exists(fullSoundPath))
                    {
                        _mediaPlayer.Open(new Uri(fullSoundPath));
                        _mediaPlayer.Play();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось воспроизвести звук: {ex.Message}", "Ошибка звука");
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            this.Close();
        }
    }
}