using PR18.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PR18.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        public AdminView()
        {
            InitializeComponent();
            LoadAnimals();
        }
        private void LoadAnimals()
        {
            using (var db = new PR18DBEntities())
            {
                AnimalsListBox.ItemsSource = db.Animals.ToList();
            }
        }
        private void Button_AddAnimal(object sender, RoutedEventArgs e)
        {

        }
        private void Button_EditAnimal(object sender, RoutedEventArgs e)
        {

        }
        private void Button_DeleteAnimal(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animals selectedAnimal)
            {
                try
                {
                    using (PR18DBEntities db = new PR18DBEntities())
                    {
                        var animalToDelete = db.Animals.FirstOrDefault(a => a.ID == selectedAnimal.ID);

                        if (animalToDelete != null)
                        {
                            db.Animals.Remove(animalToDelete);

                            db.SaveChanges();

                            MessageBox.Show("Животное успешно удалено!");
                            LoadAnimals();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении из базы данных: {ex.Message}", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите животное из списка для удаления.", "Внимание");
            }
        }
    }
}
