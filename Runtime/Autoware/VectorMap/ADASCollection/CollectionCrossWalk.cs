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
    class CollectionCrossWalk : Collection<ADASGoCrossWalk>
    {
        public void Csv2Go()
        {
            foreach (var item in ADASMapCrossWalk.List)
            {
                var slices = new GameObject().AddComponent<ADASGoCrossWalk>();
                slices.transform.SetParent(transform);
                slices.CrossWalk = item;
            }
            foreach (var item in GetComponentsInChildren<ADASGoCrossWalk>())
            {
                item.UpdateBorder();
                item.UpdateRenderer();
            }
        }
        public void Go2Csv()
        {
            int id = 1;
            Dic = GetComponentsInChildren<ADASGoCrossWalk>().ToDictionary(_ => id++);
            foreach (var item in GetComponentsInChildren<ADASGoCrossWalk>())
            {
                item.BuildData();
            }
            foreach (var item in GetComponentsInChildren<ADASGoCrossWalk>())
            {
                item.BuildDataRef();
            }
        }
        public void AddCrossWalk(Vector3 position)
        {
            position.y = 0;
            var crosswalk = new GameObject(typeof(ADASGoCrossWalk).Name).AddComponent<ADASGoCrossWalk>();
            crosswalk.transform.SetParent(transform);
            crosswalk.transform.position = position;
            crosswalk.Init4Lines();
            crosswalk.SetupRenderer();
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = crosswalk.gameObject;
#endif
        }
    }
}