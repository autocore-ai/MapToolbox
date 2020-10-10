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
    sealed class ADASGoLane : LineSegment<ADASGoLane>, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        ADASGoSlicesLane slices;
        public ADASGoSlicesLane Slices
        {
            get
            {
                if (slices == null)
                {
                    slices = GetComponentInParent<ADASGoSlicesLane>();
                }
                return slices;
            }
        }
        CollectionLane collectionLane;
        public CollectionLane CollectionLane
        {
            set => collectionLane = value;
            get
            {
                if (collectionLane == null)
                {
                    collectionLane = GetComponentInParent<AutowareADASMap>().GetComponentInChildren<CollectionLane>();
                }
                return collectionLane;
            }
        }
        public ADASMapLane.Jct jct = ADASMapLane.Jct.NORMAL;
        [HideInInspector] public List<ADASGoLane> bLane = new List<ADASGoLane>();
        [HideInInspector] public List<ADASGoLane> fLane = new List<ADASGoLane>();
        public int lCnt = 1;
        public int lno = 1;
        public ADASMapLane.Type laneType = ADASMapLane.Type.STRAIGHT;
        public int limitVel = 20;
        public int refVel = 20;
        public static float tempDist;
        public const float minDistance = 0.2f;
        [Range(0, 3)]
        public float lw = 0;
        [Range(0, 3)]
        public float rw = 0;
        public float LW
        {
            set => lw = value;
            get
            {
                if (lw == 0)
                {
                    lw = Slices.LW;
                }
                return lw;
            }
        }
        public float RW
        {
            set => rw = value;
            get
            {
                if (rw == 0)
                {
                    rw = Slices.RW;
                }
                return rw;
            }
        }
        public ADASMapLane lane;
        public ADASMapLane Lane
        {
            set
            {
                lane = value;
                if (lane != null)
                {
                    CollectionLane?.Add(lane.ID, this);
                    name = lane.ID.ToString();
                    From = lane.BNode.Point.Position;
                    To = lane.FNode.Point.Position;
                    jct = lane.JCT;
                    lCnt = lane.LCnt;
                    lno = lane.Lno;
                    laneType = lane.LaneType;
                    limitVel = lane.LimitVel;
                    refVel = lane.RefVel;
                    transform.position = (From + To) / 2;
                }
            }
            get
            {
                if (lane == null)
                {
                    var point = new ADASMapPoint { Position = From };
                    lane = new ADASMapLane
                    {
                        DTLane = new ADASMapDTLane
                        {
                            Point = point,
                            Dist = tempDist,
                            LW = LW,
                            RW = RW
                        },
                        BNode = new ADASMapNode
                        {
                            Point = point
                        },
                        JCT = jct,
                        LCnt = lCnt,
                        Lno = lno,
                        LaneType = laneType,
                        LimitVel = limitVel,
                        RefVel = refVel
                    };
                    tempDist += (To - From).magnitude;
                    if (last != null)
                    {
                        last.GetComponent<ADASGoLane>().lane.FNode = lane.BNode;
                    }
                    if (next == null)
                    {
                        lane.FNode = new ADASMapNode
                        {
                            Point = new ADASMapPoint { Position = To }
                        };
                    }
                }
                return lane;
            }
        }
        internal void OnEditorEnable()
        {

        }
        internal override void AutoSubdivision()
        {
            base.AutoSubdivision();
            ADASGoLane refLast = last?.GetComponent<ADASGoLane>();
            if (refLast == null && Slices.bLane.Count > 0)
            {
                var lastSlices = Slices.bLane.First();
                if (lastSlices != null)
                {
                    var lastLanes = lastSlices.GetComponentsInChildren<ADASGoLane>();
                    if (lastLanes.Length > 0)
                    {
                        refLast = lastLanes.Last();
                    }
                }
            }
            ADASGoLane refNext = next?.GetComponent<ADASGoLane>();
            if (refNext == null && Slices.fLane.Count > 0)
            {
                var nextSlices = Slices.fLane.First();
                if (nextSlices != null)
                {
                    var nextLanes = nextSlices.GetComponentsInChildren<ADASGoLane>();
                    if (nextLanes.Length > 0)
                    {
                        refNext = nextLanes.First();
                    }
                }
            }
            if (refLast && refNext)
            {
                Vector3 lastDir = refLast.To - refLast.From;
                Vector3 nextDir = refNext.To - refNext.From;
                if (MapToolbox.Utils.ClosestPointsOnTwoLines(out Vector3 p1, out Vector3 p2,
                    refLast.From, lastDir, refNext.From, nextDir))
                {
                    Subdivision((refLast.To + p1) / 2, (refNext.From + p2) / 2);
                }
                else
                {
                    Subdivision((refLast.To + refNext.From) / 2, (refLast.To + refNext.From) / 2);
                }
            }
        }
        internal void CheckFLanes(IEnumerable<ADASGoLane> flanes)
        {
            UpdateRef();
            UpdateOutterRef();
            fLane = flanes.Where(_ => _ != null && Vector3.Distance(_.From, To) < minDistance).ToList();
            foreach (var item in fLane)
            {
                item.From = To;
            }
        }
        internal void CheckBLanes(IEnumerable<ADASGoLane> blanes)
        {
            UpdateRef();
            UpdateOutterRef();
            bLane = blanes.Where(_ => _ != null && Vector3.Distance(_.To, From) < minDistance).ToList();
            foreach (var item in bLane)
            {
                item.To = From;
            }
        }
        internal void BuildData()
        {
            lane = null;
            lane = Lane;
        }
        internal void BuildDataRef()
        {
            if (bLane != null)
            {
                if (bLane.Count > 0 && bLane[0] != null)
                {
                    lane.BLane = bLane[0].lane;
                }
                if (bLane.Count > 1 && bLane[1] != null)
                {
                    lane.BLane2 = bLane[1].lane;
                }
                if (bLane.Count > 2 && bLane[2] != null)
                {
                    lane.BLane3 = bLane[2].lane;
                }
                if (bLane.Count > 3 && bLane[3] != null)
                {
                    lane.BLane4 = bLane[3].lane;
                }
            }
            if (fLane != null)
            {
                if (fLane.Count > 0 && fLane[0] != null)
                {
                    lane.FLane = fLane[0].lane;
                }
                if (fLane.Count > 1 && fLane[1] != null)
                {
                    lane.FLane2 = fLane[1].lane;
                }
                if (fLane.Count > 2 && fLane[2] != null)
                {
                    lane.FLane3 = fLane[2].lane;
                }
                if (fLane.Count > 3 && fLane[3] != null)
                {
                    lane.FLane4 = fLane[3].lane;
                }
            }
        }
        public override void UpdateRef()
        {
            base.UpdateRef();
            InnerLastRef();
            InnerNextRef();
        }
        private void InnerLastRef()
        {
            if (last)
            {
                if (bLane == null || bLane.Count == 0)
                {
                    bLane = new List<ADASGoLane> { last.GetComponent<ADASGoLane>() };
                }
                else if (bLane[0] != last)
                {
                    bLane[0] = last.GetComponent<ADASGoLane>();
                }
            }
        }
        private void InnerNextRef()
        {
            if (next)
            {
                if (fLane == null || fLane.Count == 0)
                {
                    fLane = new List<ADASGoLane> { next.GetComponent<ADASGoLane>() };
                }
                else if (fLane[0] != next)
                {
                    fLane[0] = next.GetComponent<ADASGoLane>();
                }
            }
        }
        public void UpdateOutterRef()
        {
            OutterLastRef();
            OutterNextRef();
        }
        private void OutterLastRef()
        {
            if (last == null && lane != null)
            {
                bLane = new List<ADASGoLane>();
                if (lane.BLane != null)
                {
                    bLane.Add(CollectionLane[lane.BLane.ID]);
                }
                if (lane.BLane2 != null)
                {
                    bLane.Add(CollectionLane[lane.BLane2.ID]);
                }
                if (lane.BLane3 != null)
                {
                    bLane.Add(CollectionLane[lane.BLane3.ID]);
                }
                if (lane.BLane4 != null)
                {
                    bLane.Add(CollectionLane[lane.BLane4.ID]);
                }
            }
        }
        private void OutterNextRef()
        {
            if (next == null && lane != null)
            {
                fLane = new List<ADASGoLane>();
                if (lane.FLane != null)
                {
                    fLane.Add(CollectionLane[lane.FLane.ID]);
                }
                if (lane.FLane2 != null)
                {
                    fLane.Add(CollectionLane[lane.FLane2.ID]);
                }
                if (lane.FLane3 != null)
                {
                    fLane.Add(CollectionLane[lane.FLane3.ID]);
                }
                if (lane.FLane4 != null)
                {
                    fLane.Add(CollectionLane[lane.FLane4.ID]);
                }
            }
        }
        protected override void DataCopy(LineSegment<ADASGoLane> target)
        {
            base.DataCopy(target);
            var t = target as ADASGoLane;
            lCnt = t.lCnt;
            lno = t.lno;
            laneType = t.laneType;
            limitVel = t.limitVel;
            refVel = t.refVel;
        }
    }
}