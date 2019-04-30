#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using Packages.UnityTools.VectorMapTools.Export;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Packages.UnityTools.VectorMapTools.Editor
{
    class EditorMenuExterns
    {
        [MenuItem("Autoware Toolkit/Create VectorMap/Lane #a")]
        private static void CreateLane()
        {
            var go = new GameObject("New Lane", typeof(Lane));
            Undo.RegisterCreatedObjectUndo(go, "CreateLane");
            Selection.activeGameObject = go;
            (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.MoveToView(go.transform);
        }

        [MenuItem("Autoware Toolkit/Create VectorMap/WhiteLine #w")]
        private static void CreateWhiteLine()
        {
            var go = new GameObject("New WhiteLine", typeof(WhiteLine));
            Undo.RegisterCreatedObjectUndo(go, "CreateWhiteLine");
            Selection.activeGameObject = go;
            (SceneView.currentDrawingSceneView ?? SceneView.lastActiveSceneView)?.MoveToView(go.transform);
        }

        [MenuItem("Autoware Toolkit/Create VectorMap/StopLine #s")]
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
            var folder = EditorUtility.SaveFolderPanel("Save VectorMap At", PlayerPrefs.GetString("ExportVectorMap", Directory.GetCurrentDirectory()), "");
            if (!string.IsNullOrEmpty(folder))
            {
                PlayerPrefs.SetString("ExportVectorMap", folder);
                VectorMapExporter.ExportMaps(folder);
            }
        }
    }
}
