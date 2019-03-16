using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
//1512054 - TranMinhCuong - 22:13 2019-01-12
namespace QL_BanHang
{
    /// <summary>
    /// Màn hình thống kê
    /// </summary>
    public partial class StatisticalWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlDataAdapter adapter;
        DataSet dataSet = new DataSet();
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        public StatisticalWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Xử lý thống kê
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThongKe_Click(object sender, RoutedEventArgs e)
        {
            if (this.dpFromDate.SelectedDate.HasValue && this.dpToDate.SelectedDate.HasValue)
            {
                string queryCalTotal = "SELECT COUNT(*) " +
                                "FROM PhieuBanHang p " +
                                "WHERE p.NgayBan > \'" + dpFromDate.Text + "\' AND p.NgayBan < \'" + dpToDate.Text + "\'";
                SqlCommand commandSum = new SqlCommand(queryCalTotal, connection);
                int totalProduct = (int)commandSum.ExecuteScalar();
                if (totalProduct != 0)
                {
                    this.txtTongSP.Text = totalProduct + " sản phẩm";
                }
                else
                {
                    MessageBox.Show("Không có sản phẩm nào trong khoảng thời gian này!", "Thông báo");
                    return;
                }

                //Thực hiện thống kê
                getData();
                List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
                valueList.Clear();
                for (int i = 0; i < dataSet.Tables["StatistCategory"].Rows.Count; i++)
                {
                    string str = dataSet.Tables["StatistCategory"].Rows[i]["CategoryName"].ToString();
                    int val = int.Parse(dataSet.Tables["StatistCategory"].Rows[i]["M"].ToString());
                    valueList.Add(new KeyValuePair<string, int>(str, val));
                }
                //Vẽ biểu đồ cột
                columnChart.DataContext = valueList;
                //Vẽ biểu đồ tròn
                this.pieChart.DataContext = valueList;
                //------------------//
                try
                {
                    
                    string queryGiaGoc = "SELECT Sum(pr.GiaGoc) " +
                        "FROM PhieuBanHang pbh, Product pr " +
                        "WHERE pbh.NgayBan > \'" + dpFromDate.Text + "\' AND pbh.NgayBan < \'" + dpToDate.Text + "\' AND pr.ProductID = pbh.ProductID";
                    SqlCommand commandGiaGoc = new SqlCommand(queryGiaGoc, connection);
                    var tienGoc = (double)commandGiaGoc.ExecuteScalar();
                    //Lay mien
                    var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
                    //set gia tien
                    this.txtVon.Text = String.Format(info, "{0:c}", tienGoc);
                    //----------------//
                    string queryTienBanDuoc = "Select Sum(p.GiaBan) " +
                        "From PhieuBanHang p " +
                        "WHERE p.NgayBan > \'" + dpFromDate.Text + "\' AND p.NgayBan < \'" + dpToDate.Text + "\'";
                    SqlCommand commandTienBanDuoc = new SqlCommand(queryTienBanDuoc, connection);
                    var tienBanDuoc = (double)commandTienBanDuoc.ExecuteScalar();
                    //Lay mien
                    //set gia tien
                    this.txtBan.Text = String.Format(info, "{0:c}", tienBanDuoc);
                    //----------------//
                    double tienLoiNhuan = tienBanDuoc - tienGoc;
                    //set gia tien
                    this.txtLoiNhuan.Text = String.Format(info, "{0:c}", tienLoiNhuan);
                    //-------Bảng xếp hạng----------//
                    this.dgBXH.ItemsSource = this.dataSet.Tables["BXH"].DefaultView;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Bạn chưa chọn ngày để thống kê!\nVui lòng chọn ngày!", "Thông báo");
            }
        }

        /// <summary>
        /// Mở kết nối
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// Lấy dữ liệu dưới Database lên
        /// </summary>
        private void getData()
        {
            //--------Lấy dữ liệu thống kê cho biểu đồ---------//
            string queryGetData = "SELECT C.CategoryName, T.M " +
                    "FROM (Select c.CategoryID, COUNT(*) As M " +
                    "FROM PhieuBanHang p JOIN Product c ON c.ProductID = p.ProductID " +
                    "WHERE p.NgayBan > \'" + dpFromDate.Text + "\' AND p.NgayBan < \'" + dpToDate.Text + "\' " +
                    "Group by(c.CategoryID)) AS T JOIN Category C ON T.CategoryID = C.CategoryID";
            adapter = new SqlDataAdapter(queryGetData, connection);
            dataSet.Clear();
            adapter.Fill(dataSet, "StatistCategory");

            //-------Bảng xếp hạng----------//
            string queryBXH = "SELECT c.CategoryID, c.CategoryName, N.ProductID, N.ProductName, N.SoLuong " +
                "FROM Category c, (SELECT TOP(10)  p.ProductID, p.CategoryID, p.ProductName, M.SoLuong " +
                "FROM Product p, (Select ProductID, COUNT(*) SoLuong " +
                "From PhieuBanHang pbh " +
                "WHERE pbh.NgayBan > \'"+ dpFromDate.Text + "\' AND pbh.NgayBan < \'"+ dpToDate.Text + "\' " +
                "Group By(ProductID)) AS M " +
                "WHERE p.CategoryID = M.ProductID) AS N " +
                "WHERE N.CategoryID = c.CategoryID " +
                "ORDER BY(N.SoLuong) DESC ";
            SqlDataAdapter adapterBXH = new SqlDataAdapter(queryBXH, connection);
            adapterBXH.Fill(dataSet, "BXH");

        }
        #endregion
    }
}
