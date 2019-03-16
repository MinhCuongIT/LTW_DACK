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
    /// Màn hình xem thông tin chi tiết product
    /// </summary>
    public partial class ProductInfoWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command;
        SqlDataReader dataReader;
        private int productID;
        private string productName;
        private float giaGoc;
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="ten"></param>
        /// <param name="gia"></param>
        public ProductInfoWindow(int productID, string ten, string gia)
        {
            InitializeComponent();
            this.productID = productID;
            this.productName = ten;
            this.giaGoc = float.Parse(gia);
        }
        #endregion

        #region Events
        /// <summary>
        /// Xử lý mở kết nối và điền thông tin vào các textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            readData();
            //Đền thông tin vào các textbox
            this.txtProductID.Text = this.productID.ToString();
            this.txtProductName.Text = this.productName;
            var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
            this.txtGiaGoc.Text = String.Format(info, "{0:c}", this.giaGoc);
            //Các textbox thông tin chi tiết
            this.txtManHinh.Text = dataReader["ManHinh"].ToString().Trim();
            this.txtHeDieuHanh.Text = dataReader["HDH"].ToString().Trim();
            this.txtCameraTruoc.Text = dataReader["CameraTruoc"].ToString().Trim();
            this.txtCameraSau.Text = dataReader["CameraSau"].ToString().Trim();
            this.txtCPU.Text = dataReader["CPU"].ToString().Trim();
            this.txtRAM.Text = dataReader["RAM"].ToString().Trim();
            this.txtROM.Text = dataReader["ROM"].ToString().Trim();
            this.txtSim.Text = dataReader["SIM"].ToString().Trim();
            this.txtPin.Text = dataReader["PIN"].ToString().Trim();
        }

        /// <summary>
        /// Xử lý đóng kết nối
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dataReader.Close();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Mở kết nối đến database
        /// </summary>
        private void makeAconnection()
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
        /// Đọc dữ liệu từ DB lên
        /// </summary>
        private void readData()
        {
            try
            {
                string queryFindSomeUser = "SELECT * FROM ProductInfo WHERE ProductID=@pId";
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                command = new SqlCommand(queryFindSomeUser, connection);
                command.Parameters.AddWithValue("@pId", this.productID);
                dataReader = command.ExecuteReader();
                dataReader.Read();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        #endregion
    }
}
