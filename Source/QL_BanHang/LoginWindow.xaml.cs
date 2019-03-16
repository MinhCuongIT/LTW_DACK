using System;
using System.Collections.Generic;
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
    /// Màn hình đăng nhập
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến database
        /// </summary>
        public static string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=QL_BanHang;Integrated Security=True";
        /// <summary>
        /// Kết nối tới database
        /// </summary>
        private SqlConnection connection = new SqlConnection(connectionString);
        private string queryLogin;
        private SqlCommand command;
        /// <summary>
        /// Lưu trữ tên đăng nhập
        /// </summary>
        private string username = string.Empty;
        /// <summary>
        /// Có phải là admin hay không?
        /// </summary>
        private bool isAdmin;
        #endregion

        #region Properties
        /// <summary>
        /// Lấy tên đăng nhập
        /// </summary>
        public string GetUsername => username;
        /// <summary>
        /// Có phải là Admin hay không?
        /// </summary>
        public bool IsAdmin { get => isAdmin;}
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Xử lí nút đăng nhập
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtUsername.Text == "" || this.txtPassword.Password == "")
            {
                MessageBox.Show("Tên tài khoản và mật khẩu không được rỗng!", "Thông báo");
                return;
            }
            try
            {
                queryLogin = "SELECT * FROM USERS WHERE UserName=@username AND Password=@pass";
                command = new SqlCommand(queryLogin, connection);
                command.Parameters.AddWithValue("@username", this.txtUsername.Text);
                command.Parameters.AddWithValue("@pass", this.txtPassword.Password);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.username = this.txtUsername.Text;
                    this.isAdmin = reader["UserType"].Equals(true);
                    //Vào Dashboard
                    MainWindow dashboard = new MainWindow(this);
                    dashboard.Show();
                    this.Hide();
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    MessageBox.Show("Tên tài khoản hoặc mật khẩu không đúng!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sự cố kết nối\n" + ex.Message);
            }
        }

        /// <summary>
        /// Mở kết nối nếu kết nối là đóng
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
                connection.Close();
            }
            this.txtUsername.Focus();
        }

        /// <summary>
        /// Nhấn enter để đăng nhập cho nhanh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(null, null);
            }
        }

        /// <summary>
        /// Nhấn enter để đăng nhập cho nhanh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(null, null);
            }
        }
        #endregion
    }
}
