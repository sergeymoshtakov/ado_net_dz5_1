using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using System.Data;
using second.Views;
using second.DAL.Entity;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace second
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection connection;
        public ObservableCollection<String> columns { get; set; } = new ObservableCollection<String>();
        private DAL.DAO.ProductGroupsDao productGroupDao;
        public ObservableCollection<DAL.Entity.ProductGroup> ProductGroups { get; set; } = new ObservableCollection<DAL.Entity.ProductGroup>();
        public MainWindow()
        {
            InitializeComponent();
            connection = null;
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connection = new SqlConnection(App.ConnectionString);
                connection.Open();
                productGroupDao = new DAL.DAO.ProductGroupsDao(connection);
                loadGroups();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            connection?.Dispose();
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = @"
                CREATE TABLE ProductGroups (
                    Id            UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    Name        NVARCHAR(50)     NOT NULL,
                    Description NTEXT            NOT NULL,
                    Picture     NVARCHAR(50)     NULL
                ) ;";
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Table created");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Create Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InsertGroup_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = @"
                INSERT INTO ProductGroups
                    ( Id, Name,    Description, Picture )
                VALUES
                ( '089015F4-31B5-4F2B-BA05-A813B5419285', N'Інструменти',     N'Ручний інструмент для побутового використання', N'tools.png' ),
                ( 'A6D7858F-6B75-4C75-8A3D-C0B373828558', N'Офісні товари',   N'Декоративні товари для офісного облаштування', N'office.jpg' ),
                ( 'DEF24080-00AA-440A-9690-3C9267243C43', N'Вироби зі скла',  N'Творчі вироби зі скла', N'glass.jpg' ),
                ( '2F9A22BC-43F4-4F73-BAB1-9801052D85A9', N'Вироби з дерева', N'Композиції та декоративні твори з деревини', N'wood.jpg' ),
                ( 'D6D9783F-2182-469A-BD08-A24068BC2A23', N'Вироби з каменя', N'Корисні та декоративні вироби з натурального каменю', N'stone.jpg' )";
            try
            {
                command.ExecuteNonQuery();
                MessageBox.Show("Table data inserted");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Insert Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GroupCount_Click(object sender, RoutedEventArgs e)
        {
            SqlCommand command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = @"SELECT COUNT(*) FROM ProductGroups WHERE DeleteDT IS NULL";
            try
            {
                int cnt = Convert.ToInt32(command.ExecuteScalar());
                MessageBox.Show($"Table has {cnt} rows");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loadGroups() 
        {
            try
            {
                foreach (var product in productGroupDao.getAll())
                {
                    ProductGroups.Add(product);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item) 
            {
                // var group = item.Content as DAL.Entity.ProductGroup;
                // if (group != null) { }
                if(item.Content is DAL.Entity.ProductGroup group)
                {
                    // MessageBox.Show(group.Description);
                    CrudGroupsWindow dialog = new CrudGroupsWindow(group);
                    bool? DialogResult = dialog.ShowDialog();
                    if(DialogResult == true) // Save
                    {
                        if(dialog.restored == true)
                        {
                            if (RestoreProductGroup(dialog.ProductGroup))
                            {
                                MessageBox.Show("Data restored");
                                int index = ProductGroups.IndexOf(group);
                                ProductGroups.Remove(group);
                                ProductGroups.Insert(index, dialog.ProductGroup);
                            }
                        }
                        else if (SaveProductGroup(dialog.ProductGroup))
                        {
                            MessageBox.Show("Data saved");
                            int index = ProductGroups.IndexOf(group);
                            ProductGroups.Remove(group);
                            ProductGroups.Insert(index, dialog.ProductGroup);
                        }
                        else
                        {
                            MessageBox.Show("Error");
                        }
                    }
                    else if (DialogResult == false) 
                    {
                        if(dialog.ProductGroup == null)
                        {
                            if(MessageBox.Show("Do you want to delete?", "Data will be deleted", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                if (DeleteProductGroup(group))
                                {
                                    ProductGroups.Remove(group);
                                    MessageBox.Show("Data deleted"); // Delete
                                }
                                else
                                { MessageBox.Show("Error"); }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Action canceled"); // Close
                        }
                    }
                }
            }
        }

        private bool DeleteProductGroup(DAL.Entity.ProductGroup group)
        {
            try
            {
                productGroupDao.Delete(group);
                return true;
            }
            catch (SqlException ex)
            {
                Title = ex.Message;
                return false;
            }
        }

        private bool SaveProductGroup(DAL.Entity.ProductGroup group)
        {
            try
            {
                productGroupDao.Update(group);
                return true;
            }
            catch (SqlException ex)
            {
                Title = ex.Message;
                return false;
            }
        }

        private bool RestoreProductGroup(DAL.Entity.ProductGroup group)
        {
            try
            {
                productGroupDao.Restore(group);
                return true;
            }
            catch (SqlException ex)
            {
                Title = ex.Message; 
                return false;
            }
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            DAL.Entity.ProductGroup newGroup = new DAL.Entity.ProductGroup()
            {
                Id = Guid.NewGuid(),
            };
            CrudGroupsWindow dialog = new CrudGroupsWindow(newGroup);
            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult ?? true)
            {
                try
                {
                    productGroupDao.Add(newGroup);
                    ProductGroups.Add(newGroup);
                    MessageBox.Show("Data saved");
                }
                catch(Exception ex) 
                {
                    Title = ex.Message;
                    MessageBox.Show("Error");
                }
            }
        }

        private void ShowDeleted_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var product in productGroupDao.getDeleted())
                {
                    ProductGroups.Add(product);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowDeleted_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<DAL.Entity.ProductGroup> myCopy = ProductGroups.ToList();
                foreach (var product1 in myCopy)
                {
                    foreach (var product2 in productGroupDao.getDeleted())
                    {
                        if(product1.Id == product2.Id)
                        {
                            ProductGroups.Remove(product1);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
