#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools
{
    public class AutoHeightBezier : Bezier
    {
        readonly float maxHeight = 500;
        public override Vector3 StartPoint { set => base.StartPoint = SetHeight(value, maxHeight); }
        public override Vector3 EndPoint { set => base.EndPoint = SetHeight(value, maxHeight); }
        public override Vector3 StartTangent { set => base.StartTangent = SetHeight(value, maxHeight); }
        public override Vector3 EndTangent { set => base.EndTangent = SetHeight(value, maxHeight); }
        public override Vector3[] Points
        {
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    value[i] = SetHeight(value[i], maxHeight);
                }
                base.Points = value;
            }
        }
        private Vector3 SetHeight(Vector3 point, float maxHeight)
        {
            if (Physics.Raycast(new Ray(new Vector3(point.x, maxHeight, point.z), Vector3.down), out RaycastHit hit, maxHeight * 2))
            {
                return hit.point;
            }
            else
            {
                return new Vector3(point.x, 0, point.z);
            }
        }
    }
}
