#region License
/******************************************************************************
* Copyright 2019 The AutoCore Authors. All Rights Reserved.
* 
* Licensed under the GNU Lesser General Public License, Version 3.0 (the "License"); 
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
* https://www.gnu.org/licenses/lgpl-3.0.html
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*****************************************************************************/
#endregion

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox.PcdTools.Editor
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