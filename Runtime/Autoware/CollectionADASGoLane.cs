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
    class CollectionADASGoLane : CollectionADASMapGo<ADASGoLane> 
    {
        public override bool ReadCsv(string path)
        {
            ADASMapPoint.ReadCsv(path);
            ADASMapNode.ReadCsv(path);
            ADASMapDtLane.ReadCsv(path);
            ADASMapLane.ReadCsv(path);
            foreach (var group in ADASMapLane.List.GroupBy(_ => _.StartLane))
            {
                foreach (var item in group.GroupBy(_ => _.FirstLane))
                {
                    var slicesLane = new GameObject().AddComponent<ADASGoSlicesLane>();
                    slicesLane.transform.SetParent(transform);
                    slicesLane.Lanes = item;
                    slicesLane.CollectionLane = this;
                    slicesLane.Csv2Go();
                }
            }
            return true;
        }
        public override bool WriteCsv(string path)
        {
            ADASMapPoint.PreWrite(path);
            ADASMapNode.PreWrite(path);
            ADASMapDtLane.PreWrite(path);
            ADASMapLane.PreWrite(path);
            foreach (var item in GetComponentsInChildren<ADASGoSlicesLane>())
            {
                item.Go2Csv();
            }
            ADASMapPoint.WriteCsv(path);
            ADASMapNode.WriteCsv(path);
            ADASMapDtLane.WriteCsv(path);
            ADASMapLane.WriteCsv(path);
            return true;
        }
        public ADASGoLane AddLane(Vector3 position)
        {
            var slane = new GameObject(typeof(ADASGoSlicesLane).Name);
            slane.transform.SetParent(transform);
            slane.AddComponent<ADASGoSlicesLane>().SetupRenderer();
            var lane = new GameObject(typeof(ADASGoLane).Name);
            lane.transform.SetParent(slane.transform);
            lane.transform.position = position;
            var goLane = lane.AddComponent<ADASGoLane>();
            goLane.To = position;
            goLane.UpdateLineRendererPosition();
            return goLane;
        }
    }
}