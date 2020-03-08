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


using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class CollectionADASPole : CollectionADASMapGo<ADASGoPole>
    {
        public override void Csv2Go()
        {
            foreach (var item in ADASMapPole.List)
            {
                var pole = new GameObject().AddComponent<ADASGoPole>();
                pole.transform.SetParent(transform);
                pole.Pole = item;
                pole.CollectionPole = this;
                pole.Csv2Go();
            }
        }
    }
}