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

namespace PR18.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class AuthorizationView : Window
    {
        public AuthorizationView()
        {
            InitializeComponent();
        }

        private void ButtonRegistration_Click(object sender, RoutedEventArgs e)
        {
            RegisrationView RegisrationView = new RegisrationView();
            RegisrationView.Show();
            this.Close();
        }
        private void ButtonCountineAsGuest_Click(object sender, RoutedEventArgs e)
        {
            GuestView guestView = new GuestView();
            guestView.Show();
            this.Close();
        }
        private void ButtonAuthorizationView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new PR18DBEntities())
                {
                    User user = db.User.FirstOrDefault(x => x.Login == TextBoxLogin.Text.Trim());

                    if (user == null)
                    {
                        MessageBox.Show("Пароль или логин неверный!");
                        return;
                    }

                    if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
                    {
                        var remainingTime = user.LockoutEnd.Value - DateTime.Now;
                        MessageBox.Show($"Превышено количество попыток ввода. Доступ заблокирован!\n" + $"Осталось: {remainingTime.Minutes} мин. {remainingTime.Seconds} сек.");
                        return;
                    }

                    bool succsedHashPassword = BCrypt.Net.BCrypt.Verify(TextBoxPassword.Text, user.Password);

                    if (!succsedHashPassword)
                    {
                        user.FailedAttempts = (user.FailedAttempts ?? 0) + 1;

                        if (user.FailedAttempts >= 3)
                        {
                            user.LockoutEnd = DateTime.Now.AddMinutes(3);
                            db.SaveChanges();
                            MessageBox.Show("Вы ввели неправильный пароль 3 раза. Вход заблокирован на 3 минуты!");
                        }
                        else
                        {
                            db.SaveChanges();
                            int attemptsLeft = 3 - user.FailedAttempts.Value;
                            MessageBox.Show($"Пароль или логин неверный! Осталось попыток до блокировки: {attemptsLeft}");
                        }
                        return;
                    }

                    if (user.IsApproved != true)
                    {
                        MessageBox.Show("Ваш аккаунт еще не подтвержден администратором. Ожидайте активации.");
                        return;
                    }

                    user.FailedAttempts = 0;
                    user.LockoutEnd = null;
                    db.SaveChanges();

                    if (user.Role == 1)
                    {
                        AdminView adminView = new AdminView();
                        adminView.Show();
                    }
                    else if (user.Role == 2)
                    {
                        VisitorView visitorView = new VisitorView();
                        visitorView.Show();
                    }
                    else
                    {
                        MessageBox.Show("Неизвестная роль пользователя в системе.");
                        return;
                    }

                    this.Close(); // Закрываем форму авторизации после успешного входа
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}