using Microsoft.IdentityModel.Tokens;
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

namespace second.Views
{
    /// <summary>
    /// Логика взаимодействия для CrudGroupsWindow.xaml
    /// </summary>
    public partial class CrudGroupsWindow : Window
    {
        public DAL.Entity.ProductGroup ProductGroup { get; set; }
        public string InitialName;
        public string InitialDescription;
        public string InitialPicture;
        public bool enabled = false;
        public bool restored = false;
        public CrudGroupsWindow(DAL.Entity.ProductGroup productGroup)
        {
            InitializeComponent();
            ProductGroup = productGroup;
            this.DataContext = this.ProductGroup;
            InitialName = ProductGroup.Name;
            InitialDescription = ProductGroup.Description;
            InitialPicture = ProductGroup.Picture;
            SaveButton.IsEnabled = enabled;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductGroup.Id.ToString().IsNullOrEmpty() || ProductGroup.Name.IsNullOrEmpty()
                || ProductGroup.Description.IsNullOrEmpty() || ProductGroup.Picture.IsNullOrEmpty()
                || !(ProductGroup.Picture.EndsWith(".jpg") || ProductGroup.Picture.EndsWith(".png")))
            {
                DialogResult = false;
                MessageBox.Show("Wrong input. Data shall be not empty and picture name shall end with .jpg/png");
            }
            else
            {
                DialogResult = true;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ProductGroup = null;
            Close();
        }

        private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            enabled = NameBox.Text != InitialName;
            SaveButton.IsEnabled = enabled;
        }

        private void DescriptionBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            enabled = DescriptionBox.Text != InitialDescription;
            SaveButton.IsEnabled = enabled;
        }

        private void PictureBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            enabled = PictureBox.Text != InitialPicture;
            SaveButton.IsEnabled = enabled;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductGroup.Id.ToString().IsNullOrEmpty() || ProductGroup.Name.IsNullOrEmpty()
                || ProductGroup.Description.IsNullOrEmpty() || ProductGroup.Picture.IsNullOrEmpty()
                || !(ProductGroup.Picture.EndsWith(".jpg") || ProductGroup.Picture.EndsWith(".png")))
            {
                DialogResult = false;
                MessageBox.Show("Wrong input. Data shall be not empty and picture name shall end with .jpg/png");
            }
            else
            {
                DialogResult = true;
                restored = true;
            }
        }
    }
}
