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
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

public static class AdminHelper
{
    /// <summary>
    /// 检查当前进程是否拥有管理员权限。
    /// </summary>
    public static bool IsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    /// <summary>
    /// 以管理员权限重新启动当前程序。
    /// </summary>
    /// <remarks>
    /// 该方法会弹出 UAC 对话框请求权限。
    /// 如果用户拒绝，则捕获异常并返回；否则退出当前进程。
    /// </remarks>
    public static void RestartAsAdmin()
    {
        string exePath = Process.GetCurrentProcess().MainModule.FileName;

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = exePath,
            Verb = "runas"
        };

        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            startInfo.Arguments = string.Join(" ", args, 1, args.Length - 1);
        }

        try
        {
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"无法获得管理员权限，程序将以普通权限继续运行。\n错误信息：{ex.Message}",
                            "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Environment.Exit(0);
            return;
        }

        // 启动新进程后，关闭当前进程
        Environment.Exit(0);
    }
}