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

namespace AutoCore.MapToolbox.Autoware
{
    class CollectionADASStopLine : CollectionADASMapGo<ADASGoStopLine>
    {
        public override void Csv2Go()
        {
            foreach (var item in ADASMapStopLine.List.GroupBy(_ => _.Line.FirstLine))
            {
                var slices = new GameObject().AddComponent<ADASGoSlicesStopLine>();
                slices.transform.SetParent(transform);
                slices.CollectionSignal = AutowareADASMap.CollectionSignal;
                slices.CollectionRoadSign = AutowareADASMap.CollectionRoadSign;
                slices.CollectionLane = AutowareADASMap.CollectionLane;
                slices.VectorMapStopLines = item;
            }
            foreach (var item in GetComponentsInChildren<ADASGoSlicesStopLine>())
            {
                item.UpdateRenderer();
            }
        }
        public override void Go2Csv()
        {
            foreach (var item in GetComponentsInChildren<ADASGoSlicesStopLine>())
            {
                item.BuildData();
            }
        }
        public ADASGoStopLine AddStopLine(Vector3 position)
        {
            position.y = 0;
            var slices = new GameObject(typeof(ADASGoSlicesStopLine).Name);
            slices.transform.SetParent(transform);
            slices.AddComponent<ADASGoSlicesStopLine>().SetupRenderer();
            var go = new GameObject(typeof(ADASGoStopLine).Name);
            go.transform.SetParent(slices.transform);
            go.transform.position = position;
            var stopLine = go.AddComponent<ADASGoStopLine>();
            stopLine.LocalFrom = position;
            stopLine.LocalTo = position;
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = slices;
#endif
            return stopLine;
        }
    }
}