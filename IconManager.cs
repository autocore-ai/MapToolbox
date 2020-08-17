using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox
{
    /// <summary>
    /// https://github.com/Thundernerd/Unity3D-IconManager
    /// MIT License
    /// </summary>
    static class IconManager
    {
        private static MethodInfo setIconForObjectMethodInfo;
        public enum LabelIcon
        {
            Gray,
            Blue,
            Teal,
            Green,
            Yellow,
            Orange,
            Red,
            Purple
        }
        public enum ShapeIcon
        {
            CircleGray,
            CircleBlue,
            CircleTeal,
            CircleGreen,
            CircleYellow,
            CircleOrange,
            CircleRed,
            CirclePurple,
            DiamondGray,
            DiamondBlue,
            DiamondTeal,
            DiamondGreen,
            DiamondYellow,
            DiamondOrange,
            DiamondRed,
            DiamondPurple
        }
        public static void SetIcon(this GameObject gameObject, LabelIcon labelIcon) => SetIcon(gameObject, $"sv_label_{(int)labelIcon}");

        public static void SetIcon(this GameObject gameObject, ShapeIcon shapeIcon) => SetIcon(gameObject, $"sv_icon_dot{(int)shapeIcon}_pix16_gizmo");

        private static void SetIcon(this GameObject gameObject, string contentName)
        {
            GUIContent iconContent = EditorGUIUtility.IconContent(contentName);
            SetIconForObject(gameObject, (Texture2D)iconContent.image);
        }

        public static void RemoveIcon(this GameObject gameObject) => SetIconForObject(gameObject, null);

        public static void SetIconForObject(this GameObject obj, Texture2D icon)
        {
            if (setIconForObjectMethodInfo == null)
            {
                Type type = typeof(EditorGUIUtility);
                setIconForObjectMethodInfo =
                    type.GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
            }
            setIconForObjectMethodInfo.Invoke(null, new object[] { obj, icon });
        }
    }
}