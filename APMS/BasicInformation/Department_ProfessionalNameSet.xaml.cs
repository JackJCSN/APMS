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
    /// Department_ProfessionalNameSet.xaml 的交互逻辑
    /// </summary>
    public partial class Department_ProfessionalNameSet : Window
    {
        private Authentication Auth = ((App)App.Current).Auth;

        public Department_ProfessionalNameSet()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItemReflush(sender,e);
        }

        private void Load_Departments()
        {
            if (SchoolBox.SelectedItem != null)
            {
                School s = (School)SchoolBox.SelectedItem;
                DepartmentBox.ItemsSource = Department.GetDepartments(s, Auth);
                dataGrid1.ItemsSource = Major.GetMajor((School)SchoolBox.SelectedItem, Auth);
                SchoolBox.SelectedItem = s;
            }
        }

        private void SchoolBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender.Equals(SchoolBox))
            {
                Load_Departments();
            }
            else if (sender.Equals(DepartmentBox))
            {
                if (DepartmentBox.SelectedItem != null)
                {
                    School s = (School)SchoolBox.SelectedItem;
                    Department d =(Department)DepartmentBox.SelectedItem;
                    dataGrid1.ItemsSource = Major.GetMajor(d, Auth);
                    SchoolBox.SelectedItem = s;
                    DepartmentBox.SelectedItem = d;
                }
            }
        }

        private void MenuItemReflush(object sender, RoutedEventArgs e)
        {
            Reflush();
            statuText.Content = "就绪";
        }

        private void Reflush()
        {
            SchoolBox.ItemsSource = School.GetSchools(Auth);
            dataGrid1.ItemsSource = Major.GetMajor(Auth);
        }

        private void MenuItemExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItemInsert(object sender, RoutedEventArgs e)
        {
            Insert();
            MenuItemReflush(sender,e);
        }

        private void Insert()
        {
            if (SchoolBox.SelectedItem != null)
            {
                if (DepartmentBox.SelectedItem != null)
                {
                    try
                    {
                        if (MajorNameBox.Text != null && MajorNameBox.Text != "")
                        {
                            Major newMajor = new Major(MajorNameBox.Text,
                                (Department)DepartmentBox.SelectedItem, (School)SchoolBox.SelectedItem);
                            if (newMajor.Insert(Auth))
                            {
                                statuText.Content = "新增专业操作成功";
                                return;
                            }
                            else
                            {
                                statuText.Content = "操作失败，请检查是否已经存在同名专业？";
                                return;
                            }
                        }
                        statuText.Content = "请输入有效的专业名称";
                    }
                    catch (Exception ex)
                    {
                        statuText.Content = "错误:" + ex.Message;
                        return;
                    }
                }
                if (DepartmentBox.Text != null && DepartmentBox.Text != "")
                {
                    var r = MessageBox.Show("当前输入的院系在系统中无法找到，是否在当前院校下，新建此院系？", "要创建新的院系吗？", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (MessageBoxResult.Yes == r)
                    {
                        statuText.Content = "";
                        Department depart = null;
                        School s = null;
                        try
                        {
                            s = (School)SchoolBox.SelectedItem;
                            depart = new Department(DepartmentBox.Text, s.ID);
                            statuText.Content += "新增院系操作成功。";
                        }
                        catch (Exception ex)
                        {
                            statuText.Content += "错误:" + ex.Message;
                            return;
                        }

                        try
                        {
                            if (MajorNameBox.Text != null && MajorNameBox.Text != "")
                            {
                                Major newMajor = new Major(MajorNameBox.Text,
                                    depart, (School)SchoolBox.SelectedItem);
                                newMajor.Insert(Auth);
                                statuText.Content += "新增专业操作成功";
                                return;
                            }
                            statuText.Content += "请输入有效的专业名称";
                        }
                        catch (Exception ex)
                        {
                            statuText.Content += "错误:" + ex.Message;
                            return;
                        }
                    }
                    statuText.Content = "未作任何更改";
                    return;
                }
                statuText.Content = "请选择或输入一个有效的院系";
                return;
            }
            statuText.Content = "请选择一个有效的院校";
        }

        private void MenuItemModify(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemDelete(object sender, RoutedEventArgs e)
        {

        }
    }
}
