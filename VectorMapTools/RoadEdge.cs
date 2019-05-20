#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Collections.Generic;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    public class RoadEdge : DoubleConnectBezier
    {
        public static List<RoadEdge> List { get; set; } = new List<RoadEdge>();
        protected override void Awake()
        {
            base.Awake();
            List.Add(this);
            LineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.cyan
            };
        }
        protected override void Start()
        {
            base.Start();
            List.Add(this);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            List.Remove(this);
        }
    }
}