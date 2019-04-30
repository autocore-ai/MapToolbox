#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEditor;
using UnityEngine;

namespace Packages.UnityTools.VectorMapTools.Editor
{
    [CustomEditor(typeof(Bezier))]
    class BezierEditor : UnityEditor.Editor
    {
        private void DuringSceneGUI(SceneView sv)
        {
            Tools.current = Tool.Custom;
            Bezier be = target as Bezier;
            Undo.RecordObject(be, "Bezier");
            be.StartPoint = Handles.PositionHandle(be.StartPoint, Quaternion.identity);
            be.EndPoint = Handles.PositionHandle(be.EndPoint, Quaternion.identity);
            be.StartTangent = Handles.PositionHandle(be.StartTangent, Quaternion.identity);
            be.EndTangent = Handles.PositionHandle(be.EndTangent, Quaternion.identity);
            if (!be.StartPoint.Equals(Vector3.zero) && !be.EndPoint.Equals(Vector3.zero))
            {
                be.Points = Handles.MakeBezierPoints(be.StartPoint, be.EndPoint, be.StartTangent, be.EndTangent, Mathf.CeilToInt(Vector3.Distance(be.StartPoint, be.EndPoint)));
            }
        }
        void OnEnable() => SceneView.duringSceneGui += DuringSceneGUI;
        void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;
    }
}
