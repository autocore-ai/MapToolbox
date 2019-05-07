#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools
{
    [RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
    public class Line : MonoBehaviour
    {
        [SerializeField]
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
        public LineRenderer LineRenderer { get; private set; }
        [SerializeField]
        public int Count
        {
            get => LineRenderer.positionCount;
            set => LineRenderer.positionCount = value;
        }
        [SerializeField]
        public float StartWidth
        {
            get => LineRenderer.startWidth;
            set => LineRenderer.startWidth = value;
        }
        [SerializeField]
        public float EndWidth
        {
            get => LineRenderer.endWidth;
            set => LineRenderer.endWidth = value;
        }
        protected virtual void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
        }
        protected virtual void Start()
        {
            if (LineRenderer.positionCount == 2 && LineRenderer.GetPosition(0).Equals(Vector3.zero) && LineRenderer.GetPosition(1).Equals(Vector3.forward))
            {
                LineRenderer.SetPosition(0, transform.position);
                LineRenderer.SetPosition(1, transform.position + Vector3.right);
            }
        }
        public virtual void SetPosition(int index, Vector3 position)
        {
            if (index == 0)
            {
                transform.position = position;
            }
            else if (index > 1 && position.Equals(Vector3.zero))
            {
                position = LineRenderer.GetPosition(index - 1) + (LineRenderer.GetPosition(index - 1) - LineRenderer.GetPosition(index - 2));
            }
            LineRenderer.SetPosition(index, position);
        }
    }
}
