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
    sealed class ADASGoLane : LineSegment, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public ADASMapLane Lane { get; set; }
        public CollectionADASGoLane CollectionLane { get; set; }
        [ReadOnly] public float dist;
        [ReadOnly] public float dir;
        public float apara;
        [ReadOnly] public float r;
        public float slop;
        public float cant;
        public float lw;
        public float rw;
        public ADASMapLane.Jct jct;
        public List<ADASGoLane> bLane;
        public List<ADASGoLane> fLane;
        public int crossID;
        public float span;
        public int lCnt;
        public int lno;
        public ADASMapLane.Type laneType;
        public int limitVel;
        public int refVel;
        public int roadSecID;
        public ADASMapLane.ChgFG laneChgFG;
        public void Csv2Go()
        {
            CollectionLane?.Add(Lane.ID, this);
            name = Lane.ID.ToString();
            transform.position = Lane.BNode.Point.Position;
            to = Lane.FNode.Point.Position;
            dist = Lane.DtLane.Dist;
            dir = Lane.DtLane.Dir;
            apara = Lane.DtLane.Apara;
            r = Lane.DtLane.R;
            slop = Lane.DtLane.Slope;
            cant = Lane.DtLane.Cant;
            lw = Lane.DtLane.LW;
            rw = Lane.DtLane.RW;
            jct = Lane.JCT;
            crossID = Lane.ClossID;
            span = Lane.Span;
            lCnt = Lane.LCnt;
            lno = Lane.Lno;
            laneType = Lane.LaneType;
            limitVel = Lane.LimitVel;
            refVel = Lane.RefVel;
            roadSecID = Lane.RoadSecID;
            laneChgFG = Lane.LaneChgFG;
        }
        public void Go2Csv()
        {
            UpdateRef();
            var point = new ADASMapPoint { Position = transform.position };
            Lane = new ADASMapLane
            {
                DtLane = new ADASMapDtLane
                {
                    Point = point
                },
                BNode = new ADASMapNode
                {
                    Point = point
                }
            };
        }
        public void UpdateRef()
        {
            InnerLastRef();
            InnerNextRef();
            OutterLastRef();
            OutterNextRef();
        }
        private void InnerLastRef()
        {
            if (Last)
            {
                if (bLane == null || bLane.Count == 0)
                {
                    bLane = new List<ADASGoLane> { Last as ADASGoLane };
                }
                else if (bLane[0] != Last)
                {
                    bLane[0] = Last as ADASGoLane;
                }
            }
        }
        private void InnerNextRef()
        {
            if (Next)
            {
                if (fLane == null || fLane.Count == 0)
                {
                    fLane = new List<ADASGoLane> { Next as ADASGoLane };
                }
                else if (fLane[0] != Next)
                {
                    fLane[0] = Next as ADASGoLane;
                }
            }
        }
        private void OutterLastRef()
        {
            if (Last == null && Lane != null)
            {
                bLane = new List<ADASGoLane>();
                if (Lane.BLane != null)
                {
                    bLane.Add(CollectionLane[Lane.BLane.ID]);
                }
                if (Lane.BLane2 != null)
                {
                    bLane.Add(CollectionLane[Lane.BLane2.ID]);
                }
                if (Lane.BLane3 != null)
                {
                    bLane.Add(CollectionLane[Lane.BLane3.ID]);
                }
                if (Lane.BLane4 != null)
                {
                    bLane.Add(CollectionLane[Lane.BLane4.ID]);
                }
            }
        }
        private void OutterNextRef()
        {
            if (Next == null && Lane != null)
            {
                fLane = new List<ADASGoLane>();
                if (Lane.FLane != null)
                {
                    fLane.Add(CollectionLane[Lane.FLane.ID]);
                }
                if (Lane.FLane2 != null)
                {
                    fLane.Add(CollectionLane[Lane.FLane2.ID]);
                }
                if (Lane.FLane3 != null)
                {
                    fLane.Add(CollectionLane[Lane.FLane3.ID]);
                }
                if (Lane.FLane4 != null)
                {
                    fLane.Add(CollectionLane[Lane.FLane4.ID]);
                }
            }
        }
    }
}