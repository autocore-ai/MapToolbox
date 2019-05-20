#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Linq;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    public class DoubleConnectBezier : AutoHeightBezier
    {
        class PointObject { public Vector3 Position { get; set; } }
        readonly PointObject beginPointObject = new PointObject();
        readonly PointObject finalPointObject = new PointObject();
        static PointOctree<PointObject> PointObjects { get; set; } = new PointOctree<PointObject>(1000, Vector3.zero, 100);
        public float autoConnectDistance = 2;
        public override Vector3 StartPosition
        {
            set
            {
                base.StartPosition = value;
                if (!startPositionMove.Equals(Vector3.zero))
                {
                    var target = PointObjects.GetNearby(startPosition, autoConnectDistance)
                        ?.Where(_ => !_.Equals(beginPointObject) && !_.Equals(finalPointObject))
                        ?.OrderBy(_ => Vector3.Distance(startPosition, _.Position))
                        ?.FirstOrDefault();
                    if (target != null)
                    {
                        base.StartPosition = target.Position;
                    }
                }
                PointObjects.Remove(beginPointObject);
                PointObjects.Add(beginPointObject, startPosition);
                beginPointObject.Position = startPosition;
            }
        }
        public override Vector3 EndPosition
        {
            set
            {
                base.EndPosition = value;
                if (!endPositionMove.Equals(Vector3.zero))
                {
                    var target = PointObjects.GetNearby(endPosition, autoConnectDistance)
                        ?.Where(_ => !_.Equals(finalPointObject) && !_.Equals(beginPointObject))
                        ?.OrderBy(_ => Vector3.Distance(endPosition, _.Position))
                        ?.FirstOrDefault();
                    if (target != null)
                    {
                        base.EndPosition = target.Position;
                    }
                }
                PointObjects.Remove(finalPointObject);
                PointObjects.Add(finalPointObject, endPosition);
                finalPointObject.Position = endPosition;
            }
        }
        protected virtual void Start()
        {
            beginPointObject.Position = startPosition;
            finalPointObject.Position = endPosition;
            PointObjects.Add(beginPointObject, startPosition);
            PointObjects.Add(finalPointObject, endPosition);
        }
        protected virtual void OnDestroy()
        {
            PointObjects.Remove(beginPointObject);
            PointObjects.Remove(finalPointObject);
        }
    }
}