#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.UnityTools.VectorMapTools
{
    public class Lane : AutoHeightBezier
    {
        static PointOctree<Lane> BeginPoints { get; set; } = new PointOctree<Lane>(1000, Vector3.zero, 100);
        internal static PointOctree<Lane> FinalPoints { get; set; } = new PointOctree<Lane>(1000, Vector3.zero, 100);
        public static List<Lane> List { get; set; } = new List<Lane>();
        [Range(0,20)]
        public float velocityBegin = 10;
        [Range(0, 20)]
        public float velocityFinal = 10;
        [Range(0, 5)]
        public float autoConnectDistance = 2;
        [Range(0, 5)]
        public float displayWidth = 2;
        public override Vector3 StartPoint
        {
            set
            {
                var laneToConnect = FinalPoints.GetNearby(value, autoConnectDistance)?.OrderBy(_ => Vector3.Distance(value, _.EndPoint))?.FirstOrDefault();
                if (laneToConnect)
                {
                    base.StartPoint = laneToConnect.EndPoint;
                }
                else
                {
                    base.StartPoint = value;
                }
                BeginPoints.Remove(this);
                BeginPoints.Add(this, StartPoint);
            }
        }
        public override Vector3 EndPoint
        {
            set
            {
                var laneToConnect = BeginPoints.GetNearby(value, autoConnectDistance)?.OrderBy(_ => Vector3.Distance(value, _.StartPoint))?.FirstOrDefault();
                if (laneToConnect)
                {
                    base.EndPoint = laneToConnect.StartPoint;
                }
                else
                {
                    base.EndPoint = value;
                }
                FinalPoints.Remove(this);
                FinalPoints.Add(this, EndPoint);
            }
        }
        protected override void Awake()
        {
            base.Awake();
            LineRenderer.textureMode = LineTextureMode.Tile;
            LineRenderer.sharedMaterial = Resources.Load<Material>("UnityToolsForAutoware/Lane");
            LineRenderer.startWidth = LineRenderer.endWidth = displayWidth;
        }
        protected override void Start()
        {
            base.Start();
            List.Add(this);
            BeginPoints.Add(this, StartPoint);
            FinalPoints.Add(this, EndPoint);
        }
        private void OnDestroy()
        {
            BeginPoints.Remove(this);
            FinalPoints.Remove(this);
            List.Remove(this);
        }
    }
}
