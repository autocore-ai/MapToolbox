#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools
{
    public class AutoHeightLine : Line
    {
        readonly float maxHeight = 500;
        [Range(0, 1)]
        public float aboveGround = 0.2f;
        public override void SetPosition(int index, Vector3 position) => base.SetPosition(index, SetHeight(position, aboveGround, maxHeight));
        private Vector3 SetHeight(Vector3 point, float aboveGround, float maxHeight)
        {
            if (Physics.Raycast(new Ray(new Vector3(point.x, maxHeight, point.z), Vector3.down), out RaycastHit hit, maxHeight * 2))
            {
                return hit.point + Vector3.up * aboveGround;
            }
            else
            {
                return new Vector3(point.x, 0, point.z);
            }
        }
    }
}
