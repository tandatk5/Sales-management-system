using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sales_management_system
{
    public partial class RevenueReport : Form
    {
        // Chuỗi kết nối đến cơ sở dữ liệu
        private string connectionString = @"Data Source=LAPTOP-PSH4U4GU;Initial Catalog=SalesSystem;Integrated Security=True";

        public RevenueReport()
        {
            InitializeComponent();
            ConfigureListView(); // Gọi hàm cấu hình ListView khi khởi tạo form
        }

        private void RevenueReport_Load(object sender, EventArgs e)
        {
            // Tự động load khi form được mở nếu cần
        }

        // Hàm cấu hình các cột cho ListView
        private void ConfigureListView()
        {
            // Đặt chế độ hiển thị chi tiết
            lvStockReport.View = View.Details;
            lvStockReport.Columns.Clear(); // Xóa cột cũ nếu có

            // Thêm các cột vào ListView
            lvStockReport.Columns.Add("Product ID", 100, HorizontalAlignment.Left);
            lvStockReport.Columns.Add("Product Name", 100, HorizontalAlignment.Left);
            lvStockReport.Columns.Add("Stock", 100, HorizontalAlignment.Right);
        }

        // Sự kiện để tạo báo cáo doanh thu
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            // Làm sạch dữ liệu trước đó
            chartRevenue.Series.Clear(); // Xóa dữ liệu cũ trên biểu đồ
            lblTotalRevenue.Text = "Generating...";

            // Lấy ngày bắt đầu và kết thúc từ DateTimePickers
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;

            // Câu lệnh SQL để lấy doanh thu theo ngày và tổng doanh thu
            string query = @"
        SELECT 
            CAST(created_at AS DATE) AS ReportDate, 
            SUM(total_price) AS DailyRevenue
        FROM Orders
        WHERE created_at BETWEEN @StartDate AND @EndDate
        GROUP BY CAST(created_at AS DATE)
        ORDER BY ReportDate;

        SELECT SUM(total_price) AS TotalRevenue
        FROM Orders
        WHERE created_at BETWEEN @StartDate AND @EndDate";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = startDate;
                    cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = endDate;

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Tạo Series cho biểu đồ
                    Series series = new Series("Daily Revenue");
                    series.ChartType = SeriesChartType.Column; // Biểu đồ cột
                    series.XValueType = ChartValueType.DateTime;

                    // Đọc dữ liệu doanh thu theo ngày để vẽ biểu đồ
                    while (reader.Read())
                    {
                        DateTime reportDate = Convert.ToDateTime(reader["ReportDate"]);
                        decimal dailyRevenue = Convert.ToDecimal(reader["DailyRevenue"]);
                        series.Points.AddXY(reportDate, dailyRevenue);
                    }

                    // Đưa Series vào biểu đồ
                    chartRevenue.Series.Add(series);

                    // Đọc kết quả tổng doanh thu (từ truy vấn thứ 2)
                    if (reader.NextResult() && reader.Read())
                    {
                        object totalResult = reader["TotalRevenue"];
                        if (totalResult != DBNull.Value)
                        {
                            decimal totalRevenue = Convert.ToDecimal(totalResult);
                            // Sử dụng định dạng văn hóa Việt Nam để hiển thị tiền tệ
                            lblTotalRevenue.Text = $"Total Revenue: {totalRevenue.ToString("C", CultureInfo.GetCultureInfo("vi-VN"))}";
                        }
                        else
                        {
                            lblTotalRevenue.Text = "No revenue data found for this period.";
                        }
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Sự kiện để kiểm tra số lượng sản phẩm còn lại trong kho
        private void btnCheckStock_Click(object sender, EventArgs e)
        {
            string query = @"
                SELECT 
                    id AS ProductID, 
                    name AS ProductName, 
                    ISNULL(stock, 0) AS Stock 
                FROM Products 
                ORDER BY ProductName";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    lvStockReport.Items.Clear(); // Xóa dữ liệu cũ trong ListView

                    while (reader.Read())
                    {
                        ListViewItem item = new ListViewItem(reader["ProductID"].ToString());
                        item.SubItems.Add(reader["ProductName"].ToString());
                        item.SubItems.Add(reader["Stock"].ToString());
                        lvStockReport.Items.Add(item);
                    }

                    reader.Close();
                    MessageBox.Show("Stock report generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating stock report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lvStockReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Xử lý sự kiện nếu cần
        }

        private void lblTotalRevenue_Click(object sender, EventArgs e)
        {

        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {

        }

        // Thêm các phương thức khác nếu cần
    }
}
