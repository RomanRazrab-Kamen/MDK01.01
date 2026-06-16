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
using PR16.View;

namespace PR16.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationView.xaml
    /// </summary>
    public partial class AuthorizationView : Window
    {
        public AuthorizationView()
        {
            InitializeComponent();
        }

        private void ButtonRegistrationClick(object sender, RoutedEventArgs e)
        {
            // ИСПРАВЛЕНО: Изменено имя класса на RegisrationView
            RegisrationView regisration = new RegisrationView();
            regisration.Show();
            this.Close();
        }

        private void ButtonAuthorizationClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    Сотрудник user = db.Сотрудник
                        .Include("Роль")
                        .FirstOrDefault(x => x.Логин == TextBoxLogin.Text);

                    if (user != null)
                    {
                        bool succsedHashPassword = BCrypt.Net.BCrypt.Verify(TextBoxPassword.Text, user.Пароль);

                        if (succsedHashPassword)
                        {
                            if (user.Роль != null)
                            {
                                if (user.Роль.Наименование == "Авторизированный клиент")
                                {
                                    AuthorizedClientView authorizedClient = new AuthorizedClientView(user);
                                    authorizedClient.Show();
                                    this.Close();
                                }
                                else if (user.Роль.Наименование == "Менеджер" || user.Роль.Наименование == "Администратор")
                                {
                                    // Открываем StoreView. Имя класса совпадает со скриншотом структуры
                                    StoreView storeView = new StoreView(user);
                                    storeView.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("У вашей роли нет доступа к системе.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Ошибка: Пользователю не назначена роль в базе данных.");
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}");
            }
        }

        // ДОБАВЛЕНО: Кнопка для входа в режиме Гостя (согласно ТЗ вашей практической работы)
        private void ButtonGuestClick(object sender, RoutedEventArgs e)
        {
            // Передаем null вместо пользователя — окно само скроет вкладку заказов
            AuthorizedClientView guestClient = new AuthorizedClientView(null);
            guestClient.Show();
            this.Close();
        }
    }
}