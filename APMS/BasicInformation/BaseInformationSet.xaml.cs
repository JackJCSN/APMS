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

namespace APMS.BasicInformation
{
    /// <summary>
    /// BaseInformationSet.xaml 的交互逻辑
    /// </summary>
    public partial class BaseInformationSet : Page
    {
        Window m = App.Current.MainWindow;

        public BaseInformationSet()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //学院设置
            var s = new SchoolName();
            s.Owner = m;
            s.Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //系、专业名称设置
            var d =  new Department_ProfessionalNameSet();
            d.Owner = m;
            d.Show();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //床位状态管理
            var b = new BedStatusQuery();
            b.Owner = m;
            b.Show();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            //学生信息统计
            var s = new StudentInformationStatistics();
            s.Owner = m;
            s.Show();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //规章制度通知管理
            var r =  new RulesAndRegulationsManagement();
            r.Owner = m;
            r.Show();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            //物业人员信息
            var p =  new PropertyInformation();
            p.Owner = m;
            p.Show();
        }
    }
}
