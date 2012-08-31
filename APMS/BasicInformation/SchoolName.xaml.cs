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
using DataProvider;

namespace APMS.BasicInformation
{
    /// <summary>
    /// SchoolName.xaml 的交互逻辑
    /// </summary>
    public partial class SchoolName : Window
    {
        App app = (App)App.Current;
        private Authentication Auth;

        public SchoolName()
        {
            Auth = app.Auth;
            InitializeComponent();
            GetList();
            statuText.Content = "就绪";
        }

        private void GetList()
        {
            School[]  s = School.GetSchools(Auth);
            SchoolListBox.ItemsSource = s;
        }

        private void Exiting(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            GetList();
            statuText.Content = "就绪";
        }

        private void Insert(object sender, RoutedEventArgs e)
        {
            if (SchoolBox.Text == "")
            {
                statuText.Content = "请输入一个有效的名称";
                return;
            }
            try
            {
                statuText.Content = "插入错误,请联系管理员";
                School.Insert(SchoolBox.Text, Auth);
                statuText.Content = "就绪";
            }
            catch (NoPermissionException ex)
            {
                statuText.Content = "权限不足！请使用更高权限的账户或者向管理员请求临时授权";
                MessageBox.Show(this, "权限不足！\r\n" + ex.Message, "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "系统错误！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            GetList();
        }

        private void Modify(object sender, RoutedEventArgs e)
        {
            School s = null;
            statuText.Content = "更新错误，请联系管理员";
            try
            {
                s = (School)SchoolListBox.SelectedItem;
                if (s != null)
                {
                    s.Name = SchoolBox.Text;
                    s.Update(Auth);
                    statuText.Content = "已更新";
                }
                else
                {
                    statuText.Content = "请先选择一个条目或者使用新增功能";
                    return;
                }
            }
            catch (NoPermissionException ex)
            {
                statuText.Content = "权限不足！请使用更高权限的账户或者向管理员请求临时授权";
                MessageBox.Show(this, "权限不足！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "系统错误！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            GetList();
            if (s != null)
            {
                SchoolListBox.SelectedValue = s.Name;
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            School s = null;
            statuText.Content = "删除错误，请联系管理员";
            try
            {
                s =  (School)SchoolListBox.SelectedItem;
                if (s != null)
                {
                    s.Delete(Auth);
                    statuText.Content = "已删除";
                }
                else
                {
                    statuText.Content = "请选择一个用于删除的条目";
                    return;
                }
            }
            catch (NoPermissionException ex)
            {
                statuText.Content = "权限不足！请使用更高权限的账户或者向管理员请求临时授权";
                MessageBox.Show(this, "权限不足！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "系统错误！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            GetList();
        }
    }
}
