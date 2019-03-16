using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SalseWindow.xaml
    /// </summary>
    public partial class SalseWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlDataReader dataReader;
        SqlDataReader dataReaderCustomer;
        DataSet dataSet = new DataSet();
        private string username;
        /// <summary>
        /// Check xem đã bán được hàng hay chưa
        /// </summary>
        private bool checkSold = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khỏi tạo mặc định
        /// </summary>
        /// <param name="username"></param>
        public SalseWindow(string username)
        {
            InitializeComponent();
            this.username = username;
            this.txtSaleName.Text = this.username;
            tpGioBan.SelectedTime = DateTime.Now;
            dpNgayBan.SelectedDate = DateTime.Now;
            //Thêm dữ liệu bảo hành
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("3 tháng");
            list.Add("6 tháng");
            list.Add("1 năm");
            list.Add("2 năm");
            list.Add("3 năm");
            list.Add("5 năm");
            this.cbThoiHanBaoHanh.ItemsSource = list;
        }
        #endregion

        #region Events
        /// <summary>
        /// Xử lý hủy phiếu bán hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHuyPhieuBanHang_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Xử lý xuất phiếu bán hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnXuatPhieuBanHang_Click(object sender, RoutedEventArgs e)
        {
            if (checkeAllTextbox())
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            //Thực hiện ghi hóa đơn vào DB
            string querySaveBill = "INSERT INTO PhieuBanHang(ProductID, CustomerID, TenNguoiBan, NgayBan, GiaBan, BaoHanh) " +
                "VALUES(@productID, @customerID, @tenNguoiBan, @ngayBan, @giaBan, @baoHanh)";
            SqlCommand commandSave;
            try
            {
                commandSave = new SqlCommand(querySaveBill, connection);
                commandSave.Parameters.AddWithValue("@productID", this.cbProduct.SelectedValue);
                commandSave.Parameters.AddWithValue("@customerID", this.cbTenKhachHang.SelectedValue);
                commandSave.Parameters.AddWithValue("@tenNguoiBan", this.username);
                commandSave.Parameters.AddWithValue("@ngayBan", DateTime.Now);
                commandSave.Parameters.AddWithValue("@giaBan", this.txtGiaBanHienTai.Text);
                commandSave.Parameters.AddWithValue("@baoHanh", this.cbThoiHanBaoHanh.SelectedValue);
                if (commandSave.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Xuất phiếu hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.checkSold = true;
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Xử lý điền thông tin cho các trường khi người dùng chọn khách hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbTenKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbTenKhachHang.SelectedIndex != -1)
            {
                readDataCustomer();
                //Điền thông tin cho các trường trong Customer
                this.txtCmnd.Text = dataReaderCustomer["Cmnd"].ToString().Trim();
                this.txtSoDienThoai.Text = dataReaderCustomer["SDT"].ToString().Trim();
                this.txtNgaySinh.Text = dataReaderCustomer["NgaySinh"].ToString().Trim();
                this.txtDiaChi.Text = dataReaderCustomer["DiaChi"].ToString().Trim();
                this.txtIDKhachHang.Text = dataReaderCustomer["CustomerID"].ToString();
                dataReaderCustomer.Close();
            }
        }

        /// <summary>
        /// Xử lý điền thông tin product khi người dùng chọn product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CdProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbProduct.SelectedIndex != -1)
            {
                readDataProduct();
                //Điền thông tin cho các trường trong Product
                //Lay mien
                var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
                var money = float.Parse(dataReader["GiaGoc"].ToString().Trim());
                //set gia tien
                this.txtGiaGoc.Text = String.Format(info, "{0:c}", money);
                this.txtManHinh.Text = dataReader["ManHinh"].ToString().Trim();
                this.txtHeDieuHanh.Text = dataReader["HDH"].ToString().Trim();
                this.txtCameraTruoc.Text = dataReader["CameraTruoc"].ToString().Trim();
                this.txtCameraSau.Text = dataReader["CameraSau"].ToString().Trim();
                this.txtCPU.Text = dataReader["CPU"].ToString().Trim();
                this.txtRAM.Text = dataReader["RAM"].ToString().Trim();
                this.txtROM.Text = dataReader["ROM"].ToString().Trim();
                this.txtSim.Text = dataReader["SIM"].ToString().Trim();
                this.txtPin.Text = dataReader["PIN"].ToString().Trim();
                this.txtIDSanPham.Text = this.cbProduct.SelectedValue.ToString();
                this.txtGiaBanHienTai.Text = dataReader["GiaGoc"].ToString().Trim();
                dataReader.Close();
            }

        }

        /// <summary>
        /// Mở kết nối và fill data cho các combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            fillTheComboboxProduct();
            fillTheComboboxCustomer();
        }

        /// <summary>
        /// Xác nhận hủy đơn hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((this.cbTenKhachHang.SelectedIndex != -1) && !this.checkSold || (this.cbProduct.SelectedIndex != -1) && !this.checkSold)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn hủy phiếu hàng này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Chỉ cho phép nhập số
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtGiaBanHienTai_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
        private void fillTheComboboxProduct()
        {
            string query = "SELECT * FROM Product";
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataAdapter.Fill(dataSet, "Product");
                this.cbProduct.ItemsSource = dataSet.Tables["Product"].DefaultView;
                this.cbProduct.DisplayMemberPath = "ProductName";
                this.cbProduct.SelectedValuePath = "ProductID";
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// Điền thông tin vào bảng
        /// </summary>
        private void fillTheComboboxCustomer()
        {
            string query2 = "SELECT * FROM Customer";
            try
            {
                SqlDataAdapter dataAdapter2 = new SqlDataAdapter(query2, connection);
                dataAdapter2.Fill(dataSet, "Customer");
                this.cbTenKhachHang.ItemsSource = dataSet.Tables["Customer"].DefaultView;
                this.cbTenKhachHang.DisplayMemberPath = "Ten";
                this.cbTenKhachHang.SelectedValuePath = "CustomerID";
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// Kiểm tra các điều kiện của textbox
        /// </summary>
        /// <returns></returns>
        private bool checkeAllTextbox()
        {
            return string.IsNullOrEmpty(this.txtGiaGoc.Text) ||
                    string.IsNullOrEmpty(this.txtManHinh.Text) ||
                    string.IsNullOrEmpty(this.txtHeDieuHanh.Text) ||
                    string.IsNullOrEmpty(this.txtCameraSau.Text) ||
                    string.IsNullOrEmpty(this.txtCameraTruoc.Text) ||
                    string.IsNullOrEmpty(this.txtCPU.Text) ||
                    string.IsNullOrEmpty(this.txtRAM.Text) ||
                    string.IsNullOrEmpty(this.txtROM.Text) ||
                    string.IsNullOrEmpty(this.txtSim.Text) ||
                    string.IsNullOrEmpty(this.txtPin.Text) ||
                    this.cbProduct.SelectedIndex == -1 ||
                    this.cbTenKhachHang.SelectedIndex == -1 ||
                    string.IsNullOrEmpty(this.txtCmnd.Text) ||
                    string.IsNullOrEmpty(this.txtNgaySinh.Text) ||
                    string.IsNullOrEmpty(this.txtSoDienThoai.Text) ||
                    string.IsNullOrEmpty(this.txtDiaChi.Text) ||
                    this.cbThoiHanBaoHanh.SelectedIndex == -1 ||
                    string.IsNullOrEmpty(this.txtGiaBanHienTai.Text);
        }

        /// <summary>
        /// Đọc dữ liệu Product từ DB lên
        /// </summary>
        private void readDataProduct()
        {
            try
            {
                SqlCommand command;
                string queryFindSomeUser = "SELECT * FROM Product p JOIN ProductInfo i ON p.ProductID=i.ProductID WHERE p.ProductID=@pId";
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                command = new SqlCommand(queryFindSomeUser, connection);
                command.Parameters.AddWithValue("@pId", this.cbProduct.SelectedValue);
                dataReader = command.ExecuteReader();
                dataReader.Read();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// Đọc dữ liệu Customer từ DB lên
        /// </summary>
        private void readDataCustomer()
        {
            try
            {
                SqlCommand command2;
                string queryFindCustomer = "SELECT * FROM Customer WHERE CustomerID = @cusID";
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                command2 = new SqlCommand(queryFindCustomer, connection);
                command2.Parameters.AddWithValue("@cusID", this.cbTenKhachHang.SelectedValue);
                dataReaderCustomer = command2.ExecuteReader();
                dataReaderCustomer.Read();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        #endregion
    }
}
