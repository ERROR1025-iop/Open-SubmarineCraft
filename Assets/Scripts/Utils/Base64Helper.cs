using System;
using System.IO;
using Scraft;
using UnityEngine;
using UnityEngine.UI;

public static class Base64Helper
{
    /// <summary>
    /// 将指定路径的文件转换为Base64编码的字符串
    /// </summary>
    /// <param name="filePath">文件的路径</param>
    /// <returns>Base64编码的字符串，如果出错则返回null</returns>
    public static string FileToBase64String(string filePath)
    {
        try
        {
            // 检查文件是否存在
            if (!File.Exists(filePath))
            {
                Debug.LogError($"文件不存在: {filePath}");
                return null;
            }

            // 读取文件为字节数组
            byte[] fileData = File.ReadAllBytes(filePath);

            // 将字节数组转换为Base64字符串
            string base64String = Convert.ToBase64String(fileData);

            return base64String;
        }
        catch (Exception e)
        {
            Debug.LogError($"读取文件并转换为Base64时出错: {e.Message}");
            return null;
        }
    }

    public static bool SetImageFromBase64(string base64String, Image image, Vector2 size)
    {
        try
        {
            // 1. 移除可能存在的 Base64 数据 URL 头部（如：data:image/png;base64,）
            if (base64String.Contains(","))
            {
                base64String = base64String.Substring(base64String.IndexOf(',') + 1);
            }

            // 2. 将 Base64 字符串解码为字节数组
            byte[] imageData = Convert.FromBase64String(base64String);

            // 3. 创建 Texture2D 并加载图像数据
            Texture2D texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            if (!texture.LoadImage(imageData))
            {
                Debug.LogError("无法加载图像数据，可能是不支持的格式或损坏的数据。");
                return false;
            }

            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            IUtils.resetImageSize(image, size);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Base64 转换为 Image 失败: " + e.Message);
            return false;
        }
    }

    public static void SaveBase64StringToFile(string base64String, string outputPath)
    {
        try
        {
            // 检查base64String是否包含data URL头部（如果有，则移除）
            if (base64String.Contains(","))
            {
                base64String = base64String.Substring(base64String.IndexOf(',') + 1);
            }

            // 将base64字符串转换为字节数组
            byte[] fileBytes = Convert.FromBase64String(base64String);

            // 确保输出目录存在
            string directoryPath = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // 将字节数组写入到指定路径
            File.WriteAllBytes(outputPath, fileBytes);

            Debug.Log("文件已成功保存至: " + outputPath);
        }
        catch (Exception e)
        {
            Debug.LogError("保存文件时出错: " + e.Message);
        }
    }
}