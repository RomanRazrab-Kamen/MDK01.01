using PR18.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
            LoadUsers();
        }

        private void LoadAnimals()
        {
            using (var db = new PR18DBEntities())
            {
                if (AnimalsListBox != null)
                {
                    AnimalsListBox.ItemsSource = db.Animals.ToList();
                }
            }
        }

        private void Button_AddAnimal(object sender, RoutedEventArgs e)
        {
            AddAnimalView addWindow = new AddAnimalView();
            addWindow.Owner = this;
            if (addWindow.ShowDialog() == true)
            {
                LoadAnimals();
            }
        }

        private void Button_EditAnimal(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animals selectedAnimal)
            {
                EditAnimalView editWindow = new EditAnimalView(selectedAnimal);
                editWindow.Owner = this;

                if (editWindow.ShowDialog() == true)
                {
                    LoadAnimals();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите животное из списка для изменения.", "Предупреждение");
            }
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

        private void LoadUsers()
        {
            try
            {
                using (var db = new PR18DBEntities())
                {
                    UsersDataGrid.ItemsSource = db.User.Where(u => u.Role1.ID == 2).ToList();
                }
            }
            catch { }
        }

        private void ButtonApprove_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is User selectedUser)
            {
                try
                {
                    using (var db = new PR18DBEntities())
                    {
                        var userToUpdate = db.User.FirstOrDefault(u => u.ID == selectedUser.ID);
                        if (userToUpdate != null)
                        {
                            userToUpdate.IsApproved = true;
                            db.SaveChanges();

                            MessageBox.Show($"Пользователь {userToUpdate.Login} успешно одобрен!");
                            LoadUsers();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении: {ex.Message}");
                }
            }
        }

        private void ButtonBlock_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is User selectedUser)
            {
                try
                {
                    using (var db = new PR18DBEntities())
                    {
                        var userToUpdate = db.User.FirstOrDefault(u => u.ID == selectedUser.ID);
                        if (userToUpdate != null)
                        {
                            userToUpdate.IsApproved = !userToUpdate.IsApproved;
                            db.SaveChanges();

                            string status = (bool)userToUpdate.IsApproved ? "заблокирован" : "разблокирован";
                            MessageBox.Show($"Пользователь {userToUpdate.Login} успешно {status}!");
                            LoadUsers();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при блокировке: {ex.Message}");
                }
            }
        }
    }
}
