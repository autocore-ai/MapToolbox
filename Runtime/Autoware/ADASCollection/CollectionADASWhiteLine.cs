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
    class CollectionADASWhiteLine : CollectionADASMapGo<ADASGoWhiteLine>
    {
        public override void Csv2Go()
        {
            foreach (var item in ADASMapWhiteLine.List.GroupBy(_ => _.Line.FirstLine))
            {
                var slices = new GameObject().AddComponent<ADASGoSlicesWhiteLine>();
                slices.transform.SetParent(transform);
                slices.WhiteLines = item;
            }
            foreach (var item in GetComponentsInChildren<ADASGoSlicesWhiteLine>())
            {
                item.UpdateRenderer();
            }
        }
        public override void Go2Csv()
        {
            foreach (var item in GetComponentsInChildren<ADASGoSlicesWhiteLine>())
            {
                item.BuildData();
            }
        }
        public ADASGoWhiteLine AddWhiteLine(Vector3 position)
        {
            position.y = 0;
            var slices = new GameObject(typeof(ADASGoSlicesWhiteLine).Name);
            slices.transform.SetParent(transform);
            slices.AddComponent<ADASGoSlicesWhiteLine>().SetupRenderer();
            var go = new GameObject(typeof(ADASGoWhiteLine).Name);
            go.transform.SetParent(slices.transform);
            go.transform.position = position;
            var whiteLine = go.AddComponent<ADASGoWhiteLine>();
            whiteLine.To = position;
            return whiteLine;
        }
    }
}