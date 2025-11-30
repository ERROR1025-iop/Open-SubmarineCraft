#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class RenameFilesInSubfolders : EditorWindow
{
    private Object targetFolder;
    private string statusMessage = "请选择一个 Assets 下的文件夹";

    [MenuItem("Tools/Rename Files in Subfolders")]
    public static void ShowWindow()
    {
        GetWindow<RenameFilesInSubfolders>("批量重命名");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量重命名子文件夹中的文件", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();

        targetFolder = EditorGUILayout.ObjectField(
            "目标文件夹",
            targetFolder,
            typeof(Object),
            false
        ) as Object;

        EditorGUILayout.Space();

        if (GUILayout.Button("开始重命名"))
        {
            if (targetFolder == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个文件夹！", "确定");
                return;
            }

            string folderPath = AssetDatabase.GetAssetPath(targetFolder);
            if (!Directory.Exists(Application.dataPath + "/" + folderPath.Substring(6)))
            {
                EditorUtility.DisplayDialog("错误", "请选择一个有效的文件夹！", "确定");
                return;
            }

            RenameFiles(folderPath);
        }

        EditorGUILayout.Space();
        GUILayout.Label(statusMessage);
    }

    private void RenameFiles(string parentFolderPath)
    {
        // 获取所有子文件夹
        string[] allSubFolders = AssetDatabase.GetSubFolders(parentFolderPath);

        // 筛选出“直接子文件夹”（只比 parent 多一级）
        var directSubFolders = allSubFolders
            .Where(subFolder => GetDepth(subFolder, parentFolderPath) == 1)
            .ToArray();

        if (directSubFolders.Length == 0)
        {
            statusMessage = "没有找到直接子文件夹！";
            return;
        }

        statusMessage = "正在处理...";

        // 开始资产编辑
        AssetDatabase.StartAssetEditing();

        try
        {
            for (int i = 0; i < directSubFolders.Length; i++)
            {
                string subFolder = directSubFolders[i];
                string[] guids = AssetDatabase.FindAssets("", new string[] { subFolder });

                foreach (string guid in guids)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(guid);
                    if (Directory.Exists(filePath)) continue; // 跳过文件夹

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    string extension = Path.GetExtension(filePath); // 包含点号 ".png"
                    string directoryPath = Path.GetDirectoryName(filePath);
                    string newName = $"{fileNameWithoutExtension}{i}{extension}";

                    string newFilePath = Path.Combine(directoryPath, newName).Replace("\\", "/");

                    // 检查目标文件是否已存在（物理层面）
                    string fullPath = Application.dataPath + "/" + newFilePath.Substring(6);
                    if (File.Exists(fullPath))
                    {
                        Debug.LogWarning($"跳过文件（目标已存在）: {newFilePath}");
                        continue;
                    }

                    // 使用 AssetDatabase.RenameAsset，返回错误信息字符串
                    string error = AssetDatabase.RenameAsset(filePath, newName);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError($"重命名失败: {filePath} -> {newName}\n错误: {error}");
                    }
                }
            }

            statusMessage = "重命名完成！";
        }
        catch (System.Exception e)
        {
            statusMessage = "发生错误！";
            Debug.LogException(e);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); // 刷新以更新项目视图
        }
    }

    // 计算子文件夹相对于父文件夹的深度
    private int GetDepth(string subFolder, string parentFolderPath)
    {
        string relative = subFolder.Substring(parentFolderPath.Length).Trim('/');
        int depth = relative.Split('/').Length;
        return depth;
    }
}

#endif