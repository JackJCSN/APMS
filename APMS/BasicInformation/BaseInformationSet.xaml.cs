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
        public BaseInformationSet()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //学院设置
            new SchoolName().Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //系、专业名称设置
            new Department_ProfessionalNameSet().Show();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            //床位状态管理
            new BedStatusQuery().Show();
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            //学生信息统计
            new StudentInformationStatistics().Show();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            //规章制度通知管理
            new RulesAndRegulationsManagement().Show();
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            //物业人员信息
            new PropertyInformation().Show();
        }
    }
}
