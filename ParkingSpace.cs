#region License
/******************************************************************************
* Copyright 2018-2021 The AutoCore Authors. All Rights Reserved.
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

namespace Packages.MapToolbox
{
    [RequireComponent(typeof(AddOrRemovable))]
    public class ParkingSpace : WayTypeBase<ParkingSpace>, IAddOrRemoveTarget
    {
        public float width = 2.5f;
        public float Width
        {
            set => LineRenderer.startWidth = LineRenderer.endWidth = width = value;
            get => LineRenderer.startWidth;
        }
        protected override void Start()
        {
            base.Start();
            LineRenderer.startColor = LineRenderer.endColor = Color.yellow;
            LineRenderer.startWidth = LineRenderer.endWidth = width;
        }
        public void OnAdd()
        {
            AddNextPoint(GetClickedPoint());
            UpdateRenderer();
        }
        public void OnRemove()
        {
            RemoveLastNode();
            UpdateRenderer();
        }
        internal override void AddNextPoint(Vector3 point)
        {
            if (Way.Nodes.Count < 2)
            {
                base.AddNextPoint(point);
            }
        }
        public void MouseEnterInspector()
        {
            if (Width != width)
            {
                Width = width;
            }
        }
    }
}