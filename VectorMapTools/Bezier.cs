#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools
{
    [RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
    public class Bezier : MonoBehaviour
    {
        public Vector3 startTangentOffset;
        public Vector3 endTangentOffset;
        public Vector3 endPosition;
        public virtual Vector3 StartPoint
        {
            get => transform.position;
            set => transform.position = value;
        }
        public virtual Vector3 EndPoint
        {
            get => endPosition;
            set => endPosition = value;
        }
        public virtual Vector3 StartTangent
        {
            get => transform.position + startTangentOffset;
            set => startTangentOffset = value - transform.position;
        }
        public virtual Vector3 EndTangent
        {
            get => EndPoint + endTangentOffset;
            set => endTangentOffset = value - EndPoint;
        }
        public virtual Vector3[] Points
        {
            get
            {
                var ret = new Vector3[LineRenderer.positionCount];
                LineRenderer.GetPositions(ret);
                return ret;
            }
            set
            {
                LineRenderer.positionCount = value.Length;
                LineRenderer.SetPositions(value);
            }
        }
        protected LineRenderer LineRenderer { get; private set; }
        protected virtual void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
        }
        protected virtual void Start()
        {
            if (!StartPoint.Equals(Vector3.zero) && EndPoint.Equals(Vector3.zero))
            {
                EndPoint = StartPoint + Vector3.right * 20;
                StartTangent = StartPoint + Vector3.right * 5;
                EndTangent = EndPoint + Vector3.left * 5;
            }
        }
    }
}
