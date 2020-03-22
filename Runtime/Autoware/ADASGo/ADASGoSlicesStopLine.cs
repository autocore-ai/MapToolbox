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


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoSlicesStopLine : ADASGoSlicesLine
    {
        public CollectionADASSignal CollectionSignal { get; set; }
        public CollectionADASRoadSign CollectionRoadSign { get; set; }
        public CollectionADASLane CollectionLane { get; set; }
        public IEnumerable<ADASMapStopLine> VectorMapStopLines 
        {
            set
            {
                SetupRenderer();
                Lines = value.Select(_ => _.Line);
                name = $"{value.First().ID}-{value.Last().ID}";
                foreach (var item in value)
                {
                    var go = new GameObject(typeof(ADASGoStopLine).Name);
                    go.transform.SetParent(transform);
                    var stopline = go.AddComponent<ADASGoStopLine>();
                    stopline.CollectionSignal = CollectionSignal;
                    stopline.CollectionRoadSign = CollectionRoadSign;
                    stopline.CollectionLane = CollectionLane;
                    stopline.StopLine = item;
                }
            }
        }

        public override void BuildData()
        {
            foreach (var item in GetComponentsInChildren<ADASGoStopLine>())
            {
                item.UpdateRef();
                item.BuildData();
            }
        }

        public void SetupRenderer()
        {
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
            LineRenderer.startColor = LineRenderer.endColor = Color.red;
            LineRenderer.useWorldSpace = false;
#if UNITY_EDITOR
            LineRenderer.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
#endif
        }
    }
}