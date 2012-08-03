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

namespace APMS.ApartmentHousingManagement
{
    /// <summary>
    /// ApartmentHousing.xaml 的交互逻辑
    /// </summary>
    public partial class ApartmentHousing : Page
    {
        public ApartmentHousing()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //公寓收费标准
            new FlatRateSetting().Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //公寓房间查询
            new ApartmentSearch().Show();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //公寓床位查询
            new ApartmentSearch().Show();
        }
    }
}
