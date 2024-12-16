using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sales_management_system
{
    public partial class ManageCustomer : Form
    {
        // Connection string for the database
        private string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

        public ManageCustomer()
        {
            InitializeComponent();

            // Initialize ListView columns
            lvCustomer.Columns.Add("ID", 50, HorizontalAlignment.Left);
            lvCustomer.Columns.Add("Customer Code", 100, HorizontalAlignment.Left);
            lvCustomer.Columns.Add("Name", 150, HorizontalAlignment.Left);
            lvCustomer.Columns.Add("Phone", 100, HorizontalAlignment.Left);
            lvCustomer.Columns.Add("Address", 150, HorizontalAlignment.Left);
            lvCustomer.Columns.Add("Email", 150, HorizontalAlignment.Left);
            lvCustomer.Columns.Add("Created At", 150, HorizontalAlignment.Left);

            lvCustomer.FullRowSelect = true;
            lvCustomer.View = View.Details;
        }

        // Load customers when the form loads
        private void ManageCustomer_Load(object sender, EventArgs e)
        {
            LoadCustomersToListView();
        }

        // Load customers from the database and display them in the ListView
        private void LoadCustomersToListView()
        {
            string query = "SELECT id, customer_code, name, phone, address, email, created_at FROM Customers";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    lvCustomer.Items.Clear(); // Clear existing data in ListView

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["id"].ToString());
                        item.SubItems.Add(reader["customer_code"].ToString());
                        item.SubItems.Add(reader["name"].ToString());
                        item.SubItems.Add(reader["phone"].ToString());
                        item.SubItems.Add(reader["address"].ToString());
                        item.SubItems.Add(reader["email"].ToString());
                        item.SubItems.Add(Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss"));

                        lvCustomer.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Add a new customer
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string customerCode = txtCustomerCode.Text.Trim();
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Kiểm tra dữ liệu đầu vào
            if (!ValidateInput(customerCode, name, phone, address, email))
            {
                return; // Nếu dữ liệu không hợp lệ, dừng thực thi
            }

            string query = "INSERT INTO Customers (customer_code, name, phone, address, email, created_at) " +
                           "VALUES (@CustomerCode, @Name, @Phone, @Address, @Email, @CreatedAt)";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CustomerCode", customerCode);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery(); // Thực thi lệnh thêm mới
                    MessageBox.Show("Customer has been added successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomersToListView(); // Cập nhật lại danh sách khách hàng
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Update an existing customer
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lvCustomer.SelectedItems.Count > 0)
            {
                string id = lvCustomer.SelectedItems[0].SubItems[0].Text;
                string customerCode = txtCustomerCode.Text.Trim();
                string name = txtName.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string address = txtAddress.Text.Trim();
                string email = txtEmail.Text.Trim();

                // Kiểm tra dữ liệu đầu vào
                if (!ValidateInput(customerCode, name, phone, address, email))
                {
                    return; // Nếu dữ liệu không hợp lệ, dừng thực thi
                }

                string query = "UPDATE Customers SET customer_code = @CustomerCode, name = @Name, phone = @Phone, " +
                               "address = @Address, email = @Email, updated_at = @UpdatedAt WHERE id = @ID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@CustomerCode", customerCode);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Customer has been updated successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomersToListView(); // Cập nhật lại danh sách khách hàng
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to update!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete a customer
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvCustomer.SelectedItems.Count > 0)
            {
                string id = lvCustomer.SelectedItems[0].SubItems[0].Text;

                DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string query = "DELETE FROM Customers WHERE id = @ID";

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ID", id);

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery(); // Execute delete command
                            MessageBox.Show("Customer has been deleted successfully!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadCustomersToListView(); // Refresh customer list
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete!", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // When a customer is selected, populate the textboxes with the customer's details
        private void lvCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvCustomer.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvCustomer.SelectedItems[0];

                txtCustomerCode.Text = selectedItem.SubItems[1].Text; // Customer Code
                txtName.Text = selectedItem.SubItems[2].Text; // Customer Name
                txtPhone.Text = selectedItem.SubItems[3].Text; // Customer Phone
                txtAddress.Text = selectedItem.SubItems[4].Text; // Customer Address
                txtEmail.Text = selectedItem.SubItems[5].Text; // Customer Email
            }
        }

        // Search functionality
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower().Trim();
            lvCustomer.Items.Clear(); // Clear old items in ListView

            if (string.IsNullOrEmpty(searchText))
            {
                LoadCustomersToListView(); // If no search term, show all data
                return;
            }

            string query = "SELECT id, customer_code, name, phone, address, email, created_at FROM Customers WHERE LOWER(name) LIKE @SearchText";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    lvCustomer.Items.Clear(); // Clear the listview first

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["id"].ToString());
                        item.SubItems.Add(reader["customer_code"].ToString());
                        item.SubItems.Add(reader["name"].ToString());
                        item.SubItems.Add(reader["phone"].ToString());
                        item.SubItems.Add(reader["address"].ToString());
                        item.SubItems.Add(reader["email"].ToString());
                        item.SubItems.Add(Convert.ToDateTime(reader["created_at"]).ToString("yyyy-MM-dd HH:mm:ss"));

                        lvCustomer.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during search: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {

        }
        private bool ValidateInput(string customerCode, string name, string phone, string address, string email)
        {
            // Kiểm tra các trường rỗng
            if (string.IsNullOrEmpty(customerCode) || string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("All fields are required. Please fill in all the fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra định dạng email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Invalid email format. Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra định dạng số điện thoại (chỉ cho phép số và tối đa 10-12 chữ số)
            string phonePattern = @"^\d{10,12}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, phonePattern))
            {
                MessageBox.Show("Invalid phone number. Please enter a number with 10-12 digits.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Kiểm tra độ dài của địa chỉ (giới hạn 250 ký tự)
            if (address.Length > 250)
            {
                MessageBox.Show("Address is too long. Please enter an address with less than 250 characters.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

    }
}
