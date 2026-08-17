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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace SeatMapper.Setting.AppSetting.Pages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();
        }

        private async Task LoadImageFromWebAsync(string imageUrl, Image targetImage)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 异步获取图片字节流
                    byte[] imageData = await client.GetByteArrayAsync(imageUrl);

                    // 在内存流中创建图片
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        // 设置源为内存流
                        bitmapImage.StreamSource = ms;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.DecodePixelWidth = 400;
                        bitmapImage.EndInit();
                        // 图片解码后，通过Dispatcher切换到UI线程更新控件
                        this.Dispatcher.Invoke(() =>
                        {
                            targetImage.Source = bitmapImage;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                //跳过
            }
        }

        private void WriteToEventLog(string source, string logName, int eventId, string message)
        {

            using (EventLog eventLog = new EventLog())
            {
                eventLog.Source = source;
                eventLog.Log = logName; // 通常是 "Application" 或 "System"
                eventLog.WriteEntry(message, EventLogEntryType.Information, eventId);
            }


        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(GlobalVariables.json.GiteeMode ?? false)
            {
                await LoadImageFromWebAsync("https://foruda.gitee.com/avatar/1777196359746152210/15207534_haaa4_1777196359.png!avatar100", HeadImage);
                await LoadImageFromWebAsync("https://raw.giteeusercontent.com/haaa4/NameCube/raw/main/NameCube/icon.png?metadata=eyJyIjoibWFpbiIsImZwIjoiTmFtZUN1YmUvaWNvbi5wbmciLCJ1aWQiOjE1MjA3NTM0LCJwaWQiOjQ1MzY1MzYxLCJzdG8iOiJnaXQtc2hhcmRpbmctc3RvLTQydC0wMTQiLCJycCI6InJlcG9zLzhmL2RiLzhmZGJiNGM1MDdhYzQ1ZWY5NmIyZmY1ODU4ZDM3NTVhOGM3MjVkMDQ5MzQyM2I5OWQwZTE5M2QwOTE1MzExZmQuZ2l0IiwiaXNwIjp0cnVlLCJleHBpcmVfYXQiOjE3ODY5NTY2MDB9&signature=vIuuYREbx9tXE51ysZ8Sdjr-d96rAS3VgABcw1nG-vI", NameCubeIcon);
                await LoadImageFromWebAsync("https://raw.giteeusercontent.com/haaa4/DeskSweeper/raw/main/DeskSweeper.png?metadata=eyJyIjoibWFpbiIsImZwIjoiRGVza1N3ZWVwZXIucG5nIiwidWlkIjoxNTIwNzUzNCwicGlkIjo0OTc0NTg3NSwic3RvIjoiZ2l0LXNoYXJkaW5nLXN0by00MnQtMDE0IiwicnAiOiJyZXBvcy9hMS81OC9hMTU4YWUyYWU4ZTJmNWJkZmQxOWI5YTFmMmJlMTQ5OWNjN2FhZjM0ZDM3MWI0MjM1NWNmY2ZkNDhhYjcyMmRhLmdpdCIsImlzcCI6dHJ1ZSwiZXhwaXJlX2F0IjoxNzg2OTU3MjAwfQ&signature=hAgvqtSg5jMESkzD-IScmskcVQVGWLn9N0aOfYnoZq4", DeskSweeperIcon);
            }
            else
            {
                await LoadImageFromWebAsync("https://avatars.githubusercontent.com/u/172395030?v=4", HeadImage);
                await LoadImageFromWebAsync("https://raw.githubusercontent.com/haaa4/NameCube/refs/heads/main/NameCube/icon.png",NameCubeIcon);
                await LoadImageFromWebAsync("https://raw.githubusercontent.com/haaa4/DeskSweeper/refs/heads/main/DeskSweeper.png", DeskSweeperIcon);
            }
        }
    }
}
