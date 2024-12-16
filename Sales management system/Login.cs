using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sales_management_system
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        // Sự kiện khi nhấn Enter trong TextBox
        private void txtUserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus(); // Chuyển focus qua TextBox mật khẩu
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtlogin_Click(sender, e); // Gọi sự kiện đăng nhập khi nhấn Enter
            }
        }

        private void txtlogin_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=LAPTOP-PSH4U4GU;Database=SalesSystem;Trusted_Connection=True;";
            string username = txtUserName.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, name, role_id FROM users WHERE username = @Username AND password = @Password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int userId = reader.GetInt32(0);  // ID của người dùng
                        string name = reader.GetString(1);  // Tên của người dùng
                        int roleId = reader.GetInt32(2);   // Vai trò của người dùng

                        MessageBox.Show($"Chào mừng {name}!", "Đăng nhập thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Mở form chính và truyền thông tin người dùng
                        MainForm mainForm = new MainForm(userId, name, roleId);
                        mainForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tạo sự kiện KeyDown cho txtUserName và txtPassword
            txtUserName.KeyDown += new KeyEventHandler(txtUserName_KeyDown);
            txtPassword.KeyDown += new KeyEventHandler(txtPassword_KeyDown);
        }
    }
}
