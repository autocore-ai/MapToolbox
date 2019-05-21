#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using Packages.AutowareUnityTools.VectorMapTools.Export;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools.Editor
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
                VectorMapExporter.ExportMaps(folder);
            }
        }
    }
}