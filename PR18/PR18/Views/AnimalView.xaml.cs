using PR18.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AnimalView.xaml
    /// </summary>
    public partial class AnimalView : Window
    {
        public Animals CurrentAnimal { get; set; }

        public AnimalView(Animals animal)
        {
            InitializeComponent();

            CurrentAnimal = animal;
            this.DataContext = CurrentAnimal;

            DisplayAnimalImage();
        }

        private void DisplayAnimalImage()
        {
            if (CurrentAnimal?.Image != null && CurrentAnimal.Image.Length > 0)
            {
                try
                {
                    using (MemoryStream memoryStream = new MemoryStream(CurrentAnimal.Image))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = memoryStream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();

                        AnimalImage.Source = bitmap;
                    }
                }
                catch
                {
                    AnimalImage.Source = null;
                }
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
