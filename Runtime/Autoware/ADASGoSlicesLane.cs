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
    sealed class ADASGoSlicesLane : BrokenLineRenderer, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public IEnumerable<ADASMapLane> Lanes { get; set; }
        public CollectionADASGoLane CollectionLane { get; set; }
        [ReadOnly] [SerializeField] List<ADASGoSlicesLane> bLane;
        [ReadOnly] [SerializeField] List<ADASGoSlicesLane> fLane;
        public void Csv2Go()
        {
            foreach (var item in Lanes)
            {
                var go = new GameObject(typeof(ADASGoLane).Name);
                go.transform.SetParent(transform);
                go.transform.position = item.FNode.Point.Position;
                var lane = go.AddComponent<ADASGoLane>();
                lane.Lane = item;
                lane.CollectionLane = CollectionLane;
                lane.Csv2Go();
            }
            Refresh();
            name = $"{Lanes.First().ID}-{Lanes.Last().ID}";
            SetupRenderer();
        }

        public void SetupRenderer()
        {
            LineRenderer.startWidth = LineRenderer.endWidth = 0.4f;
            LineRenderer.textureMode = LineTextureMode.Tile;
            LineRenderer.sharedMaterial = Resources.Load<Material>("MapToolbox/Lane");
        }

        public void Go2Csv()
        {
            var lanes = GetComponentsInChildren<ADASGoLane>();
            foreach (var lane in lanes)
            {
                lane.Go2Csv();
            }
            for (int i = 0; i < lanes.Length - 1; i++)
            {
                var target = lanes[i];
                switch (target.fLane.Count)
                {
                    case 1:
                        target.Lane.FLane = target.fLane[0].Lane;
                        break;
                    case 2:
                        target.Lane.FLane = target.fLane[0].Lane;
                        target.Lane.FLane2 = target.fLane[1].Lane;
                        break;
                    case 3:
                        target.Lane.FLane = target.fLane[0].Lane;
                        target.Lane.FLane2 = target.fLane[1].Lane;
                        target.Lane.FLane3 = target.fLane[2].Lane;
                        break;
                    case 4:
                        target.Lane.FLane = target.fLane[0].Lane;
                        target.Lane.FLane2 = target.fLane[1].Lane;
                        target.Lane.FLane3 = target.fLane[2].Lane;
                        target.Lane.FLane4 = target.fLane[3].Lane;
                        break;
                    default:
                        Debug.LogError($"{target.name} has {target.fLane.Count} fLane, which should be 1 - 4");
                        break;
                }
            }
            for (int i = 1; i < lanes.Length; i++)
            {
                var target = lanes[i];
                switch (target.bLane.Count)
                {
                    case 1:
                        target.Lane.BLane = target.bLane[0].Lane;
                        break;
                    case 2:
                        target.Lane.BLane = target.bLane[0].Lane;
                        target.Lane.BLane2 = target.bLane[1].Lane;
                        break;
                    case 3:
                        target.Lane.BLane = target.bLane[0].Lane;
                        target.Lane.BLane2 = target.bLane[1].Lane;
                        target.Lane.BLane3 = target.bLane[2].Lane;
                        break;
                    case 4:
                        target.Lane.BLane = target.bLane[0].Lane;
                        target.Lane.BLane2 = target.bLane[1].Lane;
                        target.Lane.BLane3 = target.bLane[2].Lane;
                        target.Lane.BLane4 = target.bLane[3].Lane;
                        break;
                    default:
                        Debug.LogError($"{target.name} has {target.bLane.Count} bLane, which should be 1 - 4");
                        break;
                }
            }
            lanes.Last().Lane.FNode = new ADASMapNode { Point = new ADASMapPoint { Position = lanes.Last().To } };
        }
        public void UpdateRef()
        {
            var child = GetComponentsInChildren<ADASGoLane>();
            if (child.Length > 0)
            {
                var target = child[0];
                target.UpdateRef();
                if (target.bLane != null)
                {
                    bLane = target.bLane.Select(_ => _.GetComponentInParent<ADASGoSlicesLane>()).ToList();
                }
                target = child[child.Length - 1];
                target.UpdateRef();
                if (target.fLane != null)
                {
                    fLane = target.fLane.Select(_ => _.GetComponentInParent<ADASGoSlicesLane>()).ToList();
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