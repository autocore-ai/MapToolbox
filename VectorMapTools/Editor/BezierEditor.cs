#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEditor;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools.Editor
{
    [CustomEditor(typeof(Bezier))]
    class BezierEditor : LRMapElementEditor
    {
        internal virtual void OnSceneGUI()
        {
            Tools.current = Tool.Custom;
            Bezier t = target as Bezier;
            EditorGUI.BeginChangeCheck();
            t.StartPosition = Handles.PositionHandle(t.startPosition, Quaternion.identity);
            t.EndPosition = Handles.PositionHandle(t.endPosition, Quaternion.identity);
            t.startTangent = Handles.PositionHandle(t.startTangent, Quaternion.identity);
            t.endTangent = Handles.PositionHandle(t.endTangent, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(t);
                if (!t.startPosition.Equals(t.endPosition))
                    t.Points = Handles.MakeBezierPoints(t.startPosition, t.endPosition, t.startTangent, t.endTangent, Mathf.CeilToInt(Vector3.Distance(t.startPosition, t.endPosition)));
            }
        }
    }
}