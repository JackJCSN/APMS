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
using APMS.Properties;
using DataProvider;

namespace APMS
{
    /// <summary>
    /// DataTest.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {
        Settings setting = new Settings();

        public ConfigWindow()
        {
            this.DataContext = setting;
            InitializeComponent();
            passwordBox.Password = setting.UserPass;
        }

        private void FocusLost(object sender, RoutedEventArgs e)
        {
            setting.UserPass = passwordBox.Password;
        }

        private void FocusGet(object sender, EventArgs e)
        {
            Change();
        }

        private void Change()
        {
            try
            {
                String server = serverNameBox.Text;
                String user = userNameBox.Text;
                String pass = passwordBox.Password;
                Boolean IsIntegrated = setting.IsWindowsIntegrated;
                String[] dbnames = new String[0];
                if (IsIntegrated)
                {
                    dbnames = DBConnctor.GetDBNames(server);
                }
                else
                {
                    dbnames = DBConnctor.GetDBNames(server, user, pass);
                }
                dbNameBox.ItemsSource = dbnames;
            }
            catch (NullReferenceException)
            {
            }
        }



        private void testBtn_Click(object sender, RoutedEventArgs e)
        {
            bool isOk = false;
            String Error = "";
            try
            {

                if (setting.IsWindowsIntegrated)
                {
                    isOk = DBConnctor.TestDBConnction(setting.ServerName, setting.DataBaseName, out Error);
                }
                else
                {
                    isOk = DBConnctor.TestDBConnction(setting.ServerName, setting.DataBaseName, setting.UserName, setting.UserPass, out Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "连接测试失败!\r\n发生了一个错误：\r\n" + ex.Message, "连接测试", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (isOk)
            {
                MessageBox.Show(this, "连接测试成功", "连接测试", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "连接测试失败\r\n" + Error, "连接测试", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void reloadBtn_Click(object sender, RoutedEventArgs e)
        {
            setting.Reload();
        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult Result = MessageBox.Show(this,
                "存储于当前计算机的设置将会被设置重置为默认值。\r\n该操作不可恢复，设置当前设置将会丢失\r\n是否继续？",
                "重置设置", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (Result == MessageBoxResult.Yes)
            {
                setting.Reset();
            }
        }

        private void saveExitBtn_Click(object sender, RoutedEventArgs e)
        {
            setting.Save();
            Settings.Default.Reload();
            this.Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            setting.Save();
            Settings.Default.Reload();
        }
    }

    [ValueConversion(typeof(Boolean), typeof(Boolean))]
    public class BooleanNotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !((Boolean)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

}
