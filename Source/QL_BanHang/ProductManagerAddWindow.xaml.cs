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
    /// Màn hình thêm mới một product
    /// </summary>
    public partial class ProductManagerAddWindow : Window
    {
        #region Variables
        /// <summary>
        /// Chuỗi kết nối đến db
        /// </summary>
        public static string connectionString = LoginWindow.connectionString;
        SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand command;
        SqlDataReader dataReader;
        DataSet dataSet = new DataSet();
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        public ProductManagerAddWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Mở kết nối vào điền thông tin vào Combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            makeAconnection();
            fillTheComboboxCategory();
        }

        /// <summary>
        /// Xử lý thêm dữ liệu vào DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThemMoi_Click(object sender, RoutedEventArgs e)
        {
            int productIdAdd = -1;
            if (checkeAllTextbox())
            {
                MessageBox.Show("Vui lòng kiểm tra lại các trường trên!");
                return;
            }
            //Thêm vào Product trước
            string queryAddProduct = "INSERT INTO Product(CategoryID, ProductName, GiaGoc) VALUES(@categoryID, @productName, @giaGoc)";
            SqlCommand command2;
            try
            {
                command2 = new SqlCommand(queryAddProduct, connection);
                command2.Parameters.AddWithValue("@categoryID", this.cbCategory.SelectedValue);
                command2.Parameters.AddWithValue("@productName", this.txtProductName.Text);
                command2.Parameters.AddWithValue("@giaGoc", this.txtGiaGoc.Text);
                if (command2.ExecuteNonQuery() > 0)
                {
                    //lấy productID mới thêm vào để làm khóa ngoại cho productinfo
                    string querySelectID = "SELECT ProductID FROM Product WHERE CategoryID = @categoryID AND productName = @productName AND GiaGoc = @giaGoc";
                    command = new SqlCommand(querySelectID, connection);
                    command.Parameters.AddWithValue("@categoryID", this.cbCategory.SelectedValue);
                    command.Parameters.AddWithValue("@productName", this.txtProductName.Text);
                    command.Parameters.AddWithValue("@giaGoc", this.txtGiaGoc.Text);
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    if (dataReader.HasRows)
                    {
                        productIdAdd = int.Parse(dataReader["ProductID"].ToString());
                        dataReader.Close();
                    }
                    //Thêm vào productInfo
                    string queryProductInfo2 = "INSERT INTO ProductInfo(ProductID, ManHinh, HDH, CameraTruoc, CameraSau, CPU, RAM, ROM, SIM, PIN) " +
                        "VALUES(@productID, @manHinh, @hdh, @cameraTruoc, @cameraSau, @cpu, @ram, @rom, @sim, @pin)";
                    SqlCommand command3;
                    try
                    {
                        command3 = new SqlCommand(queryProductInfo2, connection);
                        command3.Parameters.AddWithValue("@productID", productIdAdd);
                        command3.Parameters.AddWithValue("@manHinh", this.txtManHinh.Text);
                        command3.Parameters.AddWithValue("@hdh", this.txtHeDieuHanh.Text);
                        command3.Parameters.AddWithValue("@cameraTruoc", this.txtCameraTruoc.Text);
                        command3.Parameters.AddWithValue("@cameraSau", this.txtCameraSau.Text);
                        command3.Parameters.AddWithValue("@cpu", this.txtCPU.Text);
                        command3.Parameters.AddWithValue("@ram", this.txtRAM.Text);
                        command3.Parameters.AddWithValue("@rom", this.txtROM.Text);
                        command3.Parameters.AddWithValue("@sim", this.txtSim.Text);
                        command3.Parameters.AddWithValue("@pin", this.txtPin.Text);
                        if (command3.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Thêm Product thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo");
                    return;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        /// <summary>
        /// Xử lý xóa trống các trường để người dùng nhập lại
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnXoaTrang_Click(object sender, RoutedEventArgs e)
        {
            this.cbCategory.SelectedIndex = -1;
            this.txtProductName.Text = string.Empty;
            this.txtGiaGoc.Text = string.Empty;
            this.txtManHinh.Text = string.Empty;
            this.txtHeDieuHanh.Text = string.Empty;
            this.txtCameraTruoc.Text = string.Empty;
            this.txtCameraSau.Text = string.Empty;
            this.txtCPU.Text = string.Empty;
            this.txtRAM.Text = string.Empty;
            this.txtROM.Text = string.Empty;
            this.txtSim.Text = string.Empty;
            this.txtPin.Text = string.Empty;
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
        private void fillTheComboboxCategory()
        {
            string query = "SELECT * FROM Category;";
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                dataAdapter.Fill(dataSet);
                this.cbCategory.ItemsSource = dataSet.Tables[0].DefaultView;
                this.cbCategory.DisplayMemberPath = "CategoryName";
                this.cbCategory.SelectedValuePath = "CategoryID";
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
            return string.IsNullOrEmpty(this.txtProductName.Text) ||
                    string.IsNullOrEmpty(this.txtGiaGoc.Text) ||
                    string.IsNullOrEmpty(this.txtManHinh.Text) ||
                    string.IsNullOrEmpty(this.txtHeDieuHanh.Text) ||
                    string.IsNullOrEmpty(this.txtCameraSau.Text) ||
                    string.IsNullOrEmpty(this.txtCameraTruoc.Text) ||
                    string.IsNullOrEmpty(this.txtCPU.Text) ||
                    string.IsNullOrEmpty(this.txtRAM.Text) ||
                    string.IsNullOrEmpty(this.txtROM.Text) ||
                    string.IsNullOrEmpty(this.txtSim.Text) ||
                    string.IsNullOrEmpty(this.txtPin.Text) ||
                    this.cbCategory.SelectedIndex == -1;
        }
        #endregion
    }
}
