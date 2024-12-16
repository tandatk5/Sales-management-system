using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sales_management_system
{
    public partial class ManageProduct : Form
    {
        public ManageProduct()
        {
            InitializeComponent();
            lvProduct.Columns.Add("ID", 50, HorizontalAlignment.Left);
            lvProduct.Columns.Add("Product Code", 100, HorizontalAlignment.Left);
            lvProduct.Columns.Add("Product Name", 150, HorizontalAlignment.Left);
            lvProduct.Columns.Add("Selling Price", 100, HorizontalAlignment.Right);
            lvProduct.Columns.Add("Category", 150, HorizontalAlignment.Left);
            lvProduct.Columns.Add("Stock", 50, HorizontalAlignment.Left);  // Add Stock column
            lvProduct.Columns.Add("Create at", 150, HorizontalAlignment.Left);


            lvProduct.FullRowSelect = true;
            lvProduct.View = View.Details;

            txtSearch.TextChanged += txtSearch_SelectTextChanged; // Attach event
        }

        private void ManageProduct_Load(object sender, EventArgs e)
        {
            string connectionString = "Server=LAPTOP-PSH4U4GU;Database=SalesSystem;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT p.id, p.product_code, p.name, p.price_sell, p.stock, c.name AS category_name, p.created_at " +
                               "FROM products p " +
                               "LEFT JOIN product_categories c ON p.category_id = c.id";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                lvProduct.Items.Clear(); // Clear old data

                while (reader.Read())
                {
                    ListViewItem item = new ListViewItem(reader["id"].ToString()); // ID
                    item.SubItems.Add(reader["product_code"].ToString());          // Product Code
                    item.SubItems.Add(reader["name"].ToString());                  // Product Name
                    item.SubItems.Add(string.Format("{0:C}", reader["price_sell"])); // Selling Price (currency format)
                    item.SubItems.Add(reader["category_name"].ToString());         // Category
                    item.SubItems.Add(reader["stock"].ToString());                 // Stock
                    item.SubItems.Add(Convert.ToDateTime(reader["created_at"]).ToString("dd/MM/yyyy HH:mm:ss")); // Created at (date-time format)

                    lvProduct.Items.Add(item);
                }
            }

            // Add product categories to ComboBox
            cbCategory.Items.Add("Cake");
            cbCategory.Items.Add("Drink");
            cbCategory.Items.Add("Fruit");

            // Select the first category by default (if needed)
            cbCategory.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Ensure all fields are filled correctly
            string productCode = txtProductCode.Text.Trim();
            string productName = txtProductName.Text.Trim();
            decimal priceSell = 0;

            // Ensure price is a valid decimal value
            if (!decimal.TryParse(txtPriceSell.Text.Trim(), out priceSell))
            {
                MessageBox.Show("Please enter a valid price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string categoryName = cbCategory.SelectedItem.ToString(); // Get value from ComboBox
            int stock = 0;

            // Ensure stock is a valid integer
            if (!int.TryParse(txtStock.Text.Trim(), out stock))
            {
                MessageBox.Show("Please enter a valid stock number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime createdAt = DateTime.Now;

            // Check if all required fields are filled
            if (string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(productName) || priceSell <= 0 || stock < 0)
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // SQL query to insert a new product
            string query = "INSERT INTO products (product_code, name, price_sell, category_id, stock, created_at) " +
                           "VALUES (@productCode, @productName, @priceSell, " +
                           "(SELECT id FROM product_categories WHERE name = @categoryName), @stock, @createdAt)";

            string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@productCode", productCode);
                cmd.Parameters.AddWithValue("@productName", productName);
                cmd.Parameters.AddWithValue("@priceSell", priceSell);
                cmd.Parameters.AddWithValue("@categoryName", categoryName);
                cmd.Parameters.AddWithValue("@stock", stock); // Add stock value
                cmd.Parameters.AddWithValue("@createdAt", createdAt);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery(); // Execute the insert
                    MessageBox.Show("Product has been added successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProductsToListView(); // Refresh the product list in the ListView
                }
                catch (Exception ex)
                {
                    // Display any errors that occur during the insert process
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadProductsToListView()
        {
            // Don't clear the columns each time, only once during initialization
            if (lvProduct.Columns.Count == 0) // Check if columns are already added
            {
                lvProduct.Columns.Add("ID", 50, HorizontalAlignment.Left);
                lvProduct.Columns.Add("Product Code", 100, HorizontalAlignment.Left);
                lvProduct.Columns.Add("Product Name", 150, HorizontalAlignment.Left);
                lvProduct.Columns.Add("Selling Price", 100, HorizontalAlignment.Right);
                lvProduct.Columns.Add("Category", 150, HorizontalAlignment.Left);
                lvProduct.Columns.Add("Stock", 50, HorizontalAlignment.Right);
                lvProduct.Columns.Add("Create at", 150, HorizontalAlignment.Left);  // Add Stock column
            }

            string query = "SELECT p.id, p.product_code, p.name, p.price_sell, p.stock, c.name AS category_name, p.created_at " +
                           "FROM products p " +
                           "LEFT JOIN product_categories c ON p.category_id = c.id";

            string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    lvProduct.Items.Clear(); // Clear old data

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["id"].ToString()); // ID
                        item.SubItems.Add(reader["product_code"].ToString());          // Product Code
                        item.SubItems.Add(reader["name"].ToString());                  // Product Name
                        item.SubItems.Add(string.Format("{0:C}", reader["price_sell"])); // Selling Price (currency format)
                        item.SubItems.Add(reader["category_name"].ToString());         // Category
                        item.SubItems.Add(reader["stock"].ToString());
                        item.SubItems.Add(Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss")); // Created at (date-time format)


                        lvProduct.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lvProduct.SelectedItems.Count > 0)
            {
                string productId = lvProduct.SelectedItems[0].SubItems[0].Text;
                string productCode = txtProductCode.Text;
                string productName = txtProductName.Text;
                decimal priceSell = Convert.ToDecimal(txtPriceSell.Text);
                string categoryName = cbCategory.SelectedItem.ToString(); // Get value from ComboBox
                int stock = Convert.ToInt32(txtStock.Text); // Get stock value

                string query = "UPDATE products SET product_code = @productCode, name = @productName, " +
                               "price_sell = @priceSell, category_id = (SELECT id FROM product_categories WHERE name = @categoryName), " +
                               "stock = @stock WHERE id = @productId";

                string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@productCode", productCode);
                    cmd.Parameters.AddWithValue("@productName", productName);
                    cmd.Parameters.AddWithValue("@priceSell", priceSell);
                    cmd.Parameters.AddWithValue("@stock", stock); // Include stock
                    cmd.Parameters.AddWithValue("@categoryName", categoryName);


                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Product has been updated!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProductsToListView(); // Update data in ListView
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to update!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?",
                                      "Confirmation",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                DeleteProduct();
            }
        }

        private void DeleteProduct()
        {
            // Check if the user has selected a row in the ListView
            if (lvProduct.SelectedItems.Count > 0)
            {
                // Get product ID from the selected row
                string productId = lvProduct.SelectedItems[0].SubItems[0].Text;

                // Connection string
                string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

                // SQL query to delete product
                string query = "DELETE FROM products WHERE id = @id";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", productId);

                    try
                    {
                        conn.Open();

                        // Execute delete command
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // If deleted successfully, remove the selected row from ListView
                            lvProduct.Items.Remove(lvProduct.SelectedItems[0]);
                            MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete the product.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle error
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lvProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvProduct.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvProduct.SelectedItems[0];

                txtProductCode.Text = selectedItem.SubItems[1].Text; // Product Code
                txtProductName.Text = selectedItem.SubItems[2].Text; // Product Name
                txtPriceSell.Text = selectedItem.SubItems[3].Text.Replace("₫", "").Trim(); // Selling Price (remove currency symbol)
                cbCategory.SelectedItem = selectedItem.SubItems[4].Text; // Category
                txtStock.Text = selectedItem.SubItems[5].Text; // Stock (add this line for stock)
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtProductCode.Clear();
            txtProductName.Clear();
            cbCategory.Items.Clear();
            txtPriceSell.Clear();
            txtStock.Clear();  // Clear the stock field
            txtProductCode.Focus();
        }

        private void txtSearch_SelectTextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();
            lvProduct.Items.Clear(); // Clear old items in ListView

            if (string.IsNullOrEmpty(searchText))
            {
                LoadProductsToListView(); // If no search term, show all data
                return;
            }

            string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";
            string query = "SELECT id, name, product_code, price_sell, created_at FROM products WHERE LOWER(name) LIKE @SearchText";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                try
                {
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Flag to check if there are results
                    bool hasResults = false;

                    while (reader.Read())
                    {
                        hasResults = true; // At least one result found
                        ListViewItem item = new ListViewItem(reader["id"].ToString());
                        item.SubItems.Add(reader["name"].ToString());
                        item.SubItems.Add(reader["product_code"].ToString());
                        item.SubItems.Add(reader["price_sell"] == DBNull.Value
                            ? "0"
                            : Convert.ToDecimal(reader["price_sell"]).ToString("N2"));
                        item.SubItems.Add(reader["created_at"] == DBNull.Value
                            ? "N/A"
                            : Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss"));
                        lvProduct.Items.Add(item);
                    }
                    reader.Close();
                    if (!hasResults)
                    {
                        ListViewItem item = new ListViewItem("Not Found!");
                        item.ForeColor = Color.Red; // Highlight with color
                        item.Font = new Font(lvProduct.Font, FontStyle.Italic); // Italic font
                        item.SubItems.Add(""); // Add empty columns for alignment
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        lvProduct.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
