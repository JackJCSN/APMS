using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Data.SqlClient;
using APMS.Properties;
using DataProvider;

namespace APMS
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        App app;
        public Login()
        {
            InitializeComponent();
            this.Show();
            app = (App)Application.Current;
            Loading();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //退出当前登录窗口
            this.Close();
        }

        private void Loading()
        {
            try
            {
                try
                {
                    app.Auth = new Authentication();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    ConfigWindow cfg = new ConfigWindow();
                    cfg.Owner = this;
                    cfg.ShowDialog();
                    Settings  s =  Settings.Default;
                    if (s.IsWindowsIntegrated)
                    {
                        app.Auth = new Authentication(s.ServerName, s.DataBaseName);
                    }
                    else
                    {
                        app.Auth = new Authentication(s.ServerName, s.DataBaseName, s.UserName, s.UserPass);
                    }
                }
                ErrorText.Content = null;
                login_btn.IsEnabled = true;
                nameBox.Focus();
#if DEBUGERLOGIN
                nameBox.Text = "DEBUGER";
                passwordBox.Password = "DEBUGER";
                login_btn_Click(this, new RoutedEventArgs());
#endif
            }
            catch (Exception ex)
            {
                Title = "错误";
                ErrorText.Content = "错误:\r\n程序无法连接到数据库。\r\n请检查应用程序设置后重新启动应用程序。\r\n" + DataProvider.Debuger.PrintExcetionW(ex, 20);
                ErrorText.Foreground = Brushes.Red;
            }
        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (app.Auth.SignIn(nameBox.Text, passwordBox.Password))
                {
                    new MainWindow().Show();
                    this.Close();
                }
                ErrorText.Content = "登录失败:\r\n" + "用户名或密码不正确。";
                ErrorText.Foreground = Brushes.Red;
            }
            catch (Exception ex)
            {
                ErrorText.Content = "错误:\r\n" + DataProvider.Debuger.PrintExcetionW(ex, 20);
                ErrorText.Foreground = Brushes.Red;
            }
        }
    }
}
