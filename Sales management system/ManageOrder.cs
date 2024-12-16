using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sales_management_system
{
    public partial class ManageOrder : Form
    {
        private string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

        public ManageOrder()
        {
            InitializeComponent();

            // Initialize ListView columns
            lvOrders.Columns.Add("Order ID", 50, HorizontalAlignment.Left);
            lvOrders.Columns.Add("Order Code", 100, HorizontalAlignment.Left);
            lvOrders.Columns.Add("Customer Name", 150, HorizontalAlignment.Left);
            lvOrders.Columns.Add("User Name", 150, HorizontalAlignment.Left);
            lvOrders.Columns.Add("Total Price", 100, HorizontalAlignment.Right);
            lvOrders.Columns.Add("Created At", 150, HorizontalAlignment.Left);

            lvOrders.FullRowSelect = true;
            lvOrders.View = View.Details;
        }

        private void ManageOrder_Load(object sender, EventArgs e)
        {
            LoadOrdersToListView();
            LoadCustomerComboBox();
            LoadUserComboBox();
        }

        private void LoadOrdersToListView()
        {
            string query = @"
                SELECT o.id, o.order_code, c.name AS customer_name, u.username AS user_name, 
                       o.total_price, o.created_at
                FROM Orders o
                LEFT JOIN Customers c ON o.customer_id = c.id
                LEFT JOIN Users u ON o.user_id = u.id";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    lvOrders.Items.Clear();

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["id"].ToString());
                        item.SubItems.Add(reader["order_code"].ToString());
                        item.SubItems.Add(reader["customer_name"].ToString());
                        item.SubItems.Add(reader["user_name"].ToString());
                        item.SubItems.Add(Convert.ToDecimal(reader["total_price"]).ToString("C", new System.Globalization.CultureInfo("vi-VN")));
                        item.SubItems.Add(Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss"));

                        lvOrders.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading orders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadCustomerComboBox()
        {
            string query = "SELECT id, name FROM Customers";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable customerTable = new DataTable();
                    customerTable.Load(reader);

                    cbCustomer.DataSource = customerTable;
                    cbCustomer.DisplayMember = "name"; // Show customer name
                    cbCustomer.ValueMember = "id"; // Use customer ID as value
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadUserComboBox()
        {
            string query = "SELECT id, username FROM Users";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable userTable = new DataTable();
                    userTable.Load(reader);

                    cbUser.DataSource = userTable;
                    cbUser.DisplayMember = "username"; // Show username
                    cbUser.ValueMember = "id"; // Use user ID as value
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading users: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string orderCode = txtOrderCode.Text.Trim();
                int customerId = Convert.ToInt32(cbCustomer.SelectedValue);
                int userId = Convert.ToInt32(cbUser.SelectedValue);
                decimal totalPrice = Convert.ToDecimal(txtTotalPrice.Text);

                if (string.IsNullOrEmpty(orderCode) || totalPrice <= 0)
                {
                    MessageBox.Show("Order Code and Total Price cannot be empty or zero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "INSERT INTO Orders (order_code, customer_id, user_id, total_price, created_at) VALUES (@OrderCode, @CustomerID, @UserID, @TotalPrice, @CreatedAt)";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrderCode", orderCode);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Order added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrdersToListView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvOrders.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select an order to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string orderId = lvOrders.SelectedItems[0].SubItems[0].Text;
                string orderCode = txtOrderCode.Text.Trim();
                int customerId = Convert.ToInt32(cbCustomer.SelectedValue);
                int userId = Convert.ToInt32(cbUser.SelectedValue);
                decimal totalPrice = Convert.ToDecimal(txtTotalPrice.Text);

                if (string.IsNullOrEmpty(orderCode) || totalPrice <= 0)
                {
                    MessageBox.Show("Order Code and Total Price cannot be empty or zero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string query = "UPDATE Orders SET order_code = @OrderCode, customer_id = @CustomerID, user_id = @UserID, total_price = @TotalPrice, updated_at = @UpdatedAt WHERE id = @OrderID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@OrderCode", orderCode);
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Order updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadOrdersToListView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (lvOrders.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select an order to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string orderId = lvOrders.SelectedItems[0].SubItems[0].Text;

                DialogResult result = MessageBox.Show("Are you sure you want to delete this order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Orders WHERE id = @OrderID";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@OrderID", orderId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Order deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadOrdersToListView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting order: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();
            lvOrders.Items.Clear();

            string query = @"
                SELECT o.id, o.order_code, c.name AS customer_name, u.username AS user_name, 
                       o.total_price, o.created_at
                FROM Orders o
                LEFT JOIN Customers c ON o.customer_id = c.id
                LEFT JOIN Users u ON o.user_id = u.id
                WHERE LOWER(o.order_code) LIKE @SearchText OR LOWER(c.name) LIKE @SearchText";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["id"].ToString());
                        item.SubItems.Add(reader["order_code"].ToString());
                        item.SubItems.Add(reader["customer_name"].ToString());
                        item.SubItems.Add(reader["user_name"].ToString());
                        item.SubItems.Add(Convert.ToDecimal(reader["total_price"]).ToString("C"));
                        item.SubItems.Add(Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss"));

                        lvOrders.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            txtOrderCode.Clear();
            txtTotalPrice.Clear();
            txtCustomerID.Clear();
            txtUserID.Clear();

            // Reset ComboBoxes to their default states
            if (cbCustomer.Items.Count > 0)
                cbCustomer.SelectedIndex = 0; // Select the first item in the ComboBox

            if (cbUser.Items.Count > 0)
                cbUser.SelectedIndex = 0; // Select the first item in the ComboBox

            // Optionally, refocus on the first input field
            txtOrderCode.Focus();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {

        }
    }
}
