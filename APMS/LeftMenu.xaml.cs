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
using APMS.BasicInformation;

namespace APMS
{
    /// <summary>
    /// LeftMenu.xaml 的交互逻辑
    /// </summary>
    public partial class LeftMenu : Page
    {
        public LeftMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //在右侧显示基本信息设置
            frame1.Navigate(new Uri("BasicInformation\\BaseInformationSet.xaml", UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //在右侧显示公寓房源管理
            frame1.Navigate(new Uri("ApartmentHousingManagement\\ApartmentHousing.xaml", UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //显示公寓住宿管理
            frame1.Navigate(new Uri("TheApartmentManagement\\ApartmentAccommodationProvided.xaml", UriKind.Relative));
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            
            //显示公寓住宿管理
            frame1.Navigate(new Uri("CommunityAssessment\\CommunityAssessment.xaml", UriKind.Relative));
        }
    }
}
