using PR18.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PR18.Views
{
    /// <summary>
    /// Логика взаимодействия для VisitorView.xaml
    /// </summary>
    public partial class VisitorView : Window
    {
        private MediaPlayer _mediaPlayer = new MediaPlayer();

        public VisitorView()
        {
            InitializeComponent();
            LoadAnimals();
        }

        private void LoadAnimals()
        {
            try
            {
                using (var db = new PR18DBEntities())
                {
                    AnimalsListBox.ItemsSource = db.Animals.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка");
            }
        }

        private void AnimalSound_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Animals selectedAnimal)
            {
                PR18.Services.Mp3Player.PlayMp3FromProject(selectedAnimal.Sound);
            }
        }

        private void AnimalsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animals selectedAnimal)
            {
                _mediaPlayer.Stop();
                AnimalView detailWindow = new AnimalView(selectedAnimal);
                detailWindow.Owner = this;
                detailWindow.ShowDialog();
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
            new AuthorizationView().Show(); 
            this.Close();
        }
    }
}