using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace APMS
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 获取或设置应用程序授权
        /// </summary>
        public DataProvider.Authentication Auth { get; internal set; }
    }
}
