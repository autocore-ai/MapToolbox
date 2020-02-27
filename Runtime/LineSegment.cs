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


using UnityEngine;

namespace AutoCore.MapToolbox
{
    [ExecuteInEditMode]
    class LineSegment : SiblingIndex<LineSegment>
    {
        BrokenLineRenderer brokenLineRenderer;
        public BrokenLineRenderer BrokenLineRenderer
        {
            get
            {
                if (brokenLineRenderer == null)
                {
                    brokenLineRenderer = GetComponentInParent<BrokenLineRenderer>();
                }
                return brokenLineRenderer;
            }
        }
        public LineRenderer LineRenderer => BrokenLineRenderer.LineRenderer;
        public override bool EnableMove => false;
        [ReadOnly] [SerializeField] protected Vector3 to;
        public Vector3 From
        {
            get => transform.position;
            set
            {
                transform.position = value;
                if (Last)
                {
                    Last.to = value;
                }
            }
        }
        public Vector3 To
        {
            get => to;
            set
            {
                to = value;
                if (Next)
                {
                    Next.From = value;
                }
            }
        }
        public void UpdateLineRendererPosition()
        {
            if (this.AllParentActived())
            {
                if (LineRenderer.useWorldSpace)
                {
                    LineRenderer.SetPosition(Index, From);
                    LineRenderer.SetPosition(Index + 1, To);
                }
                else
                {
                    LineRenderer.SetPosition(Index, transform.parent.InverseTransformPoint(From));
                    LineRenderer.SetPosition(Index + 1, transform.parent.InverseTransformPoint(To));
                }
            }
        }
        protected override void Awake()
        {
            base.Awake();
            BrokenLineRenderer?.Refresh();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            BrokenLineRenderer.Refresh();
        }
        public override void UpdateIndex()
        {
            base.UpdateIndex();
            BrokenLineRenderer.Refresh();
        }
        public override LineSegment AddBefore(LineSegment target)
        {
            if (Last == null)
            {
                target.transform.position = 2 * From - to;
            }
            else
            {
                target.transform.position = (Last.From + Last.to) / 2;
                Last.to = target.transform.position;
            }
            target.to = transform.position;
            base.AddBefore(target);
            BrokenLineRenderer.Refresh();
            return target;
        }
        public override LineSegment AddAfter(LineSegment target)
        {
            if (Next == null)
            {
                target.to = 2 * to - transform.position;
            }
            else
            {
                target.to = (Next.From + Next.to) / 2;
                Next.transform.position = target.to;
            }
            target.transform.position = To;
            base.AddAfter(target);
            BrokenLineRenderer.Refresh();
            return target;
        }
    }
}