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


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    sealed class ADASGoSlicesLane : BrokenLineRenderer<ADASGoLane>, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        [HideInInspector] public List<ADASGoSlicesLane> bLane;
        [HideInInspector] public List<ADASGoSlicesLane> fLane;
        public CollectionADASLane CollectionLane { get; set; }
        public IEnumerable<ADASMapLane> Lanes
        {
            set
            {
                SetupRenderer();
                name = $"{value.First().ID}-{value.Last().ID}";
                foreach (var item in value)
                {
                    var go = new GameObject(typeof(ADASGoLane).Name);
                    go.transform.SetParent(transform);
                    var lane = go.AddComponent<ADASGoLane>();
                    lane.CollectionLane = CollectionLane;
                    lane.Lane = item;
                }
                foreach (var item in GetComponentsInChildren<ADASGoLane>())
                {
                    item.UpdateOutterRef();
                }
            }
        }

        internal void BuildData()
        {
            ADASGoLane.tempDist = 0;
            foreach (var item in GetComponentsInChildren<ADASGoLane>())
            {
                item.UpdateRef();
                item.BuildData();
            }
        }

        public void SetupRenderer()
        {
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
            LineRenderer.textureMode = LineTextureMode.Tile;
            LineRenderer.sharedMaterial = Resources.Load<Material>("MapToolbox/Lane");
        }
        public void UpdateRef()
        {
            var child = GetComponentsInChildren<ADASGoLane>();
            if (child.Length > 0)
            {
                child.First().UpdateRef();
                child.Last().UpdateRef();
                var target = child[0];
                if (target.bLane != null && target.bLane.Count > 0)
                {
                    foreach (var item in target.bLane)
                    {
                        if (item?.Slices)
                        {
                            bLane.Add(item.Slices);
                        }
                    }
                }
                target = child[child.Length - 1];
                if (target.fLane != null && target.fLane.Count > 0)
                {
                    foreach (var item in target.fLane)
                    {
                        if (item?.Slices)
                        {
                            fLane.Add(item.Slices);
                        }
                    }
                }
            }
            else
            {
                bLane?.Clear();
                fLane?.Clear();
            }
        }
    }
}