using BCrypt.Net;
using PR16.Models;
using System.Text.RegularExpressions;
using System.Windows;

namespace PR16.View
{
    /// <summary>
    /// Логика взаимодействия для Regisration.xaml
    /// </summary>
    public partial class Regisration : Window
    {
        private PR16DBEntities _db = new PR16DBEntities();
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
               
                var user = new Сотрудник
                {
                    Имя = TextBoxName.Text,
                    Фамилия = TextBoxSurname.Text,
                    Отчество = TextBoxPatronymic.Text,
                    Пароль = hashPassword,
                    Логин = TextBoxLogin.Text,

                };
                _db.Сотрудник.Add(user);
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
