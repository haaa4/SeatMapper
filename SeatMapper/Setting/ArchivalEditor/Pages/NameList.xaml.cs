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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using Path= System.IO.Path;

namespace SeatMapper.Setting.ArchivalEditor.Pages
{
    /// <summary>
    /// NameList.xaml 的交互逻辑
    /// </summary>
    public partial class NameList : Page
    {
        public ObservableCollection<string> MaleNames { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> FemaleNames { get; set; } = new ObservableCollection<string>();
        public NameList()
        {
            InitializeComponent();
            DataContext = this;
            foreach (string name in GlobalVariables.MaleList)
            {
                MaleNames.Add(name);
            }
            foreach (string name in GlobalVariables.FemaleList)
            {
                FemaleNames.Add(name);
            }
            if (MaleNames.Count == 0)
            {
                MaleEmptyTextBlock.Visibility= Visibility.Visible;
            }
            else
            {
                MaleEmptyTextBlock.Visibility= Visibility.Hidden;
            }
            if(FemaleNames.Count == 0)
            {
                FemaleEmptyTextBlock.Visibility= Visibility.Visible;
            }
            else
            {
                FemaleEmptyTextBlock.Visibility= Visibility.Hidden;
            }
        }

        private void MaleEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(GlobalVariables.DataPath, "男.txt")) == false)
            {
                File.Create(Path.Combine(GlobalVariables.DataPath, "男.txt")).Close();
            }
            Process.Start("notepad.exe", Path.Combine(GlobalVariables.DataPath, "男.txt"));
        }

        private void FemaleEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(GlobalVariables.DataPath, "女.txt")) == false)
            {
                File.Create(Path.Combine(GlobalVariables.DataPath, "女.txt")).Close();
            }
            Process.Start("notepad.exe", Path.Combine(GlobalVariables.DataPath, "女.txt"));
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(GlobalVariables.DataPath, "男.txt")) == false)
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
            MaleNames.Clear();
            foreach (string name in GlobalVariables.MaleList)
            {
                MaleNames.Add(name);
            }
            FemaleNames.Clear();
            foreach (string name in GlobalVariables.FemaleList)
            {
                FemaleNames.Add(name);
            }
            if (MaleNames.Count == 0)
            {
                MaleEmptyTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                MaleEmptyTextBlock.Visibility = Visibility.Hidden;
            }
            if (FemaleNames.Count == 0)
            {
                FemaleEmptyTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                FemaleEmptyTextBlock.Visibility = Visibility.Hidden;
            }
        }
    }
}
