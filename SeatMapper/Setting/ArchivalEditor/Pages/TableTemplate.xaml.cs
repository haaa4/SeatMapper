using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace SeatMapper.Setting.ArchivalEditor.Pages
{
    /// <summary>
    /// TableTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class TableTemplate : Page
    {
        public TableTemplate()
        {
            InitializeComponent();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //先检查是否有模板文件，再打开编辑器
            if (File.Exists(Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx")) == false)
            {
                ResourceExporter.ExportResource("SeatMapper.User.template.xlsx", Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx"), true);
            }
            Process.Start(Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx"));
        }
    }
}
