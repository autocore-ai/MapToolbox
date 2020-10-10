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
    sealed class ADASGoSlicesLane : BrokenLineRenderer<ADASGoLane>, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        [Range(0, 3)]
        public float lw = 0;
        [Range(0, 3)]
        public float rw = 0;
        public float LW
        {
            set => lw = value;
            get => lw;
        }
        public float RW
        {
            set => rw = value;
            get => rw;
        }
        [HideInInspector] public List<ADASGoSlicesLane> bLane = new List<ADASGoSlicesLane>();
        [HideInInspector] public List<ADASGoSlicesLane> fLane = new List<ADASGoSlicesLane>();
        public CollectionLane CollectionLane { get; set; }
        static HashSet<ADASGoSlicesLane> HashSet { get; set; } = new HashSet<ADASGoSlicesLane>();
        private void OnEnable() => HashSet.Add(this);
        private void OnDisable() => HashSet.Remove(this);
        public override Vector3 From
        {
            get => base.From;
            set
            {
                if (HashSet == null)
                {
                    HashSet = new HashSet<ADASGoSlicesLane>();
                }
                foreach (var item in HashSet)
                {
                    if (Vector3.Distance(item.To, value) < ADASGoLane.minDistance && item != this)
                    {
                        value = item.To;
                        if (bLane.Count > 0)
                        {
                            bLane.RemoveAll(t => t == null);
                            bLane.RemoveAll(t => t.To != value);
                        }
                        if (!bLane.Contains(item))
                        {
                            bLane.Add(item);
                        }
                        if (!item.fLane.Contains(this))
                        {
                            item.fLane.Add(this);
                        }
                        base.From = value;
                        return;
                    }
                }
                if (bLane.Count > 0)
                {
                    bLane.Clear();
                }
                base.From = value;
            }
        }
        public override Vector3 To
        {
            get => base.To;
            set
            {
                if (HashSet == null)
                {
                    HashSet = new HashSet<ADASGoSlicesLane>();
                }
                foreach (var item in HashSet)
                {
                    if (Vector3.Distance(item.From, value) < ADASGoLane.minDistance && item != this)
                    {
                        value = item.From;
                        if (fLane.Count > 0)
                        {
                            fLane.RemoveAll(t => t == null);
                            fLane.RemoveAll(t => t.From != value);
                        }
                        if (!fLane.Contains(item))
                        {
                            fLane.Add(item);
                        }
                        if (!item.bLane.Contains(this))
                        {
                            item.bLane.Add(this);
                        }
                        base.To = value;
                        return;
                    }
                }
                if (fLane.Count > 0)
                {
                    fLane.Clear();
                }
                base.To = value;
            }
        }
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
            }
        }
        internal void OnEnableEditor()
        {
            if (Children != null && Children.Length > 0)
            {
                Children.First().CheckBLanes(bLane.Select(_ => _.Children.LastOrDefault()));
                Children.Last().CheckFLanes(fLane.Select(_ => _.Children.FirstOrDefault()));
            }
        }
        internal void BuildData()
        {
            ADASGoLane.tempDist = 0;
            foreach (var item in Children)
            {
                item.UpdateRef();
                item.BuildData();
            }
        }

        public void SetupRenderer()
        {
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
            LineRenderer.useWorldSpace = false;
            LineRenderer.textureMode = LineTextureMode.Tile;
            LineRenderer.sharedMaterial = Resources.Load<Material>("MapToolbox/Lane");
        }
        public void SetRef()
        {
            bLane.Clear();
            fLane.Clear();
            if (Children.Length > 0)
            {
                var target = Children.First();
                target.UpdateRef();
                target.UpdateOutterRef();
                bLane = target.bLane.Select(_ => _.Slices).ToList();
                target = Children.Last();
                target.UpdateRef();
                target.UpdateOutterRef();
                fLane = target.fLane.Select(_ => _.Slices).ToList();
            }
        }
    }
}