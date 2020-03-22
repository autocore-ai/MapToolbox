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
        public CollectionADASSignal CollectionSignal { get; set; }
        public CollectionADASRoadSign CollectionRoadSign { get; set; }
        CollectionADASLane collectionLane;
        public CollectionADASLane CollectionLane
        {
            set => collectionLane = value;
            get
            {
                if (collectionLane == null)
                {
                    collectionLane = GetComponentInParent<AutowareADASMap>().GetComponentInChildren<CollectionADASLane>();
                }
                return collectionLane;
            }
        }
        ADASMapStopLine data;
        public static bool ShowLinkedSignal { get; set; }
        public ADASMapStopLine StopLine
        {
            set
            {
                data = value;
                if (data != null)
                {
                    Line = data.Line;
                    name = data.ID.ToString();
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
                if (data == null)
                {
                    data = new ADASMapStopLine
                    {
                        Line = Line
                    };
                    if (signal)
                    {
                        data.Signal = signal.Signal;
                    }
                    if (roadSign)
                    {
                        data.RoadSign = roadSign.RoadSign;
                    }
                    if (linkLane)
                    {
                        data.LinkLane = linkLane.lane;
                    }
                }
                return data;
            }
        }
        internal override void BuildData()
        {
            Line = null;
            StopLine = null;
            data = StopLine;
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