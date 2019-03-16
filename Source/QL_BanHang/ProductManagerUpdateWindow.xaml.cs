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
    /// Màn hình cập nhật product
    /// </summary>
    public partial class ProductManagerUpdateWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command;
        SqlDataReader dataReader;
        /// <summary>
        /// Lưu trữ đối giá trị productid
        /// </summary>
        private int idProduct;
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        /// <param name="IdPro"></param>
        public ProductManagerUpdateWindow(int IdPro)
        {
            InitializeComponent();
            this.idProduct = IdPro;
        }
        #endregion

        #region Events
        /// <summary>
        /// Thực thi cập nhật
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            //Kiểm tra người dùng có bỏ sót trường nào không?
            if (CheckAllTextbox())
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            var result = MessageBox.Show("Bạn có chắc chắn muốn cập nhật sản phẩm này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Thực hiện cập nhật
            //Bảng product
            string queryUpdate = "UPDATE Product SET ProductName=@productName, GiaGoc=@giaGoc WHERE ProductID = @productID";
            command = new SqlCommand(queryUpdate, connection);
            try
            {
                command.Parameters.AddWithValue("@productName", this.txtProductName.Text);
                command.Parameters.AddWithValue("@giaGoc", this.txtGiaGoc.Text);
                command.Parameters.AddWithValue("@productID", this.idProduct);
                if (command.ExecuteNonQuery() > 0)
                {
                    //Tiếp tục cập nhật bảng ProductInfo
                    string queryUpdateProductInfo = "UPDATE ProductInfo SET ManHinh=@manHinh, HDH=@hdh, CameraTruoc=@camTruoc, CameraSau=@camSau, CPU=@cpu, RAM=@ram, ROM=@rom, SIM=@sim, PIN=@pin WHERE ProductID = @productID";
                     SqlCommand command2 = new SqlCommand(queryUpdateProductInfo, connection);
                    try
                    {
                        command2.Parameters.AddWithValue("@manHinh", this.txtManHinh.Text);
                        command2.Parameters.AddWithValue("@hdh", this.txtHeDieuHanh.Text);
                        command2.Parameters.AddWithValue("@camTruoc", this.txtCameraTruoc.Text);
                        command2.Parameters.AddWithValue("@camSau", this.txtCameraSau.Text);
                        command2.Parameters.AddWithValue("@cpu", this.txtCPU.Text);
                        command2.Parameters.AddWithValue("@ram", this.txtRAM.Text);
                        command2.Parameters.AddWithValue("@rom", this.txtROM.Text);
                        command2.Parameters.AddWithValue("@sim", this.txtSim.Text);
                        command2.Parameters.AddWithValue("@pin", this.txtPin.Text);
                        command2.Parameters.AddWithValue("@productID", this.idProduct);
                        if (command2.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Có lỗi:\n" + err);
                    }
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Có lỗi:\n" + err);
            }

        }

        /// <summary>
        /// Xử lý điền sẵn thông tin để người dùng cập nhật
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            readData();
            //Đền thông tin vào các textbox
            this.txtProductName.Text = this.dataReader["ProductName"].ToString().Trim();
            this.txtGiaGoc.Text = this.dataReader["GiaGoc"].ToString().Trim();
            this.txtManHinh.Text = dataReader["ManHinh"].ToString().Trim();
            this.txtHeDieuHanh.Text = dataReader["HDH"].ToString().Trim();
            this.txtCameraTruoc.Text = dataReader["CameraTruoc"].ToString().Trim();
            this.txtCameraSau.Text = dataReader["CameraSau"].ToString().Trim();
            this.txtCPU.Text = dataReader["CPU"].ToString().Trim();
            this.txtRAM.Text = dataReader["RAM"].ToString().Trim();
            this.txtROM.Text = dataReader["ROM"].ToString().Trim();
            this.txtSim.Text = dataReader["SIM"].ToString().Trim();
            this.txtPin.Text = dataReader["PIN"].ToString().Trim();
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
        /// Điền thông tin vào bảng
        /// </summary>
        private void readData()
        {
            try
            {
                string queryFindData = "SELECT * FROM Product p JOIN ProductInfo pi ON p.ProductID=pi.ProductID WHERE p.ProductID=@pId";
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                command = new SqlCommand(queryFindData, connection);
                command.Parameters.AddWithValue("@pId", this.idProduct);
                dataReader = command.ExecuteReader();
                dataReader.Read();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// Kiểm tra người dùng bỏ sót trường trắng
        /// </summary>
        /// <returns></returns>
        private bool CheckAllTextbox()
        {
            return string.IsNullOrEmpty(this.txtProductName.Text) ||
                    string.IsNullOrEmpty(this.txtGiaGoc.Text) ||
                    string.IsNullOrEmpty(this.txtManHinh.Text) ||
                    string.IsNullOrEmpty(this.txtHeDieuHanh.Text) ||
                    string.IsNullOrEmpty(this.txtCameraTruoc.Text) ||
                    string.IsNullOrEmpty(this.txtCameraSau.Text) ||
                    string.IsNullOrEmpty(this.txtCPU.Text) ||
                    string.IsNullOrEmpty(this.txtRAM.Text) ||
                    string.IsNullOrEmpty(this.txtROM.Text) ||
                    string.IsNullOrEmpty(this.txtSim.Text) ||
                    string.IsNullOrEmpty(this.txtPin.Text);
        }
        #endregion
    }
}
