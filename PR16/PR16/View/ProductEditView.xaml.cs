using PR16.Models;
using System;
using System.Linq;
using System.Windows;

namespace PR16.View
{
    public partial class ProductEditView : Window
    {
        private Товар currentProduct;
        private bool isEdit = false;

        public ProductEditView(Товар product)
        {
            InitializeComponent();
            currentProduct = product;

            LoadComboBoxes();

            if (currentProduct != null)
            {
                isEdit = true;
                TextBlockHeader.Text = "Редактирование товара";
                FillFields();
            }
            else
            {
                currentProduct = new Товар();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var db = new PR16DBEntities())
                {
                    ComboBarcode.ItemsSource = db.Артикул.ToList();
                    ComboType.ItemsSource = db.ТипТовара.ToList();
                    ComboUnit.ItemsSource = db.ЕдининицыИзмерения.ToList();
                    ComboSupplier.ItemsSource = db.Поставщик.ToList();
                    ComboManufacturer.ItemsSource = db.Производитель.ToList();
                    ComboCategory.ItemsSource = db.КатегорияТовара.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        private void FillFields()
        {
            ComboBarcode.SelectedValue = currentProduct.Артикул;
            ComboType.SelectedValue = currentProduct.ТипТовара;
            ComboUnit.SelectedValue = currentProduct.ЕдиницаИзмерения;
            ComboSupplier.SelectedValue = currentProduct.Поставщик;
            ComboManufacturer.SelectedValue = currentProduct.Производитель;
            ComboCategory.SelectedValue = currentProduct.КатегорияТовара;

            TextBoxPrice.Text = currentProduct.Цена.ToString();
            TextBoxDiscount.Text = currentProduct.ДействующаяСкидка.ToString();
            TextBoxCount.Text = currentProduct.КолВоНаСкладе.ToString();
            TextBoxDescription.Text = currentProduct.ОписаниеТовара;
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields()) return;

            try
            {
                using (var db = new PR16DBEntities())
                {
                    currentProduct.Артикул = (ComboBarcode.SelectedItem as Артикул)?.Код;
                    currentProduct.ТипТовара = (ComboType.SelectedItem as ТипТовара)?.Код;
                    currentProduct.ЕдиницаИзмерения = (ComboUnit.SelectedItem as ЕдининицыИзмерения)?.Код;
                    currentProduct.Поставщик = (ComboSupplier.SelectedItem as Поставщик)?.Код;
                    currentProduct.Производитель = (ComboManufacturer.SelectedItem as Производитель)?.Код;
                    currentProduct.КатегорияТовара = (ComboCategory.SelectedItem as КатегорияТовара)?.Код;

                    currentProduct.Цена = decimal.Parse(TextBoxPrice.Text);
                    currentProduct.ДействующаяСкидка = string.IsNullOrWhiteSpace(TextBoxDiscount.Text) ? 0 : decimal.Parse(TextBoxDiscount.Text);
                    currentProduct.КолВоНаСкладе = int.Parse(TextBoxCount.Text);
                    currentProduct.ОписаниеТовара = TextBoxDescription.Text.Trim();

                    if (isEdit)
                    {
                        db.Entry(currentProduct).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        db.Товар.Add(currentProduct);
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                    DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в базу данных: {ex.Message}");
            }
        }

        private bool ValidateFields()
        {
            if (ComboBarcode.SelectedItem == null || ComboType.SelectedItem == null ||
                ComboUnit.SelectedItem == null || ComboSupplier.SelectedItem == null ||
                ComboManufacturer.SelectedItem == null || ComboCategory.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать все справочные поля!");
                return false;
            }

            if (!decimal.TryParse(TextBoxPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную положительную цену!");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(TextBoxDiscount.Text) && (!decimal.TryParse(TextBoxDiscount.Text, out decimal disc) || disc < 0 || disc > 100))
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100!");
                return false;
            }

            if (!int.TryParse(TextBoxCount.Text, out int count) || count < 0)
            {
                MessageBox.Show("Количество на складе должно быть целым положительным числом!");
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
