using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
//1512054 - TranMinhCuong - 22:13 2019-01-12
namespace QL_BanHang
{
    /// <summary>
    /// Màn hình quản lý
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        /// <summary>
        /// Màn hình con
        /// </summary>
        private LoginWindow loginWindow;
        #endregion

        #region Constructor
        /// <summary>
        /// Phương thức khởi tạo mặc định
        /// </summary>
        /// <param name="w"></param>
        public MainWindow(LoginWindow w)
        {
            InitializeComponent();
            this.loginWindow = w;
            this.Title = "Dashboard - Xin chào "+ loginWindow.GetUsername+"!";
            if (!loginWindow.IsAdmin)
            {
                this.btnQuanLyTaiKhoan.IsEnabled = false;
            }
            //Tạo đồng hồ để bàn
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        #endregion

        #region Events
        /// <summary>
        /// Xử lí hiện đồng hồ để bàn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string s = DateTime.Now.DayOfWeek.ToString().ToUpper();
            this.txtDate.Content = s + ", " + Convert.ToString(DateTime.Now.Date).Substring(0, 10);
            this.txtTime.Content = Convert.ToString(DateTime.Now.TimeOfDay).Substring(0, 8);
        }

        /// <summary>
        /// Xử lý hiện lại màn hình đăng nhập
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            loginWindow.Show();
            this.Hide();
        }

        /// <summary>
        /// Xử lý quản lý category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryManagerWindow categoryManagerWindow = new CategoryManagerWindow();
            categoryManagerWindow.ShowDialog();
        }

        /// <summary>
        /// Quản lý product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductManagerWindow productManagerWindow = new ProductManagerWindow();
            productManagerWindow.ShowDialog();
        }

        /// <summary>
        /// Quản lý các hóa đơn bán hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBanHang_Click(object sender, RoutedEventArgs e)
        {
            SalseWindow salseWindow = new SalseWindow(this.loginWindow.GetUsername);
            salseWindow.ShowDialog();
        }

        /// <summary>
        /// Xử lý thống kê
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnThongKe_Click(object sender, RoutedEventArgs e)
        {
            StatisticalWindow statisticalWindow = new StatisticalWindow();
            statisticalWindow.ShowDialog();
        }

        /// <summary>
        /// Xử lý quản lý khách hàng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuanLyKhachHang_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagerWindow customerManagerWindow = new CustomerManagerWindow();
            customerManagerWindow.ShowDialog();
        }

        /// <summary>
        /// Xử lý quản lý tài khoản
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuanLyTaiKhoan_Click(object sender, RoutedEventArgs e)
        {
            AccountManagerWindow accountManagerWindow = new AccountManagerWindow();
            accountManagerWindow.ShowDialog();
        }

        /// <summary>
        /// Thông tin về chương trình
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.ShowDialog();
        }
        #endregion

        #region Methods
        //Chưa dùng tới
        #endregion
    }
}
