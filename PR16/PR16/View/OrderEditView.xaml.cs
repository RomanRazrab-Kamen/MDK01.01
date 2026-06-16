using PR16.Models;
using System;
using System.Linq;
using System.Windows;

namespace PR16.View
{
    public partial class OrderEditView : Window
    {
        private Заказ currentOrder;
        private bool isEdit = false;

        public OrderEditView(Заказ order)
        {
            InitializeComponent();
            currentOrder = order;

            LoadStatuses();

            if (currentOrder != null)
            {
                isEdit = true;
                TextBlockHeader.Text = $"Редактирование заказа №{currentOrder.Код}";
                FillFields();
            }
            else
            {
                currentOrder = new Заказ { ДатаЗаказа = DateTime.Now };
            }
        }

        private void LoadStatuses()
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    ComboStatus.ItemsSource = db.СтатусЗаказа.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статусов: {ex.Message}");
            }
        }

        private void FillFields()
        {
            TextBoxLastName.Text = currentOrder.Фамилия;
            TextBoxFirstName.Text = currentOrder.Имя;
            TextBoxPickupCode.Text = currentOrder.КодДляПолучения.ToString();
            ComboStatus.SelectedValue = currentOrder.СтатусЗаказа;
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            try
            {
                using (var db = new PR16DBEntities())
                {
                    currentOrder.Фамилия = TextBoxLastName.Text.Trim();
                    currentOrder.Имя = TextBoxFirstName.Text.Trim();
                    currentOrder.КодДляПолучения = int.Parse(TextBoxPickupCode.Text);
                    currentOrder.СтатусЗаказа = (ComboStatus.SelectedItem as СтатусЗаказа)?.Код;

                    if (isEdit)
                    {
                        db.Entry(currentOrder).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        db.Заказ.Add(currentOrder);
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные заказа успешно сохранены!");
                    DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения заказа: {ex.Message}");
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(TextBoxLastName.Text))
            {
                MessageBox.Show("Укажите фамилию клиента!");
                return false;
            }

            if (!int.TryParse(TextBoxPickupCode.Text, out int code) || code <= 0)
            {
                MessageBox.Show("Код пункта выдачи должен быть положительным числом!");
                return false;
            }

            if (ComboStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус заказа!");
                return false;
            }

            return true;
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}