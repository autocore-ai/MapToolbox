#region License
/******************************************************************************
* Copyright 2019 The AutoCore Authors. All Rights Reserved.
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
        [HideInInspector] public Vector3 from;
        [HideInInspector] public Vector3 to;
        public Vector3 From
        {
            get => from;
            set
            {
                if (from != value)
                {
                    from = value;
                    UpdateRef();
                    if (last)
                    {
                        last.GetComponent<T>().to = from;
                    }
                    else
                    {
                        if (Parent.From != from)
                        {
                            Parent.From = from;
                        }
                    }
                    Parent.UpdateRenderer();
                }
            }
        }

        public Vector3 To
        {
            get => to;
            set
            {
                if (to != value)
                {
                    to = value;
                    UpdateRef();
                    if (next)
                    {
                        next.GetComponent<T>().from = to;
                    }
                    else
                    {
                        if (Parent.To != to)
                        {
                            Parent.To = to;
                        }
                    }
                    Parent.UpdateRenderer();
                }
            }
        }

        public override void UpdateRef()
        {
            base.UpdateRef();
            transform.position = (from + to) / 2;
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
                target.from = 2 * from - to;
            }
            else
            {
                var temp = last.GetComponent<T>();
                target.from = (temp.from + temp.to) / 2;
                temp.to = target.from;
            }
            target.to = from;
            Parent.UpdateRenderer();
            return target.gameObject;
        }
        internal override GameObject AddAfter()
        {
            var target = base.AddAfter().GetComponent<T>();
            if (next == null)
            {
                target.to = 2 * to - from;
            }
            else
            {
                var temp = next.GetComponent<T>();
                target.to = (temp.from + temp.to) / 2;
                temp.from = target.to;
            }
            target.from = to;
            Parent.UpdateRenderer();
            return target.gameObject;
        }

        internal void ApplyBezierPoints(Vector3[] points)
        {
            LineSegment<T> current = this;
            foreach (var point in points)
            {
                if (Vector3.Distance(current.from, point) > 1)
                {
                    Vector3 dest = current.from + (point - current.from).normalized;
                    current.to = dest;
                    var temp = current.AddAfter().GetComponent<LineSegment<T>>();
                    temp.DataCopy(current);
                    current = temp;
                    current.from = dest;
                }
            }
            current.To = points.Last();
        }

        protected virtual void DataCopy(LineSegment<T> current) => name = current.name;

        internal void Merge(T[] targets)
        {
            if (targets.Length > 1)
            {
                var temp = targets.OrderBy(_ => _.Index).ToArray();
                var first = temp.First();
                var last = temp.Last();
                first.To = last.To;
                if (last.next)
                {
                    last.next.GetComponent<LineSegment<T>>().From = first.To;
                }
                for (int i = 1; i < temp.Length; i++)
                {
                    DestroyImmediate(temp[i].gameObject);
                }
            }
        }
    }
}