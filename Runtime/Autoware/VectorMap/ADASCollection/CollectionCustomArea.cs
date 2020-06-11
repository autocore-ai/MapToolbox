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
    class CollectionCustomArea : Collection<ADASGoCustomArea>
    {
        public void Csv2Go()
        {
            foreach (var item in ADASMapCustomArea.List)
            {
                var slices = new GameObject().AddComponent<ADASGoCustomArea>();
                slices.transform.SetParent(transform);
                slices.CustomArea = item;
            }
            foreach (var item in GetComponentsInChildren<ADASGoCustomArea>())
            {
                item.UpdateRenderer();
            }
        }
        public void Go2Csv()
        {
            int id = 1;
            Dic = GetComponentsInChildren<ADASGoCustomArea>().ToDictionary(_ => id++);
            foreach (var item in GetComponentsInChildren<ADASGoCustomArea>())
            {
                item.BuildData();
            }
        }
        public void AddCustomArea(Vector3 position)
        {
            position.y = 0;
            var customArea = new GameObject(typeof(ADASGoCustomArea).Name).AddComponent<ADASGoCustomArea>();
            customArea.transform.SetParent(transform);
            customArea.transform.position = position;
            customArea.Init4Lines();
            customArea.SetupRenderer();
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = customArea.gameObject;
#endif
        }
    }
}