#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEditor;

namespace Packages.AutowareUnityTools.VectorMapTools.Editor
{
    [CustomEditor(typeof(LRMapElement))]
    class LRMapElementEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LRMapElement t = target as LRMapElement;
            EditorGUI.BeginChangeCheck();
            t.width = EditorGUILayout.Slider("Width", t.width, t.WidthMin, t.WidthMax);
            if (EditorGUI.EndChangeCheck())
            {
                t.SetLineRendererWidth(t.width);
                EditorUtility.SetDirty(t);
            }
        }
    }
}