using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// BlacklistTableMate.xaml 的交互逻辑
    /// </summary>
    public partial class BlacklistTableMate : Page
    {
        public ObservableCollection<string> BlacklistPairs { get; set; } = new ObservableCollection<string>();
        public BlacklistTableMate()
        {
            InitializeComponent();
            this.DataContext = this;
            RefreshListView();
        }
        public void RefreshListView()
        {
            if(GlobalVariables.json.BlackLists==null)
            {
                GlobalVariables.json.BlackLists = new List<BlackList>();
            }
            foreach (var pair in GlobalVariables.json.BlackLists)
            {
                BlacklistPairs.Add($"{pair.Name1} - {pair.Name2}");
            }
        }

        private void AddBlacklistPair_Click(object sender, RoutedEventArgs e)
        {
            if(Name1.Text=="" || Name2.Text=="")
            {
                MessageBox.Show("请输入姓名");
                return;
            }
            else
            {
                BlacklistPairs.Add($"{Name1.Text} - {Name2.Text}");
                GlobalVariables.json.BlackLists.Add(new BlackList { Name1 = Name1.Text, Name2 = Name2.Text });
                Name1.Text = "";
                Name2.Text = "";
                BlacklistListView.SelectedIndex=BlacklistListView.Items.Count-1;
                saveJson();
            }
        }

        private void RemoveBlacklistPair_Click(object sender, RoutedEventArgs e)
        {
            if(BlacklistListView.SelectedIndex>=0)
            {
                GlobalVariables.json.BlackLists.RemoveAt(BlacklistListView.SelectedIndex);
                BlacklistPairs.RemoveAt(BlacklistListView.SelectedIndex);
                saveJson();
            }
        }

        private void BlacklistListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(BlacklistListView.SelectedIndex>=0)
            {
                RemoveBlacklistPair.IsEnabled = true;
            }
            else
            {
                RemoveBlacklistPair.IsEnabled = false;
            }
        }
        
        private void saveJson()
        {
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

        private void Name1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                Name2.Focus();
            }
        }

        private void Name2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                AddBlacklistPair_Click(sender, e);
                Name1.Focus();
            }
        }
    }
}
