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
using System.Windows.Shapes;

namespace Demo.View
{
    /// <summary>
    /// Login.xaml 的互動邏輯
    /// </summary>
    public partial class Login : Window
    {
        Setting st;

        public Login()
        {
            InitializeComponent();
        }

        private void BtLogin_Click(object sender, RoutedEventArgs e)
        {
            //Setting st = new Setting();
            //st.Topmost = true;
            //st.Show();
            //st.Closed += St_Closed;
            //this.Close();
            DateTime dt = DateTime.Today;
            var partPassword = dt.ToString("MMdd");

            if (!tbPassword.Text.Equals($"12{partPassword}34"))
                MessageBox.Show("密碼錯誤");
            else
            {
                st = new Setting();
                st.Topmost = true;
                st.Show();
                st.Closed += St_Closed1;
                this.Hide();
              
            }
        }

        private void St_Closed1(object sender, EventArgs e)
        {
            st.Close();
        }

        private void BtClear_Click(object sender, RoutedEventArgs e)
        {
            tbPassword.Text = "";
        }

        private void Bt1_Click(object sender, RoutedEventArgs e)
        {
            tbPassword.Text += (sender as Button).Name.Replace("bt", "");
        }
    }
}
