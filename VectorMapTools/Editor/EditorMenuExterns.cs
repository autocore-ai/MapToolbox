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

using Packages.MapToolbox.VectorMapTools.Export;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools.Editor
{
    class EditorMenuExterns
    {
        [MenuItem("Autoware Toolkit/Create VectorMap/Lane #q")]
        private static void CreateLane()
        {
            Selection.activeGameObject = new GameObject("New Lane");
            Selection.activeGameObject.AddComponent<Lane>().Pivot = (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.pivot;
            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "CreateLane");
        }
        [MenuItem("Autoware Toolkit/Create VectorMap/WhiteLine #w")]
        private static void CreateWhiteLine()
        {
            Selection.activeGameObject = new GameObject("New WhiteLine");
            Selection.activeGameObject.AddComponent<WhiteLine>().Pivot = (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.pivot;
            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "CreateWhiteLine");
        }
        [MenuItem("Autoware Toolkit/Create VectorMap/RoadEdge #e")]
        private static void CreateRoadEdge()
        {
            Selection.activeGameObject = new GameObject("New RoadEdge");
            Selection.activeGameObject.AddComponent<RoadEdge>().Pivot = (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.pivot;
            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "CreateRoadEdge");
        }
        [MenuItem("Autoware Toolkit/Create VectorMap/Curb #r")]
        private static void CreateCurb()
        {
            Selection.activeGameObject = new GameObject("New Curb");
            Selection.activeGameObject.AddComponent<Curb>().Pivot = (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.pivot;
            Undo.RegisterCreatedObjectUndo(Selection.activeGameObject, "CreateCurb");
        }
        [MenuItem("Autoware Toolkit/Create VectorMap/StopLine #a")]
        private static void CreateStopLine()
        {
            var go = new GameObject("New StopLine", typeof(StopLine));
            Undo.RegisterCreatedObjectUndo(go, "CreateStopLine");
            Selection.activeGameObject = go;
            (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.MoveToView(go.transform);
        }
        [MenuItem("Autoware Toolkit/Create VectorMap/SignalLight #s")]
        private static void CreateSignalLight()
        {
            var go = new GameObject("New SignalLight", typeof(SignalLight));
            Undo.RegisterCreatedObjectUndo(go, "CreateSignalLight");
            Selection.activeGameObject = go;
            (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.MoveToView(go.transform);
        }
        [MenuItem("Autoware Toolkit/Export VectorMap #m")]
        private static void ExportVectorMap()
        {
            var folder = EditorUtility.SaveFolderPanel("Save VectorMaps At", PlayerPrefs.GetString("ExportVectorMap", Directory.GetCurrentDirectory()), "");
            if (!string.IsNullOrEmpty(folder))
            {
                PlayerPrefs.SetString("ExportVectorMap", folder);
                new VectorMapCollection().WriteFiles(folder);
            }
        }
    }
}