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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataProvider;

namespace APMS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Authentication Auth = ((App)App.Current).Auth;
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((a, b) =>
            {
                try
                {
                    StatusTimer.Content = DateTime.Now;
                    //rectangle1.DataContext = ((App)App.Current).Auth;
                    LockView.Visibility = (Visibility)new BooleanToVisibilityConverter().Convert(((App)App.Current).Auth.IsExpired, null, null, null);
                    if (FailCount > 9)
                    {
                        if ((DateTime.Now - LastFail) > new TimeSpan(0, 1, 0))
                        {
                            FailCount = 0;
                            ErrorText.Text = "";
                            Unlock.IsEnabled = true;
                            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                        }
                        else
                        {
                            ErrorText.Text = String.Format("锁定:\r\n错误次数过多，请{0}秒后再试", (int)(61 - (DateTime.Now - LastFail).TotalSeconds));
                            Unlock.IsEnabled = false;
                            ErrorText.Foreground = Brushes.Yellow;
                            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debuger.PrintException(ex);
                }
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            App.Current.MainWindow = this;
            statusBar1.DataContext = Auth;
            LockView.Visibility = (Visibility)new BooleanToVisibilityConverter().Convert(Auth.IsExpired, null, null, null);
            StatusTimer.DataContext = DateTime.Now;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button15_Click(object sender, RoutedEventArgs e)
        {
            //功能界面
            frame3.Navigate(new Uri("LeftMenu.xaml", UriKind.Relative));
        }

        private void button14_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ConfigWindow cfg = new ConfigWindow();
            cfg.Owner = this;
            cfg.ShowDialog();
            MessageBox.Show("设置将在应用程序重新启动后生效", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private int FailCount = 0;
        private DateTime LastFail;
        private void Unlock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Auth.SignIn(Auth.Name, passwordBox1.Password))
                {
                    passwordBox1.Clear();
                    ErrorText.Text = "";
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                    return;
                }
                FailCount++;
                ErrorText.Text = "解锁失败:\r\n密码不正确。";
                ErrorText.Foreground = Brushes.Yellow;
                this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                LastFail = DateTime.Now;
            }
            catch (Exception ex)
            {
                ErrorText.Text = "错误:\r\n" + DataProvider.Debuger.PrintExcetionW(ex, 20);
                ErrorText.Foreground = Brushes.Red;
                this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
            }
        }
    }
}
