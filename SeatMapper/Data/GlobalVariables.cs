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
using System.IO;

namespace SeatMapper
{
    public static class GlobalVariables
    {
        public static string DataPath { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "User");

        /// <summary>
        /// 男生名单
        /// </summary>
        public static List<string> MaleList { get; set; }

        /// <summary>
        /// 女生名单
        /// </summary>
        public static List<string> FemaleList { get; set; }

        /// <summary>
        /// 储存在json文件中的数据
        /// </summary>
        public static Json json { get; set; }
    }

    public class Json
    {
        /// <summary>
        /// 启用国内镜像地址
        /// </summary>
        public bool? GiteeMode { get; set; }

        /// <summary>
        /// 黑名单
        /// </summary>
        public List<BlackList> BlackLists { get; set; }

        /// <summary>
        /// 固定文字1
        /// </summary>
        public string FixedText1 { get; set; }

        /// <summary>
        /// 固定文字2
        /// </summary>
        public string FixedText2 { get; set; }

        /// <summary>
        /// 固定文字3
        /// </summary>
        public string FixedText3 { get; set; }

        /// <summary>
        /// 应用主题（0：跟随系统，1：浅色，2：深色）
        /// </summary>
        public int AppTheme { get; set; }

        /// <summary>
        /// 学期开始日期
        /// </summary>
        public DateTime? SemesterStartDate { get; set; }

        /// <summary>
        /// 最大尝试次数
        /// </summary>
        public int MaximumNumberOfAttempts { get; set; }
    }

    public class BlackList
    {
        public string Name1 { get; set; }
        public string Name2 { get; set; }
    }
}