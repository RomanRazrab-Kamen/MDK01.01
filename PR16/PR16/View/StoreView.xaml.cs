using PR16.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PR16.View
{
    /// <summary>
    /// Логика взаимодействия для StoreView.xaml
    /// </summary>
    public partial class StoreView : Window
    {
        private Сотрудник currentUser;

        public StoreView(Сотрудник user)
        {
            InitializeComponent();
            currentUser = user;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (currentUser.Роль.Наименование == "Менеджер")
            {
                Title = "Панель Менеджера";
                AdminProductPanel.Visibility = Visibility.Collapsed;
                AdminOrderPanel.Visibility = Visibility.Collapsed;
            }
            else if (currentUser.Роль.Наименование == "Администратор")
            {
                Title = "Панель Администратора";
                AdminProductPanel.Visibility = Visibility.Visible;
                AdminOrderPanel.Visibility = Visibility.Visible;
            }

            LoadSuppliers();
            RefreshData();
        }

        private void LoadSuppliers()
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    var suppliers = db.Поставщик.ToList();
                    suppliers.Insert(0, new Поставщик { Код = 0, Наименование = "Все поставщики" });
                    ComboSupplier.ItemsSource = suppliers;
                    ComboSupplier.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки поставщиков: {ex.Message}");
            }
        }

        private void RefreshData()
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    var query = db.Товар
                        .Include("ТипТовара1")
                        .Include("Производитель1")
                        .Include("Поставщик1")
                        .AsQueryable();

                    if (!string.IsNullOrWhiteSpace(TextBoxSearch.Text))
                    {
                        string search = TextBoxSearch.Text.ToLower().Trim();
                        query = query.Where(t => t.ОписаниеТовара.ToLower().Contains(search));
                    }
                    if (ComboSupplier.SelectedItem is Поставщик selectedSupplier && selectedSupplier.Код > 0)
                    {
                        query = query.Where(t => t.Поставщик == selectedSupplier.Код);
                    }

                    if (ComboSort.SelectedIndex == 1)
                    {
                        query = query.OrderBy(t => t.КолВоНаСкладе);
                    }
                    else if (ComboSort.SelectedIndex == 2)
                    {
                        query = query.OrderByDescending(t => t.КолВоНаСкладе);
                    }

                    DataGridProducts.ItemsSource = query.ToList();

                    DataGridOrders.ItemsSource = db.Заказ
                        .Include("СтатусЗаказа1")
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}");
            }
        }

        private void TextBoxSearchTextChanged(object sender, TextChangedEventArgs e) => RefreshData();
        private void ControlChanged(object sender, SelectionChangedEventArgs e) => RefreshData();

        private void ButtonDeleteProductClick(object sender, RoutedEventArgs e)
        {
            var selectedProduct = DataGridProducts.SelectedItem as Товар;
            if (selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления.");
                return;
            }

            var result = MessageBox.Show($"Вы действительно хотите удалить товар с кодом {selectedProduct.Код}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new PR16DBEntities())
                    {
                        var product = db.Товар.FirstOrDefault(t => t.Код == selectedProduct.Код);
                        if (product != null)
                        {
                            db.Товар.Remove(product);
                            db.SaveChanges();
                            MessageBox.Show("Товар успешно удален.");
                            RefreshData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось удалить товар (возможно, он используется в заказах): {ex.Message}");
                }
            }
        }

        private void ButtonDeleteOrderClick(object sender, RoutedEventArgs e)
        {
            var selectedOrder = DataGridOrders.SelectedItem as Заказ;
            if (selectedOrder == null)
            {
                MessageBox.Show("Выберите заказ для удаления.");
                return;
            }

            var result = MessageBox.Show($"Удалить заказ №{selectedOrder.Код}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new PR16DBEntities())
                    {
                        var order = db.Заказ.FirstOrDefault(z => z.Код == selectedOrder.Код);
                        if (order != null)
                        {
                            var relatedPositions = db.СписокЗаказов.Where(s => s.КодЗаказа == order.Код);
                            db.СписокЗаказов.RemoveRange(relatedPositions);

                            db.Заказ.Remove(order);
                            db.SaveChanges();
                            MessageBox.Show("Заказ успешно удален из системы.");
                            RefreshData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления заказа: {ex.Message}");
                }
            }
        }

        private void ButtonAddProductClick(object sender, RoutedEventArgs e)
        {
            ProductEditView editWindow = new ProductEditView(null);
            if (editWindow.ShowDialog() == true)
            {
                RefreshData();
            }
        }
        private void DataGridProductsMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentUser.Роль.Наименование != "Администратор") return;

            var selectedProduct = DataGridProducts.SelectedItem as Товар;
            if (selectedProduct == null) return;

            ProductEditView editWindow = new ProductEditView(selectedProduct);
            if (editWindow.ShowDialog() == true)
            {
                RefreshData();
            }
        }
        private void ButtonAddOrderClick(object sender, RoutedEventArgs e)
        {
            OrderEditView editWindow = new OrderEditView(null);
            if (editWindow.ShowDialog() == true)
            {
                RefreshData();
            }
        }

        private void DataGridOrdersMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (currentUser.Роль.Наименование != "Администратор") return;

            var selectedOrder = DataGridOrders.SelectedItem as Заказ;
            if (selectedOrder == null) return;

            OrderEditView editWindow = new OrderEditView(selectedOrder);
            if (editWindow.ShowDialog() == true)
            {
                RefreshData();
            }
        }
    }
}
