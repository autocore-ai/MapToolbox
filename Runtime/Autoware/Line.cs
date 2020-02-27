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
    class Line : LineSegment, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public ADASMapLine VectorMapLine { get; set; }
        public virtual void Init()
        {
            name = VectorMapLine.ID.ToString();
            transform.position = VectorMapLine.BPoint.Position;
            to = VectorMapLine.FPoint.Position;
        }
    }
    class WhiteLine : Line
    {
        public ADASMapWhiteLine VectorMapWhiteLine { get; set; }
        public override void Init()
        {
            VectorMapLine = VectorMapWhiteLine.Line;
            base.Init();
            name = VectorMapWhiteLine.ID.ToString();
        }
    }
    class Curb : Line
    {
        public ADASMapCurb VectorMapCurb { get; set; }
        public override void Init()
        {
            VectorMapLine = VectorMapCurb.Line;
            base.Init();
            name = VectorMapCurb.ID.ToString();
        }
    }
    class StopLine : Line
    {
        public int tlid;
        public int sgid;
        public ADASGoLane linkLane;
        public ADASMapStopLine VectorMapStopLine { get; set; }
        public CollectionADASGoLane CollectionLane { get; set; }
        public override void Init()
        {
            VectorMapLine = VectorMapStopLine.Line;
            base.Init();
            tlid = VectorMapStopLine.TLID;
            sgid = VectorMapStopLine.SignID;
            name = VectorMapStopLine.ID.ToString();
            if (CollectionLane != null && VectorMapStopLine.LinkLane != null)
            {
                linkLane = CollectionLane[VectorMapStopLine.LinkLane.ID];
            }
        }
    }
    class SlicesLine : BrokenLineRenderer, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public IEnumerable<ADASMapLine> VectorMapLines { get; set; }
        public virtual void Init()
        {
            foreach (var item in VectorMapLines)
            {
                var go = new GameObject(typeof(Line).Name);
                go.transform.SetParent(transform);
                go.transform.position = item.FPoint.Position;
                var line = go.AddComponent<Line>();
                line.VectorMapLine = item;
                line.Init();
            }
        }
    }
    class SlicesStopLine : SlicesLine
    {
        public IEnumerable<ADASMapStopLine> VectorMapStopLines { get; set; }
        public CollectionADASGoLane CollectionLane { get; set; }
        public override void Init()
        {
            VectorMapLines = VectorMapStopLines.Select(_ => _.Line);
            foreach (var item in VectorMapStopLines)
            {
                var go = new GameObject(typeof(StopLine).Name);
                go.transform.SetParent(transform);
                go.transform.position = item.Line.FPoint.Position;
                var stopline = go.AddComponent<StopLine>();
                stopline.VectorMapStopLine = item;
                stopline.CollectionLane = CollectionLane;
                stopline.Init();
            }
            base.Refresh();
            name = $"{VectorMapStopLines.First().ID}-{VectorMapStopLines.Last().ID}";
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
            LineRenderer.startColor = LineRenderer.endColor = Color.red;
            LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        private void OnDrawGizmos()
        {
            foreach (var item in VectorMapStopLines)
            {
                var center = (item.Line.BPoint.Position + item.Line.FPoint.Position) / 2;
                if (item.Signal != null)
                {
                    Gizmos.DrawLine(center, item.Signal.Vector.Point.Position);
                }
                if (item.RoadSign != null)
                {
                    Gizmos.DrawLine(center, item.RoadSign.Vector.Point.Position);
                }
            }
        }
    }
    class SlicesWhiteLine : SlicesLine
    {
        public IEnumerable<ADASMapWhiteLine> VectorMapWhiteLines { get; set; }
        public override void Init()
        {
            VectorMapLines = VectorMapWhiteLines.Select(_ => _.Line);
            foreach (var item in VectorMapWhiteLines)
            {
                var go = new GameObject(typeof(WhiteLine).Name);
                go.transform.SetParent(transform);
                go.transform.position = item.Line.FPoint.Position;
                var whiteline = go.AddComponent<WhiteLine>();
                whiteline.VectorMapWhiteLine = item;
                whiteline.Init();
            }
            base.Refresh();
            name = $"{VectorMapWhiteLines.First().ID}-{VectorMapWhiteLines.Last().ID}";
            LineRenderer.startWidth = VectorMapWhiteLines.First().Width;
            LineRenderer.endWidth = VectorMapWhiteLines.Last().Width;
            LineRenderer.startColor = LineRenderer.endColor = VectorMapWhiteLines.First().COLOR == ADASMapWhiteLine.Color.White ? Color.white : Color.yellow;
            LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }
    class SlicesCurb : SlicesLine
    {
        public IEnumerable<ADASMapCurb> VectorMapCurbs { get; set; }
        public override void Init()
        {
            VectorMapLines = VectorMapCurbs.Select(_ => _.Line);
            foreach (var item in VectorMapCurbs)
            {
                var go = new GameObject(typeof(ADASMapCurb).Name);
                go.transform.SetParent(transform);
                go.transform.position = item.Line.FPoint.Position;
                var curb = go.AddComponent<Curb>();
                curb.VectorMapCurb = item;
                curb.Init();
            }
            base.Refresh();
            name = $"{VectorMapCurbs.First().ID}-{VectorMapCurbs.Last().ID}";
            LineRenderer.startWidth = VectorMapCurbs.First().Width;
            LineRenderer.endWidth = VectorMapCurbs.Last().Width;
            LineRenderer.startColor = LineRenderer.endColor = Color.cyan;
            LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }
    class Area : SlicesLine
    {
        public ADASMapArea VectorMapArea { get; set; }
        public override void Init()
        {
            VectorMapLines = VectorMapArea.SLine.GetRangeToEnd(VectorMapArea.ELine);
            base.Init();
            base.Refresh();
            LineRenderer.loop = true;
        }
    }
    class CrossWalk : Area
    {
        public ADASMapCrossWalk VectorMapCrossWalk { get; set; }
        public override void Init()
        {
            VectorMapArea = VectorMapCrossWalk.Area;
            base.Init();
            name = VectorMapCrossWalk.ID.ToString();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.1f;
            LineRenderer.startColor = LineRenderer.endColor = Color.white;
            LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }
    class RoadMark : Area
    {
        public ADASMapRoadMark VectorMapRoadMark { get; set; }
        public override void Init()
        {
            VectorMapArea = VectorMapRoadMark.Area;
            base.Init();
            name = VectorMapRoadMark.ID.ToString();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.1f;
            LineRenderer.startColor = LineRenderer.endColor = Color.white;
            LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }
    class ZebraZone : Area
    {
        public ADASMapZebraZone VectorMapZebraZone { get; set; }
        public override void Init()
        {
            VectorMapArea = VectorMapZebraZone.Area;
            base.Init();
            name = VectorMapZebraZone.ID.ToString();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.1f;
            LineRenderer.startColor = LineRenderer.endColor = Color.white;
            LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }
    }
}