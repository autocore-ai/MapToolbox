#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    public class Bezier : LRMapElement
    {
        [HideInInspector]
        public Vector3 startPosition;
        [HideInInspector]
        public Vector3 endPosition;
        [HideInInspector]
        public Vector3 startTangent;
        [HideInInspector] 
        public Vector3 endTangent;
        protected Vector3 startPositionMove;
        protected Vector3 endPositionMove;
        public virtual Vector3 StartPosition
        {
            set
            {
                startPositionMove = value - startPosition;
                startPosition = value;
                startTangent += startPositionMove;
            }
        }
        public virtual Vector3 EndPosition
        {
            set
            {
                endPositionMove = value - endPosition;
                endPosition = value;
                endTangent += endPositionMove;
            }
        }
        internal override Vector3? Pivot
        {
            set
            {
                base.Pivot = value;
                startPosition += value ?? Vector3.zero;
                endPosition += value ?? Vector3.zero;
                startTangent += value ?? Vector3.zero;
                endTangent += value ?? Vector3.zero;
            }
        }
        public virtual Vector3[] Points
        {
            set
            {
                LineRenderer.positionCount = value.Length;
                LineRenderer.SetPositions(value);
            }
        }
    }
}