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
    public class Curb : AutoHeightBezier
    {
        class PointObject { public Vector3 Position { get; set; } }
        class BeginPointObject : PointObject { }
        class FinalPointObject : PointObject { }
        readonly BeginPointObject beginPointObject = new BeginPointObject();
        readonly FinalPointObject finalPointObject = new FinalPointObject();
        static PointOctree<PointObject> PointObjects { get; set; } = new PointOctree<PointObject>(1000, Vector3.zero, 100);
        public float autoConnectDistance = 2;
        [Range(0, 1)]
        public float displayWidth = 0.2f;
        public static List<Curb> List { get; set; } = new List<Curb>();
        public override Vector3 StartPoint
        {
            set
            {
                var closest = PointObjects.GetNearby(value, autoConnectDistance)?.Where(_ => !_.Equals(beginPointObject))?.OrderBy(_ => Vector3.Distance(value, _.Position))?.FirstOrDefault();
                if (closest != null)
                {
                    base.StartPoint = closest.Position;
                }
                else
                {
                    base.StartPoint = value;
                }
                beginPointObject.Position = StartPoint;
                PointObjects.Remove(beginPointObject);
                PointObjects.Add(beginPointObject, StartPoint);
            }
        }
        public override Vector3 EndPoint
        {
            set
            {
                var closest = PointObjects.GetNearby(value, autoConnectDistance)?.Where(_ => !_.Equals(finalPointObject))?.OrderBy(_ => Vector3.Distance(value, _.Position))?.FirstOrDefault();
                if (closest != null)
                {
                    base.EndPoint = closest.Position;
                }
                else
                {
                    base.EndPoint = value;
                }
                finalPointObject.Position = EndPoint;
                PointObjects.Remove(finalPointObject);
                PointObjects.Add(finalPointObject, EndPoint);
            }
        }
        protected override void Awake()
        {
            base.Awake();
            List.Add(this);
            LineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.cyan
            };
            LineRenderer.startWidth = LineRenderer.endWidth = displayWidth;
        }
        protected override void Start()
        {
            base.Start();
            beginPointObject.Position = StartPoint;
            finalPointObject.Position = EndPoint;
            PointObjects.Add(beginPointObject, StartPoint);
            PointObjects.Add(finalPointObject, EndPoint);
        }
        private void OnDestroy()
        {
            List.Remove(this);
            PointObjects.Remove(beginPointObject);
            PointObjects.Remove(finalPointObject);
        }
    }
}
