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
    /// RulesAndRegulationsManagement.xaml 的交互逻辑
    /// </summary>
    public partial class RulesAndRegulationsManagement : Window
    {
        private Authentication Auth = ((App)App.Current).Auth;

        public RulesAndRegulationsManagement()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "文档|*.doc;*docx;*.pdf|电子表格|*.xls;*.xlsx|图片|*.jpg;*.gif;*.png;*.jpge|所有支持的文件类型|*.doc;*docx;*.pdf;*.xls;*.xlsx;*.jpg;*.gif;*.png;*.jpge";
            ofd.FilterIndex = 4;
            ofd.Title = "选择一个要上传的文件";
            ofd.ValidateNames = true;
            if ((bool)ofd.ShowDialog(this))
            {
                FilePathBox.Text = ofd.FileName;
                if (FileNameBox.Text == "")
                {
                    FileNameBox.Text = ofd.SafeFileName;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] data = e.Data.GetData("FileDrop") as String[];
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.FileName = data[0];
                FilePathBox.Text = ofd.FileName;
                if (FileNameBox.Text == "")
                {
                    FileNameBox.Text = ofd.SafeFileName;
                }
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Insert()
        {
            if (FileNameBox.Text != "")
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.FileName = FilePathBox.Text;
                try
                {
                    Documents D = new Documents(FileNameBox.Text);
                    D.SetData(ofd.OpenFile());
                    if (D.Insert(Auth))
                    {
                        statusText.Content = "上传成功!";
                        Reflush();
                    }
                    else
                    {
                        statusText.Content = "上传失败!已存在内容相同的文件。";
                        Reflush();
                    }
                    return;
                }
                catch (System.IO.IOException ex)
                {
                    Debuger.PrintException(ex);
                    statusText.Content = ex.Message;
                    return;
                }
            }
            statusText.Content = "请输入有效的文件名";
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Insert();
        }

        private void Reflush()
        {
            var D = Documents.GetDocuments(Auth);
            Files.ItemsSource = D;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Reflush();
            statusText.Content = "就绪";
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Reflush();
            statusText.Content = "就绪";
        }

        private void DataGridHyperlinkColumn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void Delete()
        {
            if (Files.SelectedItem != null)
            {
                int count = 0;
                int t = Files.SelectedItems.Count;
                try
                {
                    foreach (Documents d in Files.SelectedItems)
                    {
                        if (d.Delete(Auth))
                        {
                            count++;
                        }
                    }
                    if (count - t == 0)
                    {
                        statusText.Content = String.Format("已成功删除{0}个选中项", t);
                    }
                    else
                    {
                        statusText.Content = String.Format("删除时发生了一些错误，已成功删除{0}个选中项，剩余{1}", count, t - count);
                    }
                }
                catch (Exception ex)
                {
                    Debuger.PrintException(ex);
                    statusText.Content = ex.Message;
                }
                Reflush();
                return;
            }
            statusText.Content = "请先选择要删除的项。";
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void SaveFile()
        {
            if (Files.SelectedItem != null)
            {
                List<Documents> D = new List<Documents>();
                foreach (Documents d in Files.SelectedItems)
                {
                    D.Add(d);
                }
                D.All(a =>
                {
                    Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
                    sfd.OverwritePrompt = true;
                    sfd.FileName = a.Name;
                    sfd.ValidateNames = true;
                    sfd.AddExtension = true;
                    sfd.Filter = "文档|*.doc;*docx;*.pdf|电子表格|*.xls;*.xlsx|图片|*.jpg;*.gif;*.png;*.jpge|所有支持的文件类型|*.doc;*docx;*.pdf;*.xls;*.xlsx;*.jpg;*.gif;*.png;*.jpge";
                    sfd.FilterIndex = 4;
                    sfd.Title = String.Format("另存为——{0}", a.Name);
                    if ((bool)sfd.ShowDialog(this))
                    {
                        System.IO.Stream output = sfd.OpenFile();
                        output.BeginWrite(a.Data, 0, a.Data.Length, new AsyncCallback((b) => { output.Close(); }), null);
                        return true;
                    }
                    return false;
                });
            }
            statusText.Content = "请先选择一个要下载的文件。";
        }
    }
}
