#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Collections.Generic;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    class WhiteLine : DoubleConnectBezier
    {
        public static List<WhiteLine> List { get; set; } = new List<WhiteLine>();
        protected override void Awake()
        {
            base.Awake();
            List.Add(this);
            LineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.white
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