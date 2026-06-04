using PR16.Models;
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
using System.Windows.Shapes;

namespace PR16.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizedClient.xaml
    /// </summary>
    public partial class AuthorizedClient : Window
    {
        private Сотрудник _currentUser;

        public AuthorizedClient(Сотрудник user)
        {
            InitializeComponent();
            _currentUser = user;
            RefreshData();
        }

        private void RefreshData()
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    DataGridProducts.ItemsSource = db.Товар
                        .Include("ТипТовара")
                        .Include("Производитель1")
                        .ToList();

                    DataGridOrders.ItemsSource = db.Заказ
                        .Include("СтатусЗаказа")
                        .Where(z => z.Фамилия == _currentUser.Фамилия)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
            //try
            //{
            //    // Создаем контекст без using, чтобы WPF успел прочитать данные
            //    var db = new PR16DBEntities();

            //    // 1. Проверяем товары
            //    var productsList = db.Товар
            //        .Include("ТипТовара")
            //        .Include("Производитель1")
            //        .ToList();

            //    DataGridProducts.ItemsSource = productsList;

            //    // Отладочное сообщение для товаров
            //    if (productsList.Count == 0)
            //    {
            //        MessageBox.Show("В таблице 'Товар' в базе данных нет записей!");
            //    }

            //    // 2. Проверяем пользователя и заказы
            //    if (_currentUser == null)
            //    {
            //        MessageBox.Show("Ошибка: _currentUser равен null! Авторизация не выполнена.");
            //        return;
            //    }

            //    if (string.IsNullOrWhiteSpace(_currentUser.Фамилия))
            //    {
            //        MessageBox.Show("Ошибка: У текущего пользователя пустая фамилия!");
            //        return;
            //    }

            //    // Очищаем фамилию от возможных пробелов (для типов данных CHAR в БД)
            //    string searchLastName = _currentUser.Фамилия.Trim();

            //    // Загружаем все заказы, чтобы проверить их наличие
            //    var allOrders = db.Заказ.Include("СтатусЗаказа").ToList();

            //    // Фильтруем заказы в памяти C# с игнорированием регистра и пробелов
            //    var filteredOrders = allOrders
            //        .Where(z => z.Фамилия != null && z.Фамилия.Trim().Equals(searchLastName, StringComparison.OrdinalIgnoreCase))
            //        .ToList(); // ВНИМАНИЕ: Проверьте, как в вашей модели называется свойство Фамилия (может быть 'Фамилия' или транслит)

            //    DataGridOrders.ItemsSource = filteredOrders;

            //    // Если в базе заказы есть, но фильтр их отсек, выводим подсказку
            //    if (allOrders.Count > 0 && filteredOrders.Count == 0)
            //    {
            //        string availableNames = string.Join(", ", allOrders.Select(o => $"'{o.Фамилия}'").Distinct().Take(3));
            //        MessageBox.Show($"Заказы в БД есть ({allOrders.Count} шт.), но ни один не подошел под фамилию '{searchLastName}'.\n" +
            //                        $"Примеры фамилий в БД: {availableNames}");
            //    }
            //    else if (allOrders.Count == 0)
            //    {
            //        MessageBox.Show("В таблице 'Заказ' в базе данных вообще нет записей!");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Выводим полную информацию, включая внутреннее исключение базы данных
            //    string inner = ex.InnerException != null ? $"\nВнутренняя ошибка: {ex.InnerException.Message}" : "";
            //    MessageBox.Show($"Ошибка загрузки данных: {ex.Message}{inner}");
            //}
        }

        private void ButtonCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var currentOrder = button?.DataContext as Заказ;

            if (currentOrder == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите отменить заказ №{currentOrder.Код}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new PR16DBEntities())
                    {
                        var orderToDelete = db.Заказ.FirstOrDefault(z => z.Код == currentOrder.Код);

                        if (orderToDelete != null)
                        {
                            var relatedLists = db.СписокЗаказов.Where(s => s.КодЗаказа == orderToDelete.Код);
                            db.СписокЗаказов.RemoveRange(relatedLists);

                            db.Заказ.Remove(orderToDelete);
                            db.SaveChanges();

                            MessageBox.Show("Заказ успешно отменен.");
                            RefreshData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене заказа: {ex.Message}");
                }
            }
        }
    }
}
