using PR18.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace PR15Hash.Services
{
    public static class Validation
    {
        public static bool CheckPhoneNumber(string phonenumber)
        {
            string pattern = @"^(\+7|8)[\s\-\(\)]*(\d[\s\-\(\)]*){9}\d$";
            bool isValid = Regex.IsMatch(phonenumber, pattern);
            return isValid;
        }
        public static bool CheckEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if(!Regex.IsMatch(email, pattern))
            {
                MessageBox.Show("Почта введена не коректно формат: (mail@domain.com)");
                return false;
            }
            
            using (var db = new PR18DBEntities())
            {
                bool exists = db.User.Any(u => u.Mail.ToLower() == email.ToLower());
                if (exists)
                {
                    MessageBox.Show("Такая почта уже есть!");
                    return false;
                }
            }
            
            return true;
        }
        public static bool CheckPassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
            bool isValid = Regex.IsMatch(password, pattern);
            return isValid;
        }
        public static bool CheckLogin(string login)
        {
            using (var db = new PR18DBEntities())
            {
                bool exists = db.User.Any(u => u.Login.ToLower() == login.ToLower());
                if (exists)
                    MessageBox.Show("Такой логин уже есть");
                return !exists;
            }
        }
    }
}
