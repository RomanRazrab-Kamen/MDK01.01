using BCrypt.Net;
using PR18.Models;
using System.Text.RegularExpressions;
using System.Windows;

namespace PR15Hash.Views
{
    /// <summary>
    /// Логика взаимодействия для Regisration.xaml
    /// </summary>
    public partial class Regisration : Window
    {
        private PR18DBEntities _db = new PR18DBEntities();
        public Regisration()
        {
            InitializeComponent();
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
                    Role = 1
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
