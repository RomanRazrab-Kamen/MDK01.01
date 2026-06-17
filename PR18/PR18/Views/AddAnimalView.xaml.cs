using Microsoft.Win32;
using PR18.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PR18.Views
{
    public partial class AddAnimalView : Window
    {
        private string _selectedFullImagePath = string.Empty;
        private string _selectedFullSoundPath = string.Empty;

        public AddAnimalView()
        {
            InitializeComponent();
        }

        private void ButtonSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFullImagePath = openFileDialog.FileName;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedFullImagePath);

                bitmap.CacheOption = BitmapCacheOption.OnLoad;

                bitmap.EndInit();
                bitmap.Freeze();

                ImgPreview.Source = bitmap;
            }
        }


        private void ButtonSelectSound_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Аудиофайлы (*.mp3)|*.mp3";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFullSoundPath = openFileDialog.FileName;
                TxtSoundPath.Text = Path.GetFileName(_selectedFullSoundPath);
                TxtSoundPath.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Заполните название животного!", "Предупреждение");
                return;
            }

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));

                string imagesFolder = Path.Combine(projectDir, "Data", "Images");
                string soundsFolder = Path.Combine(projectDir, "Data", "Sounds");

                // Безопасное создание папок
                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
                if (!Directory.Exists(soundsFolder)) Directory.CreateDirectory(soundsFolder);

                string savedSoundName = null;
                if (!string.IsNullOrEmpty(_selectedFullSoundPath) && File.Exists(_selectedFullSoundPath))
                {
                    savedSoundName = Path.GetFileName(_selectedFullSoundPath);
                    string targetPath = Path.Combine(soundsFolder, savedSoundName);
                    File.Copy(_selectedFullSoundPath, targetPath, true);
                }

                byte[] imageBytes = null;
                string savedImageName = null;

                if (!string.IsNullOrEmpty(_selectedFullImagePath) && File.Exists(_selectedFullImagePath))
                {
                    savedImageName = Path.GetFileName(_selectedFullImagePath);

                    imageBytes = File.ReadAllBytes(_selectedFullImagePath);
                }
                else
                {
                    imageBytes = new byte[0];
                }

                using (var db = new PR18DBEntities())
                {
                    var newAnimal = new Animals
                    {
                        Name = TxtName.Text.Trim(),
                        Description = TxtDescription.Text.Trim(),
                        InterestingFacts = TxtFact.Text.Trim(),
                        ImageFileName = savedImageName,
                        Sound = savedSoundName,
                        Image = imageBytes
                    };

                    db.Animals.Add(newAnimal);
                    db.SaveChanges();

                    MessageBox.Show("Животное успешно добавлено!", "Успех");
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка");
            }
        }


        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
