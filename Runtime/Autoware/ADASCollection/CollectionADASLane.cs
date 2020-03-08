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

namespace AutoCore.MapToolbox.Autoware
{
    class CollectionADASLane : CollectionADASMapGo<ADASGoLane>
    {
        public override void Csv2Go()
        {
            foreach (var group in ADASMapLane.List.GroupBy(_ => _.StartLane))
            {
                foreach (var item in group.GroupBy(_ => _.FirstLane))
                {
                    var slices = new GameObject().AddComponent<ADASGoSlicesLane>();
                    slices.transform.SetParent(transform);
                    slices.CollectionLane = this;
                    slices.Lanes = item;
                }
            }
            foreach (var item in GetComponentsInChildren<ADASGoSlicesLane>())
            {
                item.UpdateRef();
                item.UpdateRenderer();
            }
        }
        public override void Go2Csv()
        {
            foreach (var item in GetComponentsInChildren<ADASGoSlicesLane>())
            {
                item.BuildData();
            }
            foreach (var item in GetComponentsInChildren<ADASGoLane>())
            {
                item.BuildDataRef();
            }
        }
        public ADASGoLane AddLane(Vector3 position)
        {
            position.y = 0;
            var slices = new GameObject(typeof(ADASGoSlicesLane).Name);
            slices.transform.SetParent(transform);
            slices.AddComponent<ADASGoSlicesLane>().SetupRenderer();
            var go = new GameObject(typeof(ADASGoLane).Name);
            go.transform.SetParent(slices.transform);
            var lane = go.AddComponent<ADASGoLane>();
            lane.From = position;
            lane.To = position;
            return lane;
        }
    }
}