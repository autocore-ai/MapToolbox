#region License
/******************************************************************************
* Copyright 2018-2020 The AutoCore Authors. All Rights Reserved.
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


using AutoCore.MapToolbox.Autoware;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace AutoCore.MapToolbox.Editor.Autoware
{
    [CustomEditor(typeof(AutowareADASMap))]
    class AutowareADASMapEditor : UnityEditor.Editor
    {
        [MenuItem("GameObject/Autoware/AutowareADASMap", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            GameObject go = new GameObject(typeof(AutowareADASMap).Name);
            go.AddComponent<AutowareADASMap>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create Root" + go.name);
            Selection.activeObject = go;
        }
        const string LoadFromFolder = "Load Autoware ADASMap from folder";
        const string SaveToFolder = "Save Autoware ADASMap to folder";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.color = Color.yellow;
            if (GUILayout.Button(Const.AddLane))
            {
                (target as AutowareADASMap).AddLane(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddSignal))
            {
                (target as AutowareADASMap).AddSignal(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddStopLine))
            {
                (target as AutowareADASMap).AddStopLine(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddWhiteLine))
            {
                (target as AutowareADASMap).AddWhiteLine(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddRoadEdge))
            {
                (target as AutowareADASMap).AddRoadEdge(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddCrossWalk))
            {
                (target as AutowareADASMap).AddCrossWalk(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddRoadMark))
            {
                (target as AutowareADASMap).AddRoadMark(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddWayArea))
            {
                (target as AutowareADASMap).AddWayArea(SceneView.lastActiveSceneView.pivot);
            }
            else if (GUILayout.Button(Const.AddCustomArea))
            {
                (target as AutowareADASMap).AddCustomArea(SceneView.lastActiveSceneView.pivot);
            }
            GUI.color = Color.green;
            if (GUILayout.Button(LoadFromFolder))
            {
                var folder = EditorUtility.OpenFolderPanel(LoadFromFolder, PlayerPrefs.GetString(LoadFromFolder, Application.dataPath), string.Empty);
                if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                {
                    PlayerPrefs.SetString(LoadFromFolder, folder);
                    (target as AutowareADASMap).Load(folder);
                    Undo.IncrementCurrentGroup();
                    Undo.SetCurrentGroupName(LoadFromFolder);
                    int undoGroupIndex = Undo.GetCurrentGroup();
                    foreach (var go in (target as AutowareADASMap).GetComponentsInChildren<Collection>().Select(_ => _.gameObject))
                    {
                        Undo.RegisterCreatedObjectUndo(go, string.Empty);
                    }
                    Undo.CollapseUndoOperations(undoGroupIndex);
                }
            }
            GUI.color = Color.red;
            if (GUILayout.Button(SaveToFolder))
            {
                var folder = EditorUtility.OpenFolderPanel(SaveToFolder, PlayerPrefs.GetString(SaveToFolder, Application.dataPath), string.Empty);
                if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
                {
                    PlayerPrefs.SetString(SaveToFolder, folder);
                    (target as AutowareADASMap).Save(folder);
                }
            }
        }
    }
}