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
    class CollectionLane : Collection<ADASGoLane>
    {
        public void Csv2Go()
        {
            foreach (var item in ADASMapLane.List.GroupBy(_ => _.FirstLane))
            {
                var slices = new GameObject().AddComponent<ADASGoSlicesLane>();
                slices.transform.SetParent(transform);
                slices.CollectionLane = this;
                slices.Lanes = item;
            }
            foreach (var item in GetComponentsInChildren<ADASGoSlicesLane>())
            {
                item.SetRef();
                item.UpdateRenderer();
            }
        }
        internal void FindRef()
        {
            foreach (var item in GetComponentsInChildren<ADASGoSlicesLane>())
            {
                item.From = item.From;
                item.To = item.To;
                item.OnEnableEditor();
            }
        }
        public void Go2Csv()
        {
            int id = 1;
            Dic = GetComponentsInChildren<ADASGoLane>().ToDictionary(_ => id++);
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
            position.y = Utils.GetHeight(position);
            var slices = new GameObject(typeof(ADASGoSlicesLane).Name);
            slices.transform.SetParent(transform);
            slices.AddComponent<ADASGoSlicesLane>().SetupRenderer();
            var go = new GameObject(typeof(ADASGoLane).Name);
            go.transform.SetParent(slices.transform);
            var lane = go.AddComponent<ADASGoLane>();
            lane.LocalFrom = position;
            lane.LocalTo = position;
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = slices;
#endif
            return lane;
        }
    }
}