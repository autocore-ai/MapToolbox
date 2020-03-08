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


using System;
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
        T[] Children
        {
            get
            {
                if (children == null)
                {
                    children = GetComponentsInChildren<T>();
                }
                return children;
            }
        }
        public Vector3 From
        {
            set => Children.First().From = value;
            get => Children.First().From;
        }

        public Vector3 To
        {
            set => Children.Last().To = value;
            get => Children.Last().To;
        }

        public virtual void UpdateRenderer()
        {
            children = GetComponentsInChildren<T>();
            if (children.Length > 0)
            {
                LineRenderer.positionCount = children.Length + 1;
                LineRenderer.SetPositions(children.Select(_ => _.From).ToArray());
                LineRenderer.SetPosition(children.Length, children.Last().To);
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
                var to = item.to;
                item.to = item.from;
                item.from = to;
                item.transform.SetAsFirstSibling();
            }
            UpdateRenderer();
        }
    }
}