using PR16.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
    public partial class AuthorizedClientView : Window
    {
        private Сотрудник currentUser;

        public ObservableCollection<Товар> Cart { get; set; } = new ObservableCollection<Товар>();

        public AuthorizedClientView(Сотрудник user)
        {
            InitializeComponent();
            currentUser = user;
            ListBoxCart.ItemsSource = Cart;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (currentUser == null)
            {
                ButtonCheckout.IsEnabled = false;
                ButtonCheckout.Content = "Оформить (Доступно клиентам)";
            }
            RefreshData();
        }
        private void DataGridProductsMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedProduct = DataGridProducts.SelectedItem as Товар;
            if (selectedProduct == null) return;

            if (selectedProduct.КолВоНаСкладе <= 0)
            {
                MessageBox.Show("Данного товара нет в наличии на складе!");
                return;
            }

            Cart.Add(selectedProduct);
            UpdateTotalSum();
        }

        private void MenuDeleteFromCartClick(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ListBoxCart.SelectedItem as Товар;
            if (selectedProduct == null) return;

            Cart.Remove(selectedProduct);
            UpdateTotalSum();
        }

        public decimal CalculateTotal(IEnumerable<Товар> items)
        {
            if (items == null || !items.Any()) return 0;

            return items.Sum(t =>
            {
                decimal price = t.Цена ?? 0;
                decimal discountPercent = t.ДействующаяСкидка ?? 0;
                return price * (1 - (discountPercent / 100));
            });
        }

        private void UpdateTotalSum()
        {
            decimal total = CalculateTotal(Cart);
            TextBlockTotal.Text = total.ToString("N2");
        }

        private void ButtonCheckoutClick(object sender, RoutedEventArgs e)
        {
            if (Cart.Count == 0)
            {
                MessageBox.Show("Ваша корзина пуста!");
                return;
            }

            try
            {
                using (var db = new PR16DBEntities())
                {
                    var newOrder = new Заказ
                    {
                        ДатаЗаказа = DateTime.Now,
                        Фамилия = currentUser.Фамилия,
                        Имя = currentUser.Имя,
                        Отчество = currentUser.Отчество,
                        КодДляПолучения = new Random().Next(100, 999),
                        СтатусЗаказа = db.СтатусЗаказа.FirstOrDefault(s => s.Наименование == "Новый")?.Код ?? 1
                    };

                    db.Заказ.Add(newOrder);
                    db.SaveChanges();

                    var groupedCart = Cart.GroupBy(t => t.Артикул);

                    foreach (var group in groupedCart)
                    {
                        int productArtId = group.Key ?? 1;
                        int countInCart = group.Count();

                        var dbProduct = db.Товар.FirstOrDefault(t => t.Артикул == productArtId);
                        if (dbProduct == null || dbProduct.КолВоНаСкладе < countInCart)
                        {
                            MessageBox.Show($"Недостаточно товара на складе для позиции!");
                            return;
                        }

                        dbProduct.КолВоНаСкладе -= countInCart;

                        var orderPos = new СписокЗаказов
                        {
                            КодЗаказа = newOrder.Код,
                            КодАртикула = productArtId,
                            Количество = countInCart
                        };
                        db.СписокЗаказов.Add(orderPos);
                    }

                    db.SaveChanges();
                    MessageBox.Show($"Заказ №{newOrder.Код} успешно оформлен! Код получения: {newOrder.КодДляПолучения}");

                    Cart.Clear();
                    UpdateTotalSum();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка оформления заказа: {ex.Message}");
            }
        }

        private void RefreshData()
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    DataGridProducts.ItemsSource = db.Товар
                        .Include("ТипТовара1")
                        .Include("Производитель1")
                        .Include("Артикул1")
                        .ToList();

                    DataGridOrders.ItemsSource = db.Заказ
                        .Include("СтатусЗаказа1")
                        .Where(z => z.Фамилия == currentUser.Фамилия)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void ButtonCancelOrderClick(object sender, RoutedEventArgs e)
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
