using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;
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
    /// FixedText.xaml 的交互逻辑
    /// </summary>
    public partial class FixedText : Page
    {
        public FixedText()
        {

            InitializeComponent();
            if (GlobalVariables.json.FixedText1 != null)
                Text1.Text = GlobalVariables.json.FixedText1;
            if (GlobalVariables.json.FixedText2 != null)
                Text2.Text = GlobalVariables.json.FixedText2;
            if (GlobalVariables.json.FixedText1 != null)
                Text3.Text = GlobalVariables.json.FixedText3;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalVariables.json.FixedText1 = Text1.Text;
            GlobalVariables.json.FixedText2 = Text2.Text;
            GlobalVariables.json.FixedText3 = Text3.Text;
            try
            {
                string jsonString = JsonSerializer.Serialize(GlobalVariables.json);
                File.WriteAllText(Path.Combine(GlobalVariables.DataPath, "data.json"), jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
