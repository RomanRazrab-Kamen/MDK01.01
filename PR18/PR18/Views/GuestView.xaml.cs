using PR18.Models;
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

namespace PR18.Views
{
    /// <summary>
    /// Логика взаимодействия для GuestView.xaml
    /// </summary>
    public partial class GuestView : Window
    {
        public GuestView()
        {
            InitializeComponent();
            LoadAnimals();
        }

        private void LoadAnimals()
        {
            using (var db = new PR18DBEntities())
            {
                AnimalsListBox.ItemsSource = db.Animals.ToList();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWin = new RegisrationView();
            registerWin.Show();
            
            this.Close();
        }
    }
}
