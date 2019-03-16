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
    /// Màn hình quản lý các product
    /// </summary>
    public partial class ProductManagerWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        /// <summary>
        /// Lưu trữ vị trí người dùng click trong bảng
        /// </summary>
        private int indexSelected = -1;
        /// <summary>
        /// Lưu giữ giá trị productID
        /// </summary>
        private int idProduct = -1;

        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command;
        SqlDataReader dataReader;
        DataSet dataSet = new DataSet();
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        public ProductManagerWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Mở kết nối và điền thông tin vào bảng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            fillTheDataGrid();
        }

        /// <summary>
        /// Xử lý xem thông tin về sản phẩm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnXem_Click(object sender, RoutedEventArgs e)
        {
            if (indexSelected == -1)
            {
                MessageBox.Show("Bạn chưa chọn đối tượng để xem!");
                return;
            }
            ProductInfoWindow productInfoWindow = new ProductInfoWindow(this.idProduct,
                dataSet.Tables[0].Rows[indexSelected]["ProductName"].ToString(),
                dataSet.Tables[0].Rows[indexSelected]["GiaGoc"].ToString());
            productInfoWindow.ShowDialog();
        }

        /// <summary>
        /// Xử lý thêm sản phẩm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThem_Click(object sender, RoutedEventArgs e)
        {
            ProductManagerAddWindow productManagerAddWindow = new ProductManagerAddWindow();
            productManagerAddWindow.ShowDialog();
            fillTheDataGrid();
        }

        /// <summary>
        /// Xử lý cập nhật sản phẩm
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
            ProductManagerUpdateWindow productManagerUpdateWindow = new ProductManagerUpdateWindow(this.idProduct);
            productManagerUpdateWindow.ShowDialog();
            fillTheDataGrid();
        }

        /// <summary>
        /// Xử lý xóa sản phẩm
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
            var result = MessageBox.Show("Bạn có chắc chắn xóa \"" + dataSet.Tables[0].Rows[indexSelected]["ProductName"].ToString() + "\" không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            //Thực hiện xóa
            //Xóa trong ProductInfo trước
            string queryDelete = "DELETE FROM ProductInfo WHERE ProductID = @idPro";
            command = new SqlCommand(queryDelete, connection);
            command.Parameters.AddWithValue("@idPro", this.idProduct);
            if (command.ExecuteNonQuery() > 0)
            {
                //Xóa product
                string queryDelete2 = "DELETE FROM Product WHERE ProductID = @idPro";
                SqlCommand command2 = new SqlCommand(queryDelete2, connection);
                command2.Parameters.AddWithValue("@idPro", this.idProduct);
                if (command2.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Xóa Product thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    fillTheDataGrid();
                    txtProductName.Clear();
                    this.txtGiaGoc.Clear();
                    this.indexSelected = -1;
                    this.idProduct = -1;
                }
            }
            else
            {
                MessageBox.Show("Xóa Product thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Bind data to textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgProduct_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dgProduct.Items.IndexOf(this.dgProduct.CurrentItem) >= 0)
            {
                this.indexSelected = this.dgProduct.Items.IndexOf(this.dgProduct.CurrentItem);
                this.idProduct = int.Parse(dataSet.Tables[0].Rows[indexSelected]["ProductID"].ToString());
                this.txtProductName.Text = dataSet.Tables[0].Rows[indexSelected]["ProductName"].ToString();
                //Lay mien
                var info = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
                var money = float.Parse(dataSet.Tables[0].Rows[indexSelected]["GiaGoc"].ToString());
                //set gia tien
                this.txtGiaGoc.Text = String.Format(info, "{0:c}", money);
            }
        }

        /// <summary>
        /// Lọc kết quả theo yêu cầu của người dùng khi text ở đây được nhập và thay đổi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtLocSP_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = this.txtLocSP.Text;
            if (string.IsNullOrEmpty(text))
            {
                fillTheDataGrid();
                return;
            }
            else
            {
                string query = "SELECT ProductID, CategoryName, ProductName, GiaGoc " +
                    "FROM Product p JOIN Category c ON c.CategoryID = p.CategoryID " +
                    "WHERE p.ProductName LIKE N\'%" + text + "%\'";
                try
                {
                    SqlDataAdapter dataAdapter2 = new SqlDataAdapter(query, connection);
                    dataSet.Clear();
                    dataAdapter2.Fill(dataSet);
                    this.dgProduct.ItemsSource = dataSet.Tables[0].DefaultView;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
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
            string query = "SELECT ProductID, CategoryName, ProductName, GiaGoc FROM Product p JOIN Category c ON c.CategoryID = p.CategoryID";
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataSet.Clear();
                dataAdapter.Fill(dataSet);
                this.dgProduct.ItemsSource = dataSet.Tables[0].DefaultView;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        #endregion
    }
}
