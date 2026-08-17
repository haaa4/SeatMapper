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
using System.Windows;

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
            TitleTextBlock.Text = title ?? "选择以继续";
            if (selections == null || selections.Count == 0)
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