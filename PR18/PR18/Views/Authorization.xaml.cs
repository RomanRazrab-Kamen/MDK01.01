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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR15Hash.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void ButtonRegistration_Click(object sender, RoutedEventArgs e)
        {
            Regisration regisration = new Regisration();
            regisration.Show();
            this.Close();
        }

        private void ButtonAuthorization_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new PR18DBEntities())
                {
                    User user = db.User.FirstOrDefault(x => x.Login == TextBoxLogin.Text);

                    if (user != null)
                    {
                        bool succsedHashPassword = BCrypt.Net.BCrypt.Verify(TextBoxPassword.Text, user.Password);

                        if (succsedHashPassword)
                        {
                            MessageBox.Show("Успешный вход!");
                        }
                        else
                        {
                            MessageBox.Show("Пароль или логин неверный!");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Пароль или логин неверный!");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}