#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEditor;
using UnityEngine;

namespace Packages.UnityTools.VectorMapTools.Editor
{
    [CustomEditor(typeof(Line))]
    class LineEditor : UnityEditor.Editor
    {
        private void DuringSceneGUI(SceneView sv)
        {
            Tools.current = Tool.Custom;
            Line line = target as Line;
            for (int i = 0; i < line.Points.Length; i++)
            {
                line.SetPosition(i, Handles.PositionHandle(line.Points[i], Quaternion.identity));
            }
            Undo.RecordObject(line.LineRenderer, "Line Renderer Points");
        }
        void OnEnable() => SceneView.duringSceneGui += DuringSceneGUI;
        void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;
        //public override void OnInspectorGUI()
        //{
        //    base.OnInspectorGUI();
        //    Line line = target as Line;
        //    EditorGUI.BeginChangeCheck();
        //    line.StartWidth = EditorGUILayout.Slider("Start Width", line.StartWidth, 0.1f, 1);
        //    line.EndWidth = EditorGUILayout.Slider("End Width", line.EndWidth, 0.1f, 1);
        //    if (EditorGUI.EndChangeCheck())
        //    {
        //        Undo.RecordObject(line, "Line Width");
        //    }
        //}
    }
}
