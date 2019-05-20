#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    public class AutoHeightLine : Line
    {
        readonly float maxHeight = 500;
        public override void SetPosition(int index, Vector3 position) => base.SetPosition(index, SetHeight(position, maxHeight));
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