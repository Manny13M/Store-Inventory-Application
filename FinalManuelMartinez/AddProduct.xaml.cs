using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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

namespace FinalManuelMartinez
{
    /// <summary>
    /// Interaction logic for AddProduct.xaml
    /// </summary>
    public partial class AddProduct : Window
    {
        public AddProduct()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories() 
        {
            using (SqlConnection conn = new SqlConnection(Data.ConnectionString))
            {
                string queryCategories = "SELECT CategoryName FROM Categories";
                SqlCommand cmdCategories = new SqlCommand(queryCategories, conn);
                conn.Open();

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

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            double price;

            if (!string.IsNullOrEmpty(txtName.Text.ToString()) &&
                double.TryParse(txtPrice.Text, out price) &&
                cmbCategories.SelectedIndex != -1) 
            {
                using (SqlConnection conn = new SqlConnection(Data.ConnectionString))
                {
                    conn.Open();

                    string queryCategory = "SELECT CategoryID FROM Categories WHERE CategoryName = @CategoryName";

                    SqlCommand cmdCategory = new SqlCommand(queryCategory, conn);
                    cmdCategory.Parameters.AddWithValue("CategoryName", cmbCategories.SelectedItem.ToString());

                    int categoryID = (int)cmdCategory.ExecuteScalar();

                    //---------------------------------------------------

                    string query = "INSERT INTO Products (ProductName, UnitPrice, CategoryID) VALUES (@ProductName, @UnitPrice, @CategoryID)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("ProductName", txtName.Text.ToString());
                    cmd.Parameters.AddWithValue("UnitPrice", price);
                    cmd.Parameters.AddWithValue("CategoryID", categoryID);


                    int result = cmd.ExecuteNonQuery();

                    if (result == 1)
                    {
                        MessageBox.Show("Product inserted successfully!");
                        txtName.Text = "";
                        txtPrice.Text = "";
                        cmbCategories.SelectedIndex = -1;
                    }
                    else
                        MessageBox.Show("Product insert failed.");
                }
            }
            else
            {
                MessageBox.Show("Please ensure the following requirement are met.\nAll fields are filled.\nThe price field is a numeric value.");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
