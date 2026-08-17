using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SeatMapper.FunctionRealization
{
    /// <summary>
    /// SelectionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public int SelectedIndex { get; private set; } = -1;
        public SelectionWindow(string title, List<string> selections)
        {
            InitializeComponent();
            TitleTextBlock.Text= title ?? "选择以继续";
            if(selections == null || selections.Count == 0)
            {
                throw new Exception("The selection list cannot be null or empty.");
            }
            foreach (string selection in selections)
            {
                SelectionListBox.Items.Add(selection);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = SelectionListBox.SelectedIndex;
            this.Close();
        }
    }
}
