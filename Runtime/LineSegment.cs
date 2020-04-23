#region License
/******************************************************************************
* Copyright 2018-2020 The AutoCore Authors. All Rights Reserved.
* 
* Licensed under the GNU Lesser General Public License, Version 3.0 (the "License"); 
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
* https://www.gnu.org/licenses/lgpl-3.0.html
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*****************************************************************************/
#endregion


using System.Linq;
using UnityEngine;

namespace AutoCore.MapToolbox
{
    [ExecuteInEditMode]
    class LineSegment<T> : SiblingChild<T> where T : LineSegment<T>
    {
        BrokenLineRenderer<T> parent;
        public BrokenLineRenderer<T> Parent
        {
            get
            {
                if (parent == null)
                {
                    parent = GetComponentInParent<BrokenLineRenderer<T>>();
                }
                return parent;
            }
        }
        [HideInInspector] public Vector3 localFrom;
        [HideInInspector] public Vector3 localTo;
        public Vector3 LocalFrom
        {
            get => localFrom;
            set
            {
                if (localFrom != value)
                {
                    localFrom = value;
                    UpdateRef();
                    if (last)
                    {
                        last.GetComponent<T>().localTo = localFrom;
                    }
                    else
                    {
                        if (Parent.LocalFrom != localFrom)
                        {
                            Parent.LocalFrom = localFrom;
                        }
                    }
                    Parent.UpdateRenderer();
                }
            }
        }

        public Vector3 LocalTo
        {
            get => localTo;
            set
            {
                if (localTo != value)
                {
                    localTo = value;
                    UpdateRef();
                    if (next)
                    {
                        next.GetComponent<T>().localFrom = localTo;
                    }
                    else
                    {
                        if (Parent.LocalTo != localTo)
                        {
                            Parent.LocalTo = localTo;
                        }
                    }
                    Parent.UpdateRenderer();
                }
            }
        }

        public virtual Vector3 From
        {
            set => LocalFrom = transform.parent.InverseTransformPoint(value);
            get => transform.parent.TransformPoint(LocalFrom);
        }

        public virtual Vector3 To
        {
            set => LocalTo = transform.parent.InverseTransformPoint(value);
            get => transform.parent.TransformPoint(LocalTo);
        }

        public override void UpdateRef()
        {
            base.UpdateRef();
            transform.localPosition = (localFrom + localTo) / 2;
        }

        private void OnDestroy()
        {
            if (Parent != null && Parent.isActiveAndEnabled)
            {
                Parent.UpdateRenderer();
            }
        }
        internal override GameObject AddBefore()
        {
            var target = base.AddBefore().GetComponent<T>();
            if (last == null)
            {
                target.localFrom = 2 * localFrom - localTo;
            }
            else
            {
                var temp = last.GetComponent<T>();
                target.localFrom = (temp.localFrom + temp.localTo) / 2;
                temp.localTo = target.localFrom;
            }
            target.localTo = localFrom;
            target.DataCopy(this);
            Parent.UpdateRenderer();
            return target.gameObject;
        }
        internal override GameObject AddAfter()
        {
            var target = base.AddAfter().GetComponent<T>();
            if (next == null)
            {
                target.localTo = 2 * localTo - localFrom;
            }
            else
            {
                var temp = next.GetComponent<T>();
                target.localTo = (temp.localFrom + temp.localTo) / 2;
                temp.localFrom = target.localTo;
            }
            target.localFrom = localTo;
            target.DataCopy(this);
            Parent.UpdateRenderer();
            return target.gameObject;
        }

        internal virtual void AutoSubdivision()
        {
            UpdateRef();
            if (last != null && next != null)
            {
                var refLast = last.GetComponent<LineSegment<T>>();
                var refNext = next.GetComponent<LineSegment<T>>();
                if (refLast && refNext)
                {
                    Vector3 lastDir = refLast.To - refLast.From;
                    Vector3 nextDir = refNext.To - refNext.From;
                    if (Utils.ClosestPointsOnTwoLines(out Vector3 p1, out Vector3 p2,
                        refLast.From, lastDir, refNext.From, nextDir))
                    {
                        Subdivision((refLast.To + p1) / 2, (refNext.From + p2) / 2);
                    }
                    else
                    {
                        Subdivision((refLast.To + refNext.From) / 2, (refLast.To + refNext.From) / 2);
                    }
                }
            }
        }

        internal virtual void Subdivision(Vector3 startTangent, Vector3 endTangent, float distance = 1)
        {
#if UNITY_EDITOR
            ApplySubdivisionPoints(UnityEditor.Handles.MakeBezierPoints(From, To, startTangent, endTangent, (int)(From - To).magnitude * 10), distance);
#endif
        }

        internal void ApplySubdivisionPoints(Vector3[] points, float distance = 1)
        {
            LineSegment<T> current = this;
            foreach (var point in points)
            {
                if (Vector3.Distance(current.From, point) > distance)
                {
                    Vector3 dest = current.From + (point - current.From).normalized * distance;
                    current.To = dest;
                    var temp = current.AddAfter().GetComponent<LineSegment<T>>();
                    temp.DataCopy(current);
                    current = temp;
                    current.From = dest;
                }
            }
            current.To = points.Last();
        }

        protected virtual void DataCopy(LineSegment<T> target) => name = target.name;

        internal void Merge(T[] targets)
        {
            if (targets.Length > 1)
            {
                var temp = targets.OrderBy(_ => _.Index).ToArray();
                var first = temp.First();
                var last = temp.Last();
                first.LocalTo = last.LocalTo;
                first.next = last.next;
                if (last.next)
                {
                    last.next.GetComponent<LineSegment<T>>().LocalFrom = first.LocalTo;
                }
                for (int i = 1; i < temp.Length; i++)
                {
                    DestroyImmediate(temp[i].gameObject);
                }
            }
        }
    }
}