#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEditor;

namespace Packages.UnityTools.VectorMapTools.Editor
{
    [CustomEditor(typeof(SignalLight))]
    class SignalLightEditor : UnityEditor.Editor
    {
        private void DuringSceneGUI(SceneView sv)
        {
            SignalLight t = target as SignalLight;
            var position = t.transform.position;
            var rotation = t.transform.rotation;
            Handles.TransformHandle(ref position, ref rotation);
            t.transform.position = position;
            t.transform.rotation = rotation;
            Undo.RecordObject(t.transform, "SignalLight");
        }
        void OnEnable() => SceneView.duringSceneGui += DuringSceneGUI;
        void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;
    }
}
