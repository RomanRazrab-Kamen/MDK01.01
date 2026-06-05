using PR18.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR18.Services
{
    public class AuthorizeService
    {
        private readonly PR18DBEntities _db;

        public AuthorizeService(PR18DBEntities db) => _db = db;

        public string Login(string login, string password)
        {
            var user = _db.User.FirstOrDefault(u => u.Login == login);
            if (user == null) return "Пользователь не найден";

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
            {
                var timeLeft = user.LockoutEnd.Value - DateTime.Now;
                return $"Вход заблокирован. Подождите {timeLeft.Minutes} мин. {timeLeft.Seconds} сек.";
            }

            if (user.IsApproved != true) return "Ваш аккаунт еще не подтвержден администратором.";

            if (user.Password == password)
            {
                user.FailedAttempts = 0;
                user.LockoutEnd = null;
                _db.SaveChanges();
                return "Success";
            }
            else
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 3)
                {
                    user.LockoutEnd = DateTime.Now.AddMinutes(3); // Блокировка на 3 мин
                    _db.SaveChanges();
                    return "Превышено количество попыток. Вход заблокирован на 3 минуты.";
                }

                _db.SaveChanges();
                return $"Неверный пароль. Осталось попыток: {3 - user.FailedAttempts}";
            }
        }
    }
}
