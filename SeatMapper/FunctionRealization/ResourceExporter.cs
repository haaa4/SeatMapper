using System;
using System.IO;
using System.Reflection;
using System.Windows;

public static class ResourceExporter
{
    /// <summary>
    /// 将指定的嵌入资源导出到目标文件路径
    /// </summary>
    /// <param name="resourceName">资源在程序集中的完整名称（含命名空间）</param>
    /// <param name="outputPath">目标文件完整路径（包含文件名）</param>
    /// <param name="overwrite">若目标文件已存在，是否覆盖</param>
    /// <returns>是否成功导出</returns>
    public static bool ExportResource(string resourceName, string outputPath, bool overwrite = false)
    {
        if (string.IsNullOrEmpty(resourceName) || string.IsNullOrEmpty(outputPath))
            return false;

        if (File.Exists(outputPath) && !overwrite)
            return false;

        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    throw new FileNotFoundException($"资源 '{resourceName}' 在当前程序集中不存在。");
                }

                string directory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream);
                }
                return true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"导出资源失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}