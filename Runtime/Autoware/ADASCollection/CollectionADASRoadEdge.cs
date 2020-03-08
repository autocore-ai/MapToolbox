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
    class CollectionADASRoadEdge : CollectionADASMapGo<ADASGoRoadEdge>
    {
        public override void Csv2Go()
        {
            foreach (var item in ADASMapRoadEdge.List.GroupBy(_ => _.Line.FirstLine))
            {
                var slices = new GameObject().AddComponent<ADASGoSlicesRoadEdge>();
                slices.transform.SetParent(transform);
                slices.RoadEdges = item;
            }
            foreach (var item in GetComponentsInChildren<ADASGoSlicesRoadEdge>())
            {
                item.UpdateRenderer();
            }
        }
        public override void Go2Csv()
        {
            foreach (var item in GetComponentsInChildren<ADASGoSlicesRoadEdge>())
            {
                item.BuildData();
            }
        }
        public ADASGoRoadEdge AddRoadEdge(Vector3 position)
        {
            position.y = 0;
            var slices = new GameObject(typeof(ADASGoSlicesRoadEdge).Name);
            slices.transform.SetParent(transform);
            slices.AddComponent<ADASGoSlicesRoadEdge>().SetupRenderer();
            var go = new GameObject(typeof(ADASGoRoadEdge).Name);
            go.transform.SetParent(slices.transform);
            go.transform.position = position;
            var roadEdge = go.AddComponent<ADASGoRoadEdge>();
            roadEdge.To = position;
            return roadEdge;
        }
    }
}