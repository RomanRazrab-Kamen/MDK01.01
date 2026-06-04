using System;
using System.Collections.Generic;
using PR16.Models;
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

namespace PR16.View
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
                using (var db = new PR16DBEntities())
                {
                    Сотрудник user = db.Сотрудник.FirstOrDefault(x => x.Логин == TextBoxLogin.Text);

                    if (user != null)
                    {
                        bool succsedHashPassword = BCrypt.Net.BCrypt.Verify(TextBoxPassword.Text, user.Пароль);

                        if (succsedHashPassword)
                        {
                            if(user.Роль.Наименование == "Авторизированный клиент")
                            {
                                AuthorizedClient authorizedClient = new AuthorizedClient(user);
                                authorizedClient.Show();
                                this.Close();
                            }
                            if (user.Роль.Наименование == "Менеджер")
                            {

                            }
                            if (user.Роль.Наименование == "Администратор")
                            {
                                AdminView admin = new AdminView(user);
                                admin.Show();
                                this.Close();
                            }
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