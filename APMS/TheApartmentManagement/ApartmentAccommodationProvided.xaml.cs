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

namespace APMS.GongYuZhuSuGuanLi
{
    /// <summary>
    /// ApartmentAccommodationProvided.xaml 的交互逻辑
    /// </summary>
    public partial class ApartmentAccommodationProvided : Page
    {
        public ApartmentAccommodationProvided()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //学生住宿登记
            new StudentAccommodationRegistration().Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //学生宿舍调整
            new Student_accommodation_adjustment().Show();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //宿舍预分配方案
            new Housing_distribution_management().Show();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            //学生离校管理
            new StudentsLeaveManagement().Show();
        }
    }
}
