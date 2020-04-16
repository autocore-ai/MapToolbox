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
    [RequireComponent(typeof(LineRenderer))]
    class BrokenLineRenderer<T> : SiblingParent where T : LineSegment<T>
    {
        LineRenderer lineRenderer;
        public LineRenderer LineRenderer
        {
            get
            {
                if (lineRenderer == null)
                {
                    lineRenderer = GetComponent<LineRenderer>();
                }
                return lineRenderer;
            }
        }
        T[] children;
        public T[] Children
        {
            get
            {
                if (children == null || children.Length == 0)
                {
                    children = GetComponentsInChildren<T>();
                }
                return children;
            }
        }
        public virtual Vector3 LocalFrom
        {
            set => Children.First().LocalFrom = value;
            get => Children.First().LocalFrom;
        }

        public virtual Vector3 LocalTo
        {
            set => Children.Last().LocalTo = value;
            get => Children.Last().LocalTo;
        }

        public virtual Vector3 From
        {
            set => LocalFrom = transform.InverseTransformPoint(value);
            get => transform.TransformPoint(LocalFrom);
        }

        public virtual Vector3 To
        {
            set => LocalTo = transform.InverseTransformPoint(value);
            get => transform.TransformPoint(LocalTo);
        }

        public virtual void UpdateRenderer()
        {
            children = GetComponentsInChildren<T>();
            if (children.Length > 0)
            {
                if (LineRenderer.loop)
                {
                    LineRenderer.positionCount = children.Length;
                    LineRenderer.SetPositions(children.Select(_ => _.LocalFrom).ToArray());
                }
                else
                {
                    LineRenderer.positionCount = children.Length + 1;
                    LineRenderer.SetPositions(children.Select(_ => _.LocalFrom).ToArray());
                    LineRenderer.SetPosition(children.Length, children.Last().LocalTo);
                }
            }
            else
            {
                LineRenderer.positionCount = 0;
            }
        }

        internal void Reverse()
        {
            foreach (var item in Children)
            {
                var localTo = item.localTo;
                item.localTo = item.localFrom;
                item.localFrom = localTo;
                item.transform.SetAsFirstSibling();
            }
            UpdateRenderer();
        }
    }
}