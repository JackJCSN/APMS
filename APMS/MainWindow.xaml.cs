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
        public MainWindow()
        {
            InitializeComponent();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((a, b) =>
            {
                try
                {
                    StatusTimer.Content = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Debuger.PrintException(ex);
                }
            });
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            App.Current.MainWindow = this;
            statusBar1.DataContext = ((App)App.Current).Auth;
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
    }
}
