using BCrypt.Net;
using PR18.Services;
using System;
using PR18.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EasyCaptcha.Wpf;

namespace PR18.Views
{
    /// <summary>
    /// Логика взаимодействия для Regisration.xaml
    /// </summary>
    public partial class Regisration : Window
    {
        private PR18DBEntities _db = new PR18DBEntities();
        private string _currentCaptchaText = string.Empty;
        private Random _random = new Random();
        public Regisration()
        {
            InitializeComponent();
        }
        private void GenerateCaptcha()
        {
            try
            {
                var captchaProvider = new EasyCaptcha.Wpf.CaptchaControl();

                captchaProvider.CreateCaptcha(EasyCaptcha.Wpf.NoiseLevel.Medium, 6);

                _currentCaptchaText = captchaProvider.CaptchaText;

                CaptchaImage.Source = captchaProvider.CaptchaImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось обновить капчу: {ex.Message}");
            }
        }

        private void ButtonUpdateCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            TextBoxCaptcha.Text = string.Empty;
        }
        private void ButtonRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (CheckFields())
            {
                string password = TextBoxPassword.Text;
                string hashPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    FirstName = TextBoxName.Text,
                    SecondName = TextBoxSurname.Text,
                    LastName = TextBoxPatronymic.Text,
                    Mail = TextBoxEmail.Text,
                    Password = hashPassword,
                    Login = TextBoxLogin.Text,
                    Role = 2
                };
                _db.User.Add(user);
                _db.SaveChanges();

                Authorization authorization = new Authorization();
                authorization.Show();
                this.Close();
            }
        }
        public bool CheckFields()
        {
            //Поля ФИО
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

            //Логин
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

            //Почта
            if (!string.IsNullOrWhiteSpace(TextBoxEmail.Text))
            {
                bool isValid = Services.Validation.CheckEmail(TextBoxEmail.Text);
                if (!isValid)
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Введите почту");
                return false;
            }

            //Пароль
            if (!string.IsNullOrWhiteSpace(TextBoxPassword.Text))
            {
                bool isValid = Services.Validation.CheckPassword(TextBoxPassword.Text);
                if (!isValid)
                {
                    MessageBox.Show("Пароль не менее 8 символов,  пароль должен содержать хотя бы одну заглавную букву, одну строчную и одну цифру!");
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

        private void ButtonAuthorization_Click(object sender, RoutedEventArgs e)
        {
            Authorization authorization = new Authorization();
            authorization.Show();
            this.Close();
        }
    }
}
