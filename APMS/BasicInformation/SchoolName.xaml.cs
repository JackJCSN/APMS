﻿using System;
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
        }

        private void GetList()
        {
            School[]  s = School.GetSchools(Auth);

            CollegelistBox.ItemsSource = s;
        }

        private void Exiting(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            GetList();
        }

        private void Insert(object sender, RoutedEventArgs e)
        {
            if (CollegeName_textBox.Text == "")
            {
                return;
            }
            try
            {
                School.Insert(CollegeName_textBox.Text, Auth);
            }
            catch (NoPermissionException ex)
            {
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
            try
            {
                s =  (School)CollegelistBox.SelectedItem;
                if (s != null)
                {
                    s.Name = CollegeName_textBox.Text;
                    s.Update(Auth);
                }
            }
            catch (NoPermissionException ex)
            {
                MessageBox.Show(this, "权限不足！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "系统错误！\r\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            GetList();
            if (s != null)
            {
                CollegelistBox.SelectedValue = s.Name;
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            School s = null;
            try
            {
                s =  (School)CollegelistBox.SelectedItem;
                if (s != null)
                {
                    s.Delete(Auth);
                }
            }
            catch (NoPermissionException ex)
            {
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
