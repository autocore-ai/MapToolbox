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


using System.Linq;
using UnityEngine;

namespace Packages.MapToolbox
{
    [RequireComponent(typeof(AddOrRemovable))]
    public class PedestrianMarking : WayTypeBase<PedestrianMarking>, IAddOrRemoveTarget
    {
        protected override void Start()
        {
            base.Start();
            LineRenderer.startWidth = LineRenderer.startWidth = 0.2f;
            LineRenderer.startColor = LineRenderer.endColor = Color.cyan;
        }
        public void OnAdd()
        {
            RemoveLoopNodesRef();
            AddNextPoint(GetClickedPoint());
            UseLoopNodesRef();
            UpdateRenderer();
        }
        public void OnRemove()
        {
            RemoveLoopNodesRef();
            RemoveLastNode();
            UseLoopNodesRef();
            UpdateRenderer();
        }
        public void MouseEnterInspector() { }
        private void RemoveLoopNodesRef()
        {
            if (Way.Nodes.Count > 2 && Way.Nodes.First().Equals(Way.Nodes.Last()))
            {
                Way.Nodes.RemoveLast();
            }
        }
        private void UseLoopNodesRef()
        {
            if (Way.Nodes.Count > 2 && !Way.Nodes.First().Equals(Way.Nodes.Last()))
            {
                Way.Nodes.Add(Way.Nodes.First());
            }
        }
    }
}