#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    [RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
    public class LRMapElement : MonoBehaviour
    {
        public LineRenderer LineRenderer { get; private set; }
        [HideInInspector] public float width = 1;
        internal float WidthMin { get; set; } = 0.1f;
        internal float WidthMax { get; set; } = 1;
        internal virtual Vector3? Pivot { get; set; }
        protected virtual void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
            SetLineRendererWidth(width);
        }
        internal void SetLineRendererWidth(float width)
        {
            if (LineRenderer)
            {
                LineRenderer.startWidth = LineRenderer.endWidth = width;
            }
        }
    }
}