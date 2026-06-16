using BCrypt.Net;
using PR16.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PR16.View
{
    /// <summary>
    /// Логика взаимодействия для Regisration.xaml
    /// </summary>
    public partial class RegisrationView : Window
    {
        public RegisrationView()
        {
            InitializeComponent();
        }

        private void ButtonRegistrationClick(object sender, RoutedEventArgs e)
        {
            if (CheckFields())
            {
                try
                {
                    using (var db = new PR16DBEntities())
                    {
                        bool loginExists = db.Сотрудник.Any(x => x.Логин == TextBoxLogin.Text.Trim());
                        if (loginExists)
                        {
                            MessageBox.Show("Пользователь с таким логином уже зарегистрирован!");
                            return;
                        }

                        var clientRole = db.Роль.FirstOrDefault(r => r.Наименование == "Авторизированный клиент");
                        string password = TextBoxPassword.Text;
                        string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);

                        var user = new Сотрудник
                        {
                            Имя = TextBoxName.Text.Trim(),
                            Фамилия = TextBoxSurname.Text.Trim(),
                            Отчество = string.IsNullOrWhiteSpace(TextBoxPatronymic.Text) ? null : TextBoxPatronymic.Text.Trim(),
                            Пароль = hashPassword,
                            Логин = TextBoxLogin.Text.Trim(),
                            РольСотрудника = clientRole.Код
                        };

                        db.Сотрудник.Add(user);
                        db.SaveChanges();

                        MessageBox.Show("Регистрация успешно завершена!");

                        AuthorizationView authorizationView = new AuthorizationView();
                        authorizationView.Show();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
                }
            }
        }

        public bool CheckFields()
        {
            if (string.IsNullOrWhiteSpace(TextBoxSurname.Text))
            {
                MessageBox.Show("Введите фамилию");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TextBoxName.Text))
            {
                MessageBox.Show("Введите имя");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TextBoxLogin.Text))
            {
                bool isValid = Services.Validation.CheckLogin(TextBoxLogin.Text);
                if (!isValid)
                {
                    return isValid;
                }
            }
            else
            {
                MessageBox.Show("Введите логин");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TextBoxPassword.Text))
            {
                bool isValid = Services.Validation.CheckPassword(TextBoxPassword.Text);
                if (!isValid)
                {
                    MessageBox.Show("Пароль не менее 8 символов, пароль должен содержать хотя бы одну заглавную букву, одну строчную и одну цифру!");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Введите пароль");
                return false;
            }

            if (TextBoxPassword.Text != TextBoxConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }

            return true;
        }

        private void ButtonAuthorizationClick(object sender, RoutedEventArgs e)
        {
            AuthorizationView authorization = new AuthorizationView();
            authorization.Show();
            this.Close();
        }
    }
}
