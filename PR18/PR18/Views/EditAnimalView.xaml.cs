using Microsoft.Win32;
using PR18.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PR18.Views
{
    public partial class EditAnimalView : Window
    {
        private Animals _currentAnimal;
        private string _selectedFullImagePath = string.Empty;
        private string _selectedFullSoundPath = string.Empty;
        private bool _isImageChanged = false;
        private bool _isSoundChanged = false;

        public EditAnimalView(Animals animal)
        {
            InitializeComponent();
            _currentAnimal = animal;
            this.DataContext = _currentAnimal;
        }

        private void ButtonSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFullImagePath = openFileDialog.FileName;
                _isImageChanged = true;

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedFullImagePath);
                bitmap.EndInit();
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
                _isSoundChanged = true;
                TxtSoundPath.Text = Path.GetFileName(_selectedFullSoundPath);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Название животного не может быть пустым!", "Предупреждение");
                return;
            }

            try
            {
                using (var db = new PR18DBEntities())
                {
                    var animalInDb = db.Animals.FirstOrDefault(a => a.ID == _currentAnimal.ID);

                    if (animalInDb != null)
                    {
                        animalInDb.Name = TxtName.Text.Trim();
                        animalInDb.Description = TxtDescription.Text.Trim();
                        animalInDb.InterestingFacts = TxtFact.Text.Trim();

                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\"));

                        if (_isSoundChanged && !string.IsNullOrEmpty(_selectedFullSoundPath))
                        {
                            string soundsFolder = Path.Combine(projectDir, "Data", "Sounds");
                            if (!Directory.Exists(soundsFolder)) Directory.CreateDirectory(soundsFolder);

                            string soundName = Path.GetFileName(_selectedFullSoundPath);
                            File.Copy(_selectedFullSoundPath, Path.Combine(soundsFolder, soundName), true);

                            animalInDb.Sound = soundName;
                        }

                        if (_isImageChanged && !string.IsNullOrEmpty(_selectedFullImagePath))
                        {
                            animalInDb.ImageFileName = Path.GetFileName(_selectedFullImagePath);
                            animalInDb.Image = File.ReadAllBytes(_selectedFullImagePath);
                        }

                        db.SaveChanges();
                        MessageBox.Show("Данные животного успешно обновлены!", "Успех");
                        this.DialogResult = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
