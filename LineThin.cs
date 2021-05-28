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
    public class LineThin : WayTypeBase<LineThin>
    {
        public enum SubType
        {
            solid,
            dashed
        }
        public SubType subType = SubType.solid;
        protected override void Start()
        {
            base.Start();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
            LineRenderer.startColor = LineRenderer.endColor = Color.white;
        }
        internal void DuplicateNodes(LineThin target1, LineThin target2)
        {
            Way.Nodes.Clear();
            int pointCount = Mathf.Min(target1.Way.Nodes.Count, target2.Way.Nodes.Count);
            for (int i = 0; i < pointCount; i++)
            {
                Vector3 position = 2 * target2.Way.Nodes[i].Position - target1.Way.Nodes[i].Position;
                Way.InsertNode(position);
            }
        }
    }
}