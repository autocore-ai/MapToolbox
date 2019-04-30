#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.UnityTools.PcdTools.Editor
{
    class EditorMenuExterns
    {
        [MenuItem("Autoware Toolkit/Load Pcd Folder #p")]
        private static void LoadPcdFolder()
        {
            var folder = EditorUtility.OpenFolderPanel("Select pcd files folder", PlayerPrefs.GetString("LoadPcdFolder", Directory.GetCurrentDirectory()), "");
            if (!string.IsNullOrEmpty(folder))
            {
                PlayerPrefs.SetString("LoadPcdFolder", folder);
                var targetDic = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "pcd");
                if (!Directory.Exists(targetDic))
                {
                    Directory.CreateDirectory(targetDic);
                }
                foreach (var item in Directory.GetFiles(folder, "*.pcd"))
                {
                    File.Copy(item, Path.Combine(targetDic, Path.GetFileName(item)), true);
                }
                AssetDatabase.Refresh();
            }
        }
    }
}
