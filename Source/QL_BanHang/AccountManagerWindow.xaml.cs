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
    /// Màn hình quản lý tài khoản
    /// </summary>
    public partial class AccountManagerWindow : Window
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
        public AccountManagerWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Lấy vị trí grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgAccounts_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgAccounts.Items.IndexOf(this.dgAccounts.CurrentItem) >= 0)
            {
                this.indexSelected = this.dgAccounts.Items.IndexOf(this.dgAccounts.CurrentItem);
                this.txtUsername.Text = dataSet.Tables[0].Rows[indexSelected]["Username"].ToString();
                this.txtPassword.Text = dataSet.Tables[0].Rows[indexSelected]["Password"].ToString();
                bool b = dataSet.Tables[0].Rows[indexSelected]["UserType"].Equals(true);
                this.chkAdmin.IsChecked = b;
            }
        }

        /// <summary>
        /// Xử lý thêm tài khoản
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtUsername.Text) || string.IsNullOrEmpty(this.txtPassword.Text))
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            string queryFindSomeUser = "SELECT * FROM Users WHERE UserName = @userName";
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            command = new SqlCommand(queryFindSomeUser, connection);
            command.Parameters.AddWithValue("@userName", this.txtUsername.Text);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows)
            {
                MessageBox.Show("Tài khoản đã tồn tại! Vui lòng chọn một tài khoản khác", "Thông báo");
                dataReader.Close();
                return;
            }
            else
            {
                dataReader.Close();
                //Thực hiện thêm tài khoản vào DB
                string queryAddUser = "INSERT INTO Users(UserName, Password, UserType) VALUES(@username,@passsword,@type)";
                SqlCommand command2;
                try
                {
                    command2 = new SqlCommand(queryAddUser, connection);
                    command2.Parameters.AddWithValue("@username", this.txtUsername.Text);
                    command2.Parameters.AddWithValue("@passsword", this.txtPassword.Text);
                    command2.Parameters.AddWithValue("@type", getAcountType());
                    if (command2.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Thêm tài khoản thành công!", "Thông báo");
                        fillTheDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Thêm tài khoản thất bại!", "Thông báo");
                    }

                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        /// <summary>
        /// Xử lý cập nhật tài khoản
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
            if (string.IsNullOrEmpty(this.txtUsername.Text) || string.IsNullOrEmpty(this.txtPassword.Text))
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            string queryFindSomeUser = "SELECT * FROM Users WHERE UserName = @userName";
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command3;
            command3 = new SqlCommand(queryFindSomeUser, connection);
            command3.Parameters.AddWithValue("@userName", this.txtUsername.Text);
            dataReader = command3.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows)
            {
                dataReader.Close();
                //Thực hiện cập nhật
                var result = MessageBox.Show("Bạn có chắc chắn cập nhật \"" + dataSet.Tables[0].Rows[indexSelected]["Username"].ToString() + "\" không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                string queryUpdate = "UPDATE Users SET Password=@pass WHERE UserName=@user";
                command = new SqlCommand(queryUpdate, connection);
                try
                {
                    command.Parameters.AddWithValue("@pass", this.txtPassword.Text);
                    command.Parameters.AddWithValue("@user", this.txtUsername.Text);
                    if (command.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Cập nhật tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        fillTheDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật tài khoản thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Có lỗi:\n" + err);
                }
                return;
            }
            else
            {
                dataReader.Close();
                MessageBox.Show("Không thể cập nhật vì tài khoản không tồn tại trong hệ thống!\nVui lòng kiểm tra lại...");
                return;
            }
        }

        /// <summary>
        /// Xử lý xóa tài khoản
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
            var result = MessageBox.Show("Bạn có chắc chắn xóa \"" + dataSet.Tables[0].Rows[indexSelected]["Username"].ToString() + "\" không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Thực hiện xóa
            string queryDelete = "DELETE FROM Users WHERE UserName = @userName";
            command = new SqlCommand(queryDelete, connection);
            command.Parameters.AddWithValue("@userName", dataSet.Tables[0].Rows[indexSelected]["Username"].ToString());
            if (command.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                fillTheDataGrid();
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Xử lý mở kết nối và điền thông tin vào gridmo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            fillTheDataGrid();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy loại tài khoản
        /// </summary>
        /// <returns></returns>
        private int getAcountType()
        {
            if ((this.chkAdmin.IsChecked).HasValue && (this.chkAdmin.IsChecked).Value)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Mở kết nối tới db
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
            string query = "SELECT * FROM Users;";
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataSet.Clear();
                dataAdapter.Fill(dataSet);
                this.dgAccounts.ItemsSource = dataSet.Tables[0].DefaultView;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        #endregion
    }
}
