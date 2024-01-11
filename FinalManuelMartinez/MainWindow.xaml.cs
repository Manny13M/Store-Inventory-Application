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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace FinalManuelMartinez
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetAllProducts_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(Data.ConnectionString))
            {
                string query = "Select * FROM Categories cat INNER JOIN Products prod ON cat.CategoryID = prod.CategoryID";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable tblProducts = new DataTable();
                tblProducts.Load(reader);

                grdProducts.ItemsSource = tblProducts.DefaultView;

                //------------------------------------------------

                string queryCategories = "SELECT CategoryName FROM Categories";
                SqlCommand cmdCategories = new SqlCommand(queryCategories, conn);

                SqlDataReader readerCategories = cmdCategories.ExecuteReader();
                List<string> categoryNames = new List<string>();

                while (readerCategories.Read())
                {
                    string categoryName = readerCategories["CategoryName"].ToString();
                    categoryNames.Add(categoryName);
                }

                cmbCategories.ItemsSource = categoryNames;
            }
        }

        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            cmbCategories.ItemsSource = null;
            grdProducts.ItemsSource = null;
            txtName.Text = "";
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtName.Text.ToString())) // Check if an item is selected
            {
                grdProducts.ItemsSource = null;

                using (SqlConnection conn = new SqlConnection(Data.ConnectionString))
                {

                    string query = "SELECT * " +
                                   "FROM Categories cat INNER JOIN Products prod ON cat.CategoryID = prod.CategoryID " +
                                   "WHERE (UPPER(ProductName) LIKE '%' + UPPER(@ProductName) + '%')";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("ProductName", txtName.Text.ToString());

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable tblProduct = new DataTable();
                    tblProduct.Load(reader);

                    grdProducts.ItemsSource = tblProduct.DefaultView;
                }
            }
            else
            {
                MessageBox.Show("No name was specified.");
            }

        }

        private void btnAddNewProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct addProductWindow = new AddProduct();
            addProductWindow.ShowDialog();
        }

        private void cmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCategories.SelectedItem != null) // Check if an item is selected
            {
                grdProducts.ItemsSource = null;

                using (SqlConnection conn = new SqlConnection(Data.ConnectionString))
                {

                    string query = "SELECT * " +
                        "FROM Categories cat INNER JOIN Products prod ON cat.CategoryID = prod.CategoryID " +
                        "WHERE cat.CategoryName = @CategoryName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("CategoryName", cmbCategories.SelectedItem.ToString());

                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable tblCategory = new DataTable();
                    tblCategory.Load(reader);

                    grdProducts.ItemsSource = tblCategory.DefaultView;
                }
            }
        }
    }
}
