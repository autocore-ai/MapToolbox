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
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    sealed class ADASGoLane : LineSegment<ADASGoLane>, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public CollectionADASLane CollectionLane { get; set; }
        [HideInInspector] public ADASMapLane.Jct jct;
        [HideInInspector] public List<ADASGoLane> bLane;
        [HideInInspector] public List<ADASGoLane> fLane;
        public int lCnt;
        public int lno;
        public ADASMapLane.Type laneType;
        public int limitVel;
        public int refVel;
        ADASMapLane data;
        public static float tempDist;
        public ADASMapLane Lane
        {
            set
            {
                data = value;
                CollectionLane?.Add(data.ID, this);
                name = data.ID.ToString();
                from = data.BNode.Point.Position;
                to = data.FNode.Point.Position;
                jct = data.JCT;
                lCnt = data.LCnt;
                lno = data.Lno;
                laneType = data.LaneType;
                limitVel = data.LimitVel;
                refVel = data.RefVel;
                transform.position = (from + to) / 2;
            }
            get
            {
                if (data == null)
                {
                    var point = new ADASMapPoint { Position = from };
                    data = new ADASMapLane
                    {
                        DTLane = new ADASMapDTLane
                        {
                            Point = point,
                            Dist = tempDist
                        },
                        BNode = new ADASMapNode
                        {
                            Point = point
                        },
                        LCnt = lCnt,
                        Lno = lno,
                        LaneType = laneType,
                        LimitVel = limitVel,
                        RefVel = refVel
                    };
                    tempDist += (to - from).magnitude;
                    if (last != null)
                    {
                        last.GetComponent<ADASGoLane>().data.FNode = data.BNode;
                    }
                    if (next == null)
                    {
                        data.FNode = new ADASMapNode
                        {
                            Point = new ADASMapPoint { Position = to }
                        };
                    }
                }
                return data;
            }
        }

        internal void BuildData()
        {
            data = null;
            data = Lane;
        }
        internal void BuildDataRef()
        {
            if (bLane != null)
            {
                if (bLane.Count > 0 && bLane[0] != null)
                {
                    Lane.BLane = bLane[0].Lane;
                }
                if (bLane.Count > 1 && bLane[1] != null)
                {
                    Lane.BLane2 = bLane[1].Lane;
                }
                if (bLane.Count > 2 && bLane[2] != null)
                {
                    Lane.BLane3 = bLane[2].Lane;
                }
                if (bLane.Count > 3 && bLane[3] != null)
                {
                    Lane.BLane4 = bLane[3].Lane;
                }
            }
            if (fLane != null)
            {
                if (fLane.Count > 0 && fLane[0] != null)
                {
                    Lane.FLane = fLane[0].Lane;
                }
                if (fLane.Count > 1 && fLane[1] != null)
                {
                    Lane.FLane2 = fLane[1].Lane;
                }
                if (fLane.Count > 2 && fLane[2] != null)
                {
                    Lane.FLane3 = fLane[2].Lane;
                }
                if (fLane.Count > 3 && fLane[3] != null)
                {
                    Lane.FLane4 = fLane[3].Lane;
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
            if (last == null && data != null)
            {
                bLane = new List<ADASGoLane>();
                if (data.BLane != null)
                {
                    bLane.Add(CollectionLane[data.BLane.ID]);
                }
                if (data.BLane2 != null)
                {
                    bLane.Add(CollectionLane[data.BLane2.ID]);
                }
                if (data.BLane3 != null)
                {
                    bLane.Add(CollectionLane[data.BLane3.ID]);
                }
                if (data.BLane4 != null)
                {
                    bLane.Add(CollectionLane[data.BLane4.ID]);
                }
            }
        }
        private void OutterNextRef()
        {
            if (next == null && data != null)
            {
                fLane = new List<ADASGoLane>();
                if (data.FLane != null)
                {
                    fLane.Add(CollectionLane[data.FLane.ID]);
                }
                if (data.FLane2 != null)
                {
                    fLane.Add(CollectionLane[data.FLane2.ID]);
                }
                if (data.FLane3 != null)
                {
                    fLane.Add(CollectionLane[data.FLane3.ID]);
                }
                if (data.FLane4 != null)
                {
                    fLane.Add(CollectionLane[data.FLane4.ID]);
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