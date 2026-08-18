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
using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using Path = System.IO.Path;

namespace SeatMapper.Setting.AppSetting.Pages
{
    /// <summary>
    /// MiscellaneousSettings.xaml 的交互逻辑
    /// </summary>
    public partial class MiscellaneousSettings : Page
    {
        private bool isInitialized = false;

        public MiscellaneousSettings()
        {
            InitializeComponent();
            AppThemeComboBox.SelectedIndex = GlobalVariables.json.AppTheme;
            SemesterStartDatePicker.Date = GlobalVariables.json.SemesterStartDate ?? DateTime.Now.Date;
            AttemptsTimes.Value = GlobalVariables.json.MaximumNumberOfAttempts;
            GiteeMode.IsChecked = GlobalVariables.json.GiteeMode ?? false;
            TimerInterval.Value=GlobalVariables.json.TimerInterval ?? 500;
            isInitialized = true;
            if (AttemptsTimes.Value <= 10)
            {
                AttemptsTimes.Value = 1000;
            }
        }

        private void AppThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInitialized)
            {
                GlobalVariables.json.AppTheme = AppThemeComboBox.SelectedIndex;
                SaveJson();
                if (AppThemeComboBox.SelectedIndex == 0)
                {
                    SystemThemeWatcher.Watch(Application.Current.MainWindow, Wpf.Ui.Controls.WindowBackdropType.Tabbed);
                    ApplicationThemeManager.ApplySystemTheme();
                }
                else
                {
                    SystemThemeWatcher.UnWatch(Application.Current.MainWindow);
                    if (AppThemeComboBox.SelectedIndex == 1)
                    {
                        ApplicationThemeManager.Apply(ApplicationTheme.Light, WindowBackdropType.Tabbed);
                    }
                    else if (AppThemeComboBox.SelectedIndex == 2)
                    {
                        ApplicationThemeManager.Apply(ApplicationTheme.Dark, WindowBackdropType.Tabbed);
                    }
                }
            }
        }

        private void SaveJson()
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

        private void SemesterStartDatePicker_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            MessageBox.Show("请使用鼠标左键点击选择日期，而不是右键点击。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SemesterStartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!isInitialized) return;
            GlobalVariables.json.SemesterStartDate = SemesterStartDatePicker.Date.Value;
            SaveJson();
        }

        private void AttemptsTimes_ValueChanged(object sender, NumberBoxValueChangedEventArgs args)
        {
            if (!isInitialized) return;
            GlobalVariables.json.MaximumNumberOfAttempts = AttemptsTimes.Value.Value.ToInt32();
            SaveJson();
        }

        private void GiteeMode_Click(object sender, RoutedEventArgs e)
        {
            if (!isInitialized) return;
            GlobalVariables.json.GiteeMode = GiteeMode.IsChecked ?? false;
            SaveJson();
        }

        private void TimerInterval_ValueChanged(object sender, NumberBoxValueChangedEventArgs args)
        {
            if (!isInitialized) return;
            GlobalVariables.json.TimerInterval = TimerInterval.Value.Value.ToInt32();
            SaveJson();
        }
    }
}