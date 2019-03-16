using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for CustomerManagerWindow.xaml
    /// </summary>
    public partial class CustomerManagerWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        private int indexSelected = -1;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command;
        SqlDataReader dataReader;
        DataSet dataSet = new DataSet();
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        public CustomerManagerWindow()
        {
            InitializeComponent();
            //Nạp thông tin cho Combobox
            ObservableCollection<string> list = new ObservableCollection<string>();
            list.Add("Tên khách hàng");
            list.Add("CMND");
            list.Add("Địa chỉ");
            list.Add("Số điện thoại");
            list.Add("Ngày sinh");
            this.cbLocKhachHang.ItemsSource = list;
        }
        #endregion

        #region Events
        /// <summary>
        /// Xử lí khi người dùng click vào grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgCustomer_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgCustomer.Items.IndexOf(this.dgCustomer.CurrentItem) >= 0)
            {
                this.indexSelected = this.dgCustomer.Items.IndexOf(this.dgCustomer.CurrentItem);
                this.txtTenKhachHang.Text = dataSet.Tables[0].Rows[indexSelected]["Ten"].ToString();
                this.txtCmnd.Text = dataSet.Tables[0].Rows[indexSelected]["Cmnd"].ToString();
                this.dpNgaySinh.Text = dataSet.Tables[0].Rows[indexSelected]["NgaySinh"].ToString();
                this.txtDiaChi.Text = dataSet.Tables[0].Rows[indexSelected]["DiaChi"].ToString();
                this.txtDienThoai.Text = dataSet.Tables[0].Rows[indexSelected]["SDT"].ToString();
            }
        }

        /// <summary>
        /// Mở kết nối và điền thông tin vào Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            fillTheDataGrid();
        }

        /// <summary>
        /// Xử lý thêm khách hàng mới
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (IsNullOrEmptyAllTextbox())
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            string queryFindSomeUser = "SELECT * FROM Customer WHERE Cmnd = @cmnd";
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            command = new SqlCommand(queryFindSomeUser, connection);
            command.Parameters.AddWithValue("@cmnd", this.txtCmnd.Text);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows)
            {
                MessageBox.Show("Khách hàng có thông tin CMND đã tồn tại trong hệ thống!\nVui lòng kiểm tra lại!", "Thông báo");
                dataReader.Close();
                return;
            }
            else
            {
                dataReader.Close();
                //Thêm vào db
                string queryAddUser = "INSERT INTO Customer(Ten, NgaySinh, SDT, DiaChi, Cmnd) VALUES(@ten, @ngaySinh, @Sdt, @diaChi, @cmnd)";
                SqlCommand command2;
                try
                {
                    command2 = new SqlCommand(queryAddUser, connection);
                    command2.Parameters.AddWithValue("@ten", this.txtTenKhachHang.Text);
                    command2.Parameters.AddWithValue("@ngaySinh", this.dpNgaySinh.Text);
                    command2.Parameters.AddWithValue("@Sdt", this.txtDienThoai.Text);
                    command2.Parameters.AddWithValue("@diaChi", this.txtDiaChi.Text);
                    command2.Parameters.AddWithValue("@cmnd", this.txtCmnd.Text);
                    if (command2.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Thêm khách hàng thành công!", "Thông báo");
                        fillTheDataGrid();
                        BtnXoaTrang_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Thêm khách hàng thất bại!", "Thông báo");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        /// <summary>
        /// Xử lý cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCapNhat_Click(object sender, RoutedEventArgs e)
        {
            if (indexSelected == -1)
            {
                MessageBox.Show("Bạn chưa chọn đối tượng để cập nhật!");
                return;
            }
            if (IsNullOrEmptyAllTextbox())
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }

            //Thực hiện cập nhật
            string queryUpdate = "UPDATE Customer SET Ten=@ten, NgaySinh=@ngaySinh, SDT = @sdt, DiaChi = @diaChi WHERE Cmnd = @cmnd";
            command = new SqlCommand(queryUpdate, connection);
            try
            {
                command.Parameters.AddWithValue("@ten", this.txtTenKhachHang.Text);
                command.Parameters.AddWithValue("@ngaySinh", this.dpNgaySinh.Text);
                command.Parameters.AddWithValue("@Sdt", this.txtDienThoai.Text);
                command.Parameters.AddWithValue("@diaChi", this.txtDiaChi.Text);
                command.Parameters.AddWithValue("@cmnd", this.txtCmnd.Text);
                if (command.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    fillTheDataGrid();
                    BtnXoaTrang_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Cập nhật khách hàng thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Có lỗi:\n" + err);
            }
            return;
        }

        /// <summary>
        /// Xóa thông tin khách hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (indexSelected == -1)
            {
                MessageBox.Show("Bạn chưa chọn đối tượng để xóa!");
                return;
            }
            var result = MessageBox.Show("Bạn có chắc chắn xóa \"" + dataSet.Tables[0].Rows[indexSelected]["Ten"].ToString() + "\" không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Thực hiện xóa khách hàng
            try
            {
                string queryDelete = "DELETE FROM Customer WHERE Cmnd = @cmnd";
                command = new SqlCommand(queryDelete, connection);
                command.Parameters.AddWithValue("@cmnd", dataSet.Tables[0].Rows[indexSelected]["Cmnd"].ToString());
                if (command.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    fillTheDataGrid();
                    BtnXoaTrang_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Xóa khách hàng thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Xóa trăng các trường
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnXoaTrang_Click(object sender, RoutedEventArgs e)
        {
            this.txtTenKhachHang.Clear();
            this.txtDienThoai.Clear();
            this.txtDiaChi.Clear();
            this.txtCmnd.Clear();
            this.dpNgaySinh.Text = string.Empty;
        }

        /// <summary>
        /// Xử lí lọc khách hàng khi người dùng nhập
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLocKhachHang_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtLocKhachHang.Text;
            if (string.IsNullOrEmpty(text))
            {
                fillTheDataGrid();
            }
            else
            {
                if (this.cbLocKhachHang.SelectedValue.ToString().Equals("Tên khách hàng"))
                {
                    string queryFillName = "SELECT * FROM Customer c WHERE c.Ten LIKE N\'%"+text+"%\'";
                    try
                    {
                        SqlDataAdapter dataAdapterFillName = new SqlDataAdapter(queryFillName, connection);
                        dataSet.Clear();
                        dataAdapterFillName.Fill(dataSet);
                        this.dgCustomer.ItemsSource = dataSet.Tables[0].DefaultView;
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }
                    return;
                }
                if (this.cbLocKhachHang.SelectedValue.ToString().Equals("CMND"))
                {
                    string queryFillName = "SELECT * FROM Customer c WHERE c.Cmnd LIKE N\'%" + text + "%\'";
                    try
                    {
                        SqlDataAdapter dataAdapterFillName = new SqlDataAdapter(queryFillName, connection);
                        dataSet.Clear();
                        dataAdapterFillName.Fill(dataSet);
                        this.dgCustomer.ItemsSource = dataSet.Tables[0].DefaultView;
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }
                    return;
                }
                if (this.cbLocKhachHang.SelectedValue.ToString().Equals("Địa chỉ"))
                {
                    string queryFillName = "SELECT * FROM Customer c WHERE c.DiaChi LIKE N\'%" + text + "%\'";
                    try
                    {
                        SqlDataAdapter dataAdapterFillName = new SqlDataAdapter(queryFillName, connection);
                        dataSet.Clear();
                        dataAdapterFillName.Fill(dataSet);
                        this.dgCustomer.ItemsSource = dataSet.Tables[0].DefaultView;
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }
                    return;
                }
                if (this.cbLocKhachHang.SelectedValue.ToString().Equals("Số điện thoại"))
                {
                    string queryFillName = "SELECT * FROM Customer c WHERE c.SDT LIKE N\'%" + text + "%\'";
                    try
                    {
                        SqlDataAdapter dataAdapterFillName = new SqlDataAdapter(queryFillName, connection);
                        dataSet.Clear();
                        dataAdapterFillName.Fill(dataSet);
                        this.dgCustomer.ItemsSource = dataSet.Tables[0].DefaultView;
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }
                    return;
                }

            }
        }

        /// <summary>
        /// Xử lý các control cho hopwk lí với việc lọc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbLocKhachHang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbLocKhachHang.SelectedIndex != -1)
            {
                if (this.cbLocKhachHang.SelectedValue.ToString().Equals("Ngày sinh"))
                {
                    this.dpFromDate.Visibility = Visibility.Visible;
                    this.dpToDate.Visibility = Visibility.Visible;
                    this.txtLocKhachHang.Visibility = Visibility.Hidden;
                    this.btnLocTheoNgaySinh.Visibility = Visibility.Visible;
                }
                else
                {
                    //Fill lại thông tin chuẩn khi người dùng không còn lọc theo ngày sinh nữa
                    fillTheDataGrid();
                    this.txtLocKhachHang.Visibility = Visibility.Visible;
                    this.dpFromDate.Visibility = Visibility.Hidden;
                    this.dpToDate.Visibility = Visibility.Hidden;
                    this.btnLocTheoNgaySinh.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Xử lý lọc theo ngày sinh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLocTheoNgaySinh_Click(object sender, RoutedEventArgs e)
        {
            //Nếu chọn ngày rồi thì mới lọc không thì thông báo cho người dùng biết họ chưa chọn ngày
            if (this.dpFromDate.SelectedDate.HasValue && this.dpToDate.SelectedDate.HasValue)
            {
                string query = "SELECT * FROM Customer c WHERE c.NgaySinh >= \'" + dpFromDate.Text + "\' AND c.NgaySinh <= \'" + dpToDate.Text + "\'";
                try
                {
                    SqlDataAdapter dataAdapter2 = new SqlDataAdapter(query, connection);
                    dataSet.Clear();
                    dataAdapter2.Fill(dataSet);
                    this.dgCustomer.ItemsSource = dataSet.Tables[0].DefaultView;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa chọn ngày để lọc!\nVui lòng chọn ngày!", "Thông báo");
            }
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
        private void fillTheDataGrid()
        {
            string query = "SELECT * FROM Customer;";
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataSet.Clear();
                dataAdapter.Fill(dataSet);
                this.dgCustomer.ItemsSource = dataSet.Tables[0].DefaultView;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        /// <summary>
        /// Xử lý kiểm tra các textbox có thằng nào null không?
        /// </summary>
        /// <returns></returns>
        private bool IsNullOrEmptyAllTextbox()
        {
            return string.IsNullOrEmpty(this.txtTenKhachHang.Text) ||
                    string.IsNullOrEmpty(this.txtCmnd.Text) ||
                    string.IsNullOrEmpty(this.txtDiaChi.Text) ||
                    string.IsNullOrEmpty(this.txtDienThoai.Text) ||
                    string.IsNullOrEmpty(this.dpNgaySinh.Text);
        }
        #endregion
    }
}
