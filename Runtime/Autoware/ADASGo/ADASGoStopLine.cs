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
    class ADASGoStopLine : ADASGoLine
    {
        public ADASGoSignal signal;
        public ADASGoRoadSign roadSign;
        public ADASGoLane linkLane;
        public CollectionSignal CollectionSignal { get; set; }
        public CollectionRoadSign CollectionRoadSign { get; set; }
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
        ADASMapStopLine stopline;
        public static bool ShowLinkedSignal { get; set; }
        public ADASMapStopLine StopLine
        {
            set
            {
                stopline = value;
                if (stopline != null)
                {
                    Line = stopline.Line;
                    name = stopline.ID.ToString();
                    if (CollectionSignal != null && value.Signal != null)
                    {
                        signal = CollectionSignal[value.Signal.ID];
                    }
                    if (CollectionRoadSign != null && value.RoadSign != null)
                    {
                        roadSign = CollectionRoadSign[value.RoadSign.ID];
                    }
                    if (CollectionLane != null && value.LinkLane != null)
                    {
                        linkLane = CollectionLane[value.LinkLane.ID];
                    }
                }
            }
            get
            {
                if (stopline == null)
                {
                    stopline = new ADASMapStopLine
                    {
                        Line = Line
                    };
                    if (signal)
                    {
                        stopline.Signal = signal.Signal;
                    }
                    if (roadSign)
                    {
                        stopline.RoadSign = roadSign.RoadSign;
                    }
                    if (linkLane)
                    {
                        stopline.LinkLane = linkLane.lane;
                    }
                }
                return stopline;
            }
        }
        internal override void BuildData()
        {
            Line = null;
            StopLine = null;
            stopline = StopLine;
            if (linkLane == null)
            {
                linkLane = CollectionLane.First().Value;
                float distance = float.MaxValue;
                foreach (var item in CollectionLane)
                {
                    var dis = Vector3.Distance(item.Value.transform.position, transform.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        linkLane = item.Value;
                    }
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (ShowLinkedSignal && signal)
            {
                Gizmos.color = signal.Color;
                Gizmos.DrawLine(transform.position, signal.transform.position);
            }
        }
    }
}