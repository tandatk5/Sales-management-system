using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Sales_management_system
{
    public partial class MainForm : Form
    {
        private int userId;
        private string userName;
        private int roleId;

        public MainForm(int userId, string userName, int roleId)
        {
            InitializeComponent();
            this.userId = userId;
            this.userName = userName;
            this.roleId = roleId;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Xin chào, {userName}!"; // Hiển thị tên người dùng
            ConfigurePermissions(); // Cấu hình quyền theo vai trò
        }

        private void ConfigurePermissions()
        {
            //// Kiểm tra role_id để phân quyền
            //if (roleId == 1) // Admin
            //{
            //}
            //else if (roleId == 2) // Nhân viên bán hàng
            //{
            //    btnDelete.Visible = true; // Ẩn nút xóa nếu là nhân viên
            //}
            //else if (roleId == 3) // Nhân viên kho
            //{
            //    btnReports.Visible = true; // Ẩn nút báo cáo
            //}
            //else if (roleId == 4) // Nhân viên kỹ thuật
            //{
            //    btnInventory.Visible = true; // Ẩn nút liên quan đến kho
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Tạo một instance của Form Quản lý sản phẩm
            ManageProduct manageProductsForm = new ManageProduct();
            // Hiển thị Form Quản lý sản phẩm
            manageProductsForm.ShowDialog();
        }

        private void btnManageCustomer_Click(object sender, EventArgs e)
        {
            // Tạo một instance của Form Quản lý sản phẩm
            ManageCustomer manageCustomerForm = new ManageCustomer();
            // Hiển thị Form Quản lý sản phẩm
            manageCustomerForm.ShowDialog();
        }

        private void btnManageOder_Click(object sender, EventArgs e)
        {
            // Tạo một instance của Form Quản lý sản phẩm
            ManageOrder manageOrderForm = new ManageOrder();
            // Hiển thị Form Quản lý sản phẩm
            manageOrderForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Tạo một instance của Form Quản lý sản phẩm
            RevenueReport StatisticalForm = new RevenueReport();
            // Hiển thị Form Quản lý sản phẩm
            StatisticalForm.ShowDialog();
        }
    }

}
