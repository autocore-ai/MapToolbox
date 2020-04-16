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
    class CollectionSignal : CollectionADASMapGo<ADASGoSignal>
    {
        public override void Csv2Go()
        {
            foreach (var item in ADASMapSignal.List)
            {
                var signal = new GameObject().AddComponent<ADASGoSignal>();
                signal.transform.SetParent(transform);
                signal.CollectionPole = AutowareADASMap.CollectionPole;
                signal.CollectionLane = AutowareADASMap.CollectionLane;
                signal.CollectionSignal = this;
                signal.Signal = item;
            }
        }
        public override void Go2Csv()
        {
            int id = 1;
            Dic = GetComponentsInChildren<ADASGoSignal>().ToDictionary(_ => id++);
            foreach (var item in GetComponentsInChildren<ADASGoSignal>())
            {
                item.BuildData();
            }
        }
        public ADASGoSignal AddSignal(Vector3 position)
        {
            position.y = 6;
            var signal = new GameObject().AddComponent<ADASGoSignal>();
            signal.transform.SetParent(transform);
            signal.CollectionPole = AutowareADASMap.CollectionPole;
            signal.CollectionLane = AutowareADASMap.CollectionLane;
            signal.CollectionSignal = this;
            signal.transform.position = position;
            return signal;
        }
    }
}