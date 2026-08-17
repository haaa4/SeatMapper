// /*
//  * SeatMapper
//  * Copyright (C) 2026 haaa4
//  *
//  * This program is free software: you can redistribute it and/or modify
//  * it under the terms of the GNU General Public License as published by
//  * the Free Software Foundation, either version 3 of the License, or
//  * (at your option) any later version.
//  *
//  * This program is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY

using Masuit.Tools;
using SeatMapper.Setting.AppSetting;
using SeatMapper.Setting.ArchivalEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using unvell.ReoGrid;
using unvell.ReoGrid.IO;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using Path = System.IO.Path;

namespace SeatMapper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Refresh();

        }
        // 占位符类型
        public enum PlaceholderType
        {
            Fixed,      // \m 或 \f
            Flexible,   // \r
            Reference   // \=XX
        }

        public enum Gender
        {
            Unknown,
            Male,
            Female
        }

        public class Placeholder
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public string OriginalContent { get; set; }   // 原始标记
            public PlaceholderType Type { get; set; }
            public Gender Gender { get; set; } = Gender.Unknown;
            public string RefAddress { get; set; }       // 仅当 Type == Reference 时有效
            public string AssignedName { get; set; }     // 最终分配的名字
        }

        public static class RandomHelper
        {
            private static readonly Random _random = new Random();
            public static int StrictNext(int maxValue)  // 返回 [0, maxValue-1]
            {
                lock (_random)
                {
                    return _random.StrictNext(maxValue);
                }
            }
        }
        private int RefreshMode = 0;
        /// <summary>
        /// 刷新所有资源
        /// </summary>
        public void Refresh()
        {
            CaseText.Text = "正在初始化...";
            grid.Visibility = Visibility.Hidden;
            //0 什么都不做 1 回退到表格模板 2 回退到生成前的表格
            int loadmode = 1;
            if(RefreshMode ==1)
            {
                var selectionWindow = new FunctionRealization.SelectionWindow("如何初始化表格?", new List<string> { "回退到表格模板","保持现状" });
                selectionWindow.ShowDialog();
                if(selectionWindow.SelectedIndex!=-1)
                {
                    if(selectionWindow.SelectedIndex == 1)
                    {
                        loadmode = 0;
                    }
                }
                else
                {
                    grid.Visibility = Visibility.Visible;
                    CaseText.Text = "就绪";
                    return;
                }
            }
            else if(RefreshMode ==2)
            {
                var selectionWindow = new FunctionRealization.SelectionWindow("如何初始化表格?", new List<string> { "回退到表格模板", "回退到生成前的表格", "保持现状" });
                selectionWindow.ShowDialog();
                if (selectionWindow.SelectedIndex != -1)
                {
                    if (selectionWindow.SelectedIndex == 1)
                    {
                        loadmode = 2;
                    }
                    else if(selectionWindow.SelectedIndex == 2)
                    {
                        loadmode = 0;
                    }
                }
                else
                {
                    grid.Visibility = Visibility.Visible;
                    CaseText.Text = "就绪";
                    return;
                }
            }

            //检查是否有权限访问数据目录
            if (File.Exists(Path.Combine(GlobalVariables.DataPath, "PermissionTest")))
            {
                try
                {
                    File.WriteAllText(Path.Combine(GlobalVariables.DataPath, "PermissionTest"), "test");
                }
                catch (Exception ex)
                {
                    //尝试提高权限
                    IncreasePrivileges(ex);
                }
            }
            else
            {
                try
                {
                    File.Create(Path.Combine(GlobalVariables.DataPath, "PermissionTest")).Close();
                }
                catch (Exception ex)
                {
                    //尝试提高权限
                    IncreasePrivileges(ex);
                }
            }
            try
            {
                //添加男女名单
                if(File.Exists(Path.Combine(GlobalVariables.DataPath, "男.txt")) == false)
                {
                    File.Create(Path.Combine(GlobalVariables.DataPath, "男.txt")).Close();
                }
                if (File.Exists(Path.Combine(GlobalVariables.DataPath, "女.txt")) == false)
                {
                    File.Create(Path.Combine(GlobalVariables.DataPath, "女.txt")).Close();
                }
                GlobalVariables.MaleList = new List<string>();
                foreach (string item in File.ReadLines(Path.Combine(GlobalVariables.DataPath, "男.txt")))
                {
                    GlobalVariables.MaleList.Add(item);
                }
                GlobalVariables.FemaleList = new List<string>();
                foreach (string item in File.ReadLines(Path.Combine(GlobalVariables.DataPath, "女.txt")))
                {
                    GlobalVariables.FemaleList.Add(item);
                }
                //检查是否有模板文件
                if (File.Exists(Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx")) == false)
                {
                    ResourceExporter.ExportResource("SeatMapper.User.template.xlsx", Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx"), true);
                }
                //检查是否有配置文件并读取
                if (File.Exists(Path.Combine(GlobalVariables.DataPath, "data.json")) == false)
                {
                    File.Create(Path.Combine(GlobalVariables.DataPath, "data.json")).Close();
                }
                if(File.ReadAllText(Path.Combine(GlobalVariables.DataPath, "data.json")).Length == 0)
                {
                    File.WriteAllText(Path.Combine(GlobalVariables.DataPath, "data.json"), "{}");
                }
                string get=File.ReadAllText(Path.Combine(GlobalVariables.DataPath, "data.json"));
                GlobalVariables.json = JsonSerializer.Deserialize<Json>(get);
                //加载模板文件
                if (loadmode == 1)
                {
                    grid.Load(Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx"));
                }
                else if (loadmode == 2)
                {
                    if (File.Exists(Path.Combine(GlobalVariables.DataPath, "Backup.xlsx")))
                    {
                        grid.Load(Path.Combine(GlobalVariables.DataPath, "Backup.xlsx"));
                    }
                    else
                    {
                        MessageBox.Show("未找到生成前的表格备份，将回退到模板表格。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        grid.Load(Path.Combine(GlobalVariables.DataPath, "TableTemplate.xlsx"));
                    }
                }
                RefreshMode = 0;
                var snackbar = new Snackbar(snackbarPresenter)
                {
                    Content = "初始化完成", // 设置提示内容
                    Title = "信息",
                    Appearance = ControlAppearance.Info,
                    Timeout = TimeSpan.FromSeconds(3) // 显示时长
                };
                snackbar.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化失败！\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                RefreshMode = 0;
                CaseText.Text= "初始化失败!";
                return;
            }
            CaseText.Text = "就绪";
            grid.Visibility = Visibility.Visible;
            GenerateButton.IsEnabled = true;
            SaveButton.IsEnabled = false;
        }
        /// <summary>
        /// 没有权限访问数据目录时尝试提高权限
        /// </summary>
        /// <param name="ex">若已经提高权限，则展现给用户的错误信息</param>
        public void IncreasePrivileges(Exception ex)
        {
            if (!AdminHelper.IsAdministrator())
            {
                AdminHelper.RestartAsAdmin();
                return;
            }
            else
            {
                MessageBox.Show("程序已以管理员权限运行，但仍然无法访问数据目录，请检查权限！\n" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ArchivalEditor archivalEditor = new ArchivalEditor();
            archivalEditor.ShowDialog();
        }

        private void InitializeButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GenerateButton.IsEnabled = false;
                grid.Visibility=Visibility.Hidden;
                //先备份当前表格
                grid.Save(Path.Combine(GlobalVariables.DataPath, "Backup.xlsx"), FileFormat.Excel2007);
                int result = GenerateSeating();
                CaseText.Text = "生成成功！你可自行调整，然后请务必保存座位表。重新生成请按初始化按钮";
                var snackbar = new Snackbar(snackbarPresenter)
                {
                    Content = "生成成功", // 设置提示内容
                    Title = "成功",
                    Appearance = ControlAppearance.Success,
                    Timeout = TimeSpan.FromSeconds(3) // 显示时长
                };
                snackbar.Show();
                SaveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                CaseText.Text = "生成失败！重新生成请按初始化按钮";
            }
            finally
            {
                grid.Visibility = Visibility.Visible;
                RefreshMode = 2;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 检查是否有表格内容
                if (grid.CurrentWorksheet == null)
                {
                    MessageBox.Show("没有可保存的表格。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 弹出保存文件对话框
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Excel 工作簿|*.xlsx";
                saveFileDialog.DefaultExt = ".xlsx";
                saveFileDialog.FileName = "座位表_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                saveFileDialog.InitialDirectory = GlobalVariables.DataPath;

                // 用户选择保存路径
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // 调用 ReoGrid 的 Save 方法
                    grid.Save(filePath, FileFormat.Excel2007);

                    // 成功提示
                    MessageBox.Show($"座位表已成功保存至：\n{filePath}", "保存成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(filePath);
                }
                // 如果用户取消，不做任何操作
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 生成座位表：根据标记替换姓名，满足性别匹配、名字不重复、黑名单同桌距离约束。
        /// 同时支持固定文本替换：\Text1、\Text2、\Text3。
        /// </summary>
        /// <returns>成功返回 0，失败抛出异常</returns>
        public int GenerateSeating()
        {
           
            // 1. 获取当前工作表
            var sheet = grid.CurrentWorksheet;
            if (sheet == null)
                throw new Exception("未找到活动工作表。");

            // 2. 获取姓名列表，并检查非空
            var maleList = GlobalVariables.MaleList ?? new List<string>();
            var femaleList = GlobalVariables.FemaleList ?? new List<string>();
            if (maleList.Count == 0 && femaleList.Count == 0)
                throw new Exception("男生和女生名单均为空，无法分配。");

            // 3. 扫描所有单元格，收集占位符和固定文本替换
            var placeholders = new List<Placeholder>();
            var textReplacements = new List<(int Row, int Col, string Text)>();
            int rowCount = sheet.Rows;
            int colCount = sheet.Columns;

            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    var cell = sheet.GetCell(r, c);
                    if (cell == null) continue;

                    var data = cell.Data as string;
                    if (string.IsNullOrEmpty(data)) continue;

                    string content = data.Trim();
                    if (!content.StartsWith("\\")) continue;

                    // --- 固定文本替换 ---
                    if (content == "\\Text1" || content == "\\Text2" || content == "\\Text3")
                    {
                        string fixedText;
                        switch (content)
                        {
                            case "\\Text1":
                                fixedText = ConvertFixedText(GlobalVariables.json?.FixedText1);
                                break;
                            case "\\Text2":
                                fixedText = ConvertFixedText(GlobalVariables.json?.FixedText2);
                                break;
                            case "\\Text3":
                                fixedText = ConvertFixedText(GlobalVariables.json?.FixedText3);
                                break;
                            default:
                                fixedText = null;
                                break;
                        }
                        if (string.IsNullOrEmpty(fixedText))
                            throw new Exception($"标记 {content} 对应的文本未配置，请在 data.json 中设置 FixedText1/2/3。");
                        textReplacements.Add((r, c, fixedText));
                        continue; // 不加入占位符列表
                    }

                    // --- 座位占位符 ---
                    var ph = new Placeholder { Row = r, Col = c, OriginalContent = content };

                    if (content == "\\m")
                    {
                        ph.Type = PlaceholderType.Fixed;
                        ph.Gender = Gender.Male;
                        placeholders.Add(ph);
                    }
                    else if (content == "\\f")
                    {
                        ph.Type = PlaceholderType.Fixed;
                        ph.Gender = Gender.Female;
                        placeholders.Add(ph);
                    }
                    else if (content == "\\r")
                    {
                        ph.Type = PlaceholderType.Flexible;
                        ph.Gender = Gender.Unknown;
                        placeholders.Add(ph);
                    }
                    else if (content.StartsWith("\\="))
                    {
                        string refAddr = content.Substring(2).Trim();
                        if (string.IsNullOrEmpty(refAddr))
                            throw new Exception($"单元格 {GetAddress(r, c)} 的引用地址为空。");
                        ph.Type = PlaceholderType.Reference;
                        ph.RefAddress = refAddr;
                        placeholders.Add(ph);
                    }
                    // 其他以 \ 开头但不匹配的标记忽略（保持原样）
                }
            }

            if (placeholders.Count == 0 && textReplacements.Count == 0)
                throw new Exception("表格中未找到任何占位符（\\m, \\f, \\r, \\=XX）或固定文本标记（\\Text1/2/3）。");
            // 如果没有占位符但有文本替换，直接写入文本并返回
            if (placeholders.Count == 0)
            {
                foreach (var (r, c, text) in textReplacements)
                {
                    var cell = sheet.GetCell(r, c);
                    if (cell == null) throw new Exception($"无法获取单元格 {GetAddress(r, c)}。");
                    cell.Data = text;
                }
                return 0;
            }

            // 4. 构建地址 -> 占位符映射
            var addrMap = new Dictionary<string, Placeholder>();
            foreach (var ph in placeholders)
            {
                string addr = GetAddress(ph.Row, ph.Col);
                if (addrMap.ContainsKey(addr))
                    throw new Exception($"单元格 {addr} 重复出现，请检查表格。");
                addrMap[addr] = ph;
            }

            // 5. 解析所有引用依赖（允许指向 \r），仅检测循环和继承固定性别
            var visitState = new Dictionary<Placeholder, int>(); // 0=未访问, 1=访问中, 2=已完成

            foreach (var ph in placeholders)
                visitState[ph] = 0;

            void ResolveDependency(Placeholder ph, HashSet<Placeholder> path)
            {
                if (visitState[ph] == 1)
                    throw new Exception($"检测到循环依赖，涉及单元格 {GetAddress(ph.Row, ph.Col)}。");
                if (visitState[ph] == 2)
                    return;

                visitState[ph] = 1;
                path.Add(ph);

                if (ph.Type == PlaceholderType.Reference)
                {
                    if (string.IsNullOrEmpty(ph.RefAddress))
                        throw new Exception($"单元格 {GetAddress(ph.Row, ph.Col)} 引用地址为空。");

                    if (!addrMap.TryGetValue(ph.RefAddress, out var refPh))
                        throw new Exception($"单元格 {GetAddress(ph.Row, ph.Col)} 引用了不存在的单元格 {ph.RefAddress}。");

                    if (path.Contains(refPh))
                        throw new Exception($"循环依赖：{GetAddress(ph.Row, ph.Col)} -> {ph.RefAddress}。");

                    // 递归解析目标
                    ResolveDependency(refPh, path);

                    // 如果目标已确定性别，则继承
                    if (refPh.Gender != Gender.Unknown)
                        ph.Gender = refPh.Gender;
                }

                visitState[ph] = 2;
                path.Remove(ph);
            }

            // 对所有引用执行解析
            foreach (var ph in placeholders.Where(p => p.Type == PlaceholderType.Reference))
            {
                ResolveDependency(ph, new HashSet<Placeholder>());
            }

            // ★ 校验：引用目标为固定性别但自身仍为 Unknown 的，视为解析失败
            foreach (var ph in placeholders.Where(p => p.Type == PlaceholderType.Reference))
            {
                var target = addrMap[ph.RefAddress];
                if (target.Type == PlaceholderType.Fixed && ph.Gender == Gender.Unknown)
                {
                    throw new Exception($"引用 {GetAddress(ph.Row, ph.Col)} -> {ph.RefAddress} 未能正确解析性别。");
                }
            }

            // 6. 统计各类性别数量
            int mustMale = placeholders.Count(p => p.Gender == Gender.Male);
            int mustFemale = placeholders.Count(p => p.Gender == Gender.Female);
            int flexible = placeholders.Count(p => p.Gender == Gender.Unknown);

            int totalPlaceholders = placeholders.Count;
            int totalNames = maleList.Count + femaleList.Count;
            if (totalPlaceholders != totalNames)
                throw new Exception($"占位符总数 ({totalPlaceholders}) 与姓名总数 ({totalNames}) 不匹配。");

            if (mustMale > maleList.Count)
                throw new Exception($"必须为男性的占位符数量 ({mustMale}) 超过男生名单人数 ({maleList.Count})。");
            if (mustFemale > femaleList.Count)
                throw new Exception($"必须为女性的占位符数量 ({mustFemale}) 超过女生名单人数 ({femaleList.Count})。");

            int needMale = maleList.Count - mustMale;
            int needFemale = femaleList.Count - mustFemale;
            if (needMale + needFemale != flexible)
                throw new Exception($"灵活占位符数量 ({flexible}) 与需要补充的性别数 ({needMale + needFemale}) 不一致。");

            // 7. 构建灵活组：每个 \r 及其所有最终引用它的 \=XX 为一组
            var flexGroupMap = new Dictionary<Placeholder, List<Placeholder>>();

            // 初始化所有 \r 组
            foreach (var rPh in placeholders.Where(p => p.Type == PlaceholderType.Flexible))
            {
                flexGroupMap[rPh] = new List<Placeholder> { rPh };
            }

            // 遍历所有 Unknown 的 Reference 占位符（它们最终依赖某个 \r）
            foreach (var ph in placeholders.Where(p => p.Type == PlaceholderType.Reference && p.Gender == Gender.Unknown))
            {
                // 沿着引用链找到末端的 \r
                var target = ph;
                while (target.Type == PlaceholderType.Reference)
                {
                    if (!addrMap.TryGetValue(target.RefAddress, out target))
                        throw new Exception($"无法解析引用链：{GetAddress(ph.Row, ph.Col)}");
                }
                if (target.Type == PlaceholderType.Flexible)
                {
                    if (!flexGroupMap.ContainsKey(target))
                        throw new Exception($"未找到目标 \r: {GetAddress(target.Row, target.Col)}");
                    flexGroupMap[target].Add(ph);
                }
                else
                {
                    // 理论上不应该发生，因为 Gender==Unknown 说明末端必须是 \r
                    throw new Exception($"引用链末端不是灵活占位符：{GetAddress(ph.Row, ph.Col)} -> {GetAddress(target.Row, target.Col)}");
                }
            }

            var genderGroups = flexGroupMap.Values.ToList();

            // 校验灵活组总人数 == flexible
            int totalFlexibleInGroups = genderGroups.Sum(g => g.Count);
            if (totalFlexibleInGroups != flexible)
                throw new Exception($"灵活组总人数 ({totalFlexibleInGroups}) 与灵活占位符数量 ({flexible}) 不一致。");

            // 8. 为灵活组分配性别
            // 打乱组顺序
            var shuffledGroups = genderGroups.OrderBy(x => RandomHelper.StrictNext(10000)).ToList();

            int maleRemaining = needMale;
            int femaleRemaining = needFemale;

            foreach (var group in shuffledGroups)
            {
                int groupSize = group.Count;

                // ★ 检查：如果组大小同时大于两种剩余人数，则无法分配
                if (groupSize > maleRemaining && groupSize > femaleRemaining)
                {
                    throw new Exception($"灵活组大小 ({groupSize}) 大于剩余男性 ({maleRemaining}) 和剩余女性 ({femaleRemaining})，无法分配。请调整座位模板或名单。");
                }

                bool assignMale;
                if (groupSize > maleRemaining)
                {
                    // 只能分配为女性
                    assignMale = false;
                }
                else if (groupSize > femaleRemaining)
                {
                    // 只能分配为男性
                    assignMale = true;
                }
                else
                {
                    // 按剩余比例随机
                    double pMale = (double)maleRemaining / (maleRemaining + femaleRemaining);
                    assignMale = RandomHelper.StrictNext(100) < pMale * 100;
                }

                Gender targetGender = assignMale ? Gender.Male : Gender.Female;
                foreach (var ph in group)
                    ph.Gender = targetGender;

                if (assignMale)
                    maleRemaining -= groupSize;
                else
                    femaleRemaining -= groupSize;
            }

            // 最终校验剩余应为 0
            if (maleRemaining != 0 || femaleRemaining != 0)
                throw new Exception($"分配后剩余性别数异常：男性剩余 {maleRemaining}，女性剩余 {femaleRemaining}。");

            // 9. 最终性别总数校验
            int finalMale = placeholders.Count(p => p.Gender == Gender.Male);
            int finalFemale = placeholders.Count(p => p.Gender == Gender.Female);
            if (finalMale != maleList.Count || finalFemale != femaleList.Count)
                throw new Exception($"最终分配性别与名单不匹配。男性：{finalMale}/{maleList.Count}，女性：{finalFemale}/{femaleList.Count}");

            // 10. 多轮尝试分配名字并检查黑名单（最多 1000 次）
            var blackList = GlobalVariables.json?.BlackLists ?? new List<BlackList>();
            int maxAttempts = GlobalVariables.json?.MaximumNumberOfAttempts ?? 1000;
            bool success = false;

            List<string> ShuffleList(List<string> list)
            {
                var shuffled = new List<string>(list);
                for (int i = shuffled.Count - 1; i > 0; i--)
                {
                    int j = RandomHelper.StrictNext(i + 1);
                    var temp = shuffled[i];
                    shuffled[i] = shuffled[j];
                    shuffled[j] = temp;
                }
                return shuffled;
            }

            bool CheckBlacklist(Dictionary<string, (int Row, int Col)> seatMap)
            {
                foreach (var pair in blackList)
                {
                    string name1 = pair.Name1?.Trim() ?? "";
                    string name2 = pair.Name2?.Trim() ?? "";
                    if (string.IsNullOrEmpty(name1) || string.IsNullOrEmpty(name2))
                        throw new Exception("黑名单中存在空姓名。");

                    if (!seatMap.TryGetValue(name1, out var pos1))
                        throw new Exception($"黑名单中的姓名 '{name1}' 未出现在座位表中（请检查大小写或前后空格）。");
                    if (!seatMap.TryGetValue(name2, out var pos2))
                        throw new Exception($"黑名单中的姓名 '{name2}' 未出现在座位表中（请检查大小写或前后空格）。");

                    int rowDiff = Math.Abs(pos1.Row - pos2.Row);
                    int colDiff = Math.Abs(pos1.Col - pos2.Col);
                    if (rowDiff <= 1 && colDiff <= 1 && !(rowDiff == 0 && colDiff == 0))
                        return false;
                }
                return true;
            }

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var shuffledMale = ShuffleList(maleList);
                var shuffledFemale = ShuffleList(femaleList);

                var malePhs = placeholders.Where(p => p.Gender == Gender.Male).ToList();
                var femalePhs = placeholders.Where(p => p.Gender == Gender.Female).ToList();

                if (malePhs.Count != shuffledMale.Count || femalePhs.Count != shuffledFemale.Count)
                    throw new Exception("分配时性别数量与名单数量不一致。");

                for (int i = 0; i < malePhs.Count; i++)
                    malePhs[i].AssignedName = shuffledMale[i];
                for (int i = 0; i < femalePhs.Count; i++)
                    femalePhs[i].AssignedName = shuffledFemale[i];

                var seatMap = new Dictionary<string, (int Row, int Col)>(StringComparer.OrdinalIgnoreCase);
                foreach (var ph in placeholders)
                {
                    if (string.IsNullOrEmpty(ph.AssignedName))
                        throw new Exception($"单元格 {GetAddress(ph.Row, ph.Col)} 未分配名字。");
                    var key = ph.AssignedName.Trim();
                    if (seatMap.ContainsKey(key))
                        throw new Exception($"重复名字 '{key}' 出现在多个座位。");
                    seatMap[key] = (ph.Row, ph.Col);
                }

                if (CheckBlacklist(seatMap))
                {
                    success = true;
                    break;
                }
            }

            if (!success)
                throw new Exception($"经过 {maxAttempts} 次尝试，仍无法满足黑名单同桌距离约束。");

            // 11. 写入表格：先写入分配的名字
            foreach (var ph in placeholders)
            {
                var cell = sheet.GetCell(ph.Row, ph.Col);
                if (cell == null)
                    throw new Exception($"无法获取单元格 {GetAddress(ph.Row, ph.Col)}。");
                cell.Data = ph.AssignedName;
            }

            // 12. 写入固定文本替换（覆盖可能存在的占位符，但不会冲突，因为未加入 placeholders）
            foreach (var (r, c, text) in textReplacements)
            {
                var cell = sheet.GetCell(r, c);
                if (cell == null)
                    throw new Exception($"无法获取单元格 {GetAddress(r, c)}。");
                cell.Data = text;
            }

            return 0;
        }

        /// <summary>
        /// 将行列索引转换为 Excel 风格的地址（如 A1, B2）
        /// </summary>
        private string GetAddress(int row, int col)
        {
            // 列索引转字母（A=0, B=1, ...）
            string colLetters = "";
            int c = col + 1;
            while (c > 0)
            {
                int rem = (c - 1) % 26;
                colLetters = (char)('A' + rem) + colLetters;
                c = (c - rem) / 26;
            }
            return colLetters + (row + 1).ToString();
        }

        private void grid_CurrentWorksheetChanged(object sender, EventArgs e)
        {
            
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(RefreshMode==0)
            {
                RefreshMode = 1;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AppSetting appSetting = new AppSetting();
            appSetting.ShowDialog();
        }

        private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (GlobalVariables.json.AppTheme == 0)
            {
                SystemThemeWatcher.Watch(this, Wpf.Ui.Controls.WindowBackdropType.Tabbed);
            }
            else
            {
                SystemThemeWatcher.UnWatch(this);
                if (GlobalVariables.json.AppTheme == 1)
                {
                    ApplicationThemeManager.Apply(ApplicationTheme.Light, WindowBackdropType.Tabbed);
                }
                else if (GlobalVariables.json.AppTheme == 2)
                {
                    ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.Tabbed);
                }
            }
            // 获取主屏幕工作区大小（已排除任务栏）
            double maxWidth = SystemParameters.WorkArea.Width;
            double maxHeight = SystemParameters.WorkArea.Height;

            // 根据需求设置窗口大小，例如设为工作区的80%
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;
            this.Width = maxWidth * 0.8;
            this.Height = maxHeight * 0.8;
            // 1. 获取屏幕工作区大小（已排除任务栏，以 WPF 的 DIP 单位为准）
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;
            // 2. 获取窗口当前的实际大小（注意要用 ActualWidth/ActualHeight）
            //    如果窗口尚未完成布局，可以强制调用 UpdateLayout() 或使用 Dispatcher
            if (double.IsNaN(this.Width) || this.Width == 0)
            {
                // 如果 Width/Height 未显式设置，强制测量一次
                this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            double windowWidth = this.ActualWidth > 0 ? this.ActualWidth : this.Width;
            double windowHeight = this.ActualHeight > 0 ? this.ActualHeight : this.Height;

            // 3. 计算左上角坐标，确保窗口完全居中
            this.Left = (screenWidth - windowWidth) / 2;
            this.Top = (screenHeight - windowHeight) / 2;

            // 4. 【安全保险】如果坐标出现负值或溢出，重置为0（避免跑到屏幕外）
            if (this.Left < 0) this.Left = 0;
            if (this.Top < 0) this.Top = 0;
        }

        private string ConvertFixedText(string text)
        {
            string set = text;
            var dict = new Dictionary<string, string>
            {
                ["{year}"] = DateTime.Now.ToString("yyy"),
                ["{month}"] = DateTime.Now.ToString("MM"),
                ["{day}"] = DateTime.Now.ToString("dd"),
                ["{week}"] = GetRelativeWeekNumber(GlobalVariables.json.SemesterStartDate ?? DateTime.Now, DateTime.Now).ToString(),
                ["{hour}"] = DateTime.Now.ToString("HH"),
                ["{min}"] = DateTime.Now.ToString("mm"),
                ["{sec}"] = DateTime.Now.ToString("ss"),
                ["{time}"] = DateTime.Now.ToString("f"),
                ["{name}"] = Environment.UserName,
                ["{male}"] = GlobalVariables.MaleList.Count.ToString(),
                ["{female}"] = GlobalVariables.FemaleList.Count.ToString(),
                ["{person}"] = (GlobalVariables.MaleList.Count + GlobalVariables.FemaleList.Count).ToString()
            };
            foreach(var item in dict)
            {
                set=set.Replace(item.Key, item.Value);
            }
            return set;
        }
        /// <summary>
        /// 计算 targetDate 相对于 referenceDate 所在周为第1周的周数。
        /// </summary>
        /// <param name="referenceDate">参考日期，其所在周为第1周</param>
        /// <param name="targetDate">待计算的日期</param>
        /// <param name="startOfWeek">一周的起始日（默认为周一）</param>
        /// <returns>相对周数（可以为负数，表示第1周之前的周）</returns>
        public static int GetRelativeWeekNumber(DateTime referenceDate, DateTime targetDate, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            // 获取两个日期各自所在周的起始日
            DateTime refWeekStart = GetWeekStart(referenceDate, startOfWeek);
            DateTime targetWeekStart = GetWeekStart(targetDate, startOfWeek);

            // 计算起始日之间的天数差，再除以7得到周数差
            int daysDiff = (int)(targetWeekStart - refWeekStart).TotalDays;
            int weekDiff = daysDiff / 7;   // 应为整数，因为起始日都是同一周起始日

            // 相对周数 = 周数差 + 1
            return weekDiff + 1;
        }

        /// <summary>
        /// 获取日期所在周的起始日（根据指定的起始日）
        /// </summary>
        private static DateTime GetWeekStart(DateTime date, DayOfWeek startOfWeek)
        {
            int diff = (date.DayOfWeek - startOfWeek + 7) % 7;
            return date.AddDays(-diff).Date;
        }
    }
}
