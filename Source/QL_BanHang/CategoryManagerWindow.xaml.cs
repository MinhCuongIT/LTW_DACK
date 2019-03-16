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
    /// Màn hình hiển thị Category
    /// </summary>
    public partial class CategoryManagerWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        private int indexSelected = -1;
        private int idCategory = -1;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command;
        SqlDataReader dataReader;
        DataSet dataSet = new DataSet();
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        public CategoryManagerWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Thêm category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            string queryFindSomeUser = "SELECT * FROM Category WHERE CategoryName = @name";
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            command = new SqlCommand(queryFindSomeUser, connection);
            command.Parameters.AddWithValue("@name", this.txtCategoryName.Text);
            dataReader = command.ExecuteReader();
            dataReader.Read();
            if (dataReader.HasRows)
            {
                MessageBox.Show("Category đã tồn tại! Vui lòng chọn một Category khác", "Thông báo");
                dataReader.Close();
                return;
            }
            else
            {
                dataReader.Close();
                //Thêm vào db
                string queryAddUser = "INSERT INTO Category(CategoryName) VALUES(@name)";
                SqlCommand command2;
                try
                {
                    command2 = new SqlCommand(queryAddUser, connection);
                    command2.Parameters.AddWithValue("@name", this.txtCategoryName.Text);
                    if (command2.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Thêm Category thành công!", "Thông báo");
                        fillTheDataGrid();
                    }
                    else
                    {
                        MessageBox.Show("Thêm Category thất bại!", "Thông báo");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        /// <summary>
        /// Cập nhật category
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
            if (string.IsNullOrEmpty(this.txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            string queryUpdate = "UPDATE Category SET CategoryName=@name WHERE CategoryID = @id";
            command = new SqlCommand(queryUpdate, connection);
            try
            {
                command.Parameters.AddWithValue("@name", this.txtCategoryName.Text);
                command.Parameters.AddWithValue("@id", this.idCategory);
                if (command.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Cập nhật Category thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    fillTheDataGrid();
                }
                else
                {
                    MessageBox.Show("Cập nhật Category thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Có lỗi:\n" + err);
            }
            return;
        }

        /// <summary>
        /// Xóa category
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
            var result = MessageBox.Show("Bạn có chắc chắn xóa \"" + dataSet.Tables[0].Rows[indexSelected]["CategoryName"].ToString() + "\" không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Thực hiện xóa
            string queryDelete = "DELETE FROM Category WHERE CategoryID = @id";
            command = new SqlCommand(queryDelete, connection);
            command.Parameters.AddWithValue("@id", this.idCategory);
            if (command.ExecuteNonQuery() > 0)
            {
                MessageBox.Show("Xóa Category thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                fillTheDataGrid();
            }
            else
            {
                MessageBox.Show("Xóa Category thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lấy vị trí trong grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgCategory_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgCategory.Items.IndexOf(this.dgCategory.CurrentItem) >= 0)
            {
                this.indexSelected = this.dgCategory.Items.IndexOf(this.dgCategory.CurrentItem);
                this.txtCategoryName.Text = dataSet.Tables[0].Rows[indexSelected]["CategoryName"].ToString();
                this.idCategory = int.Parse(dataSet.Tables[0].Rows[indexSelected]["CategoryID"].ToString());
            }
        }

        /// <summary>
        /// Tạo kết nối và điền thông tin vào bảng
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
            string query = "SELECT * FROM Category;";
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataSet.Clear();
                dataAdapter.Fill(dataSet);
                this.dgCategory.ItemsSource = dataSet.Tables[0].DefaultView;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        #endregion
    }
}
