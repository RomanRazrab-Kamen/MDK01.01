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
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : Window
    {
        private Сотрудник _currentUser;
        public AdminView(Сотрудник user)
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
                        .AsEnumerable()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
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
