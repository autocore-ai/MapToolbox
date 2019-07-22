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
using System.IO;
using System.Linq;
using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools.Export
{
    public class VectorMapCollection
    {
        public PointOctree<CsvLane> finalLanes;
        public List<CsvPoint> csvPoints;
        public List<CsvNode> csvNodes;
        public List<CsvDtLane> csvDtLanes;
        public List<CsvLane> csvLanes;
        public List<CsvLine> csvLines;
        public List<CsvVector> csvVectors;
        public List<CsvSignalLight> csvSignalLights;
        public List<CsvStopLine> csvStopLines;
        public List<CsvWhiteLine> csvWhiteLines;
        public List<CsvRoadEdge> csvRoadEdges;
        public List<CsvCurb> csvCurbs;
        public VectorMapCollection()
        {
            ClearAll();
            GetLanes();
            GetStopLines();
            GetWhiteLines();
            GetRoadEdges();
            GetCurbs();
            AddIsolatedSignalLight();
            SetupIndex();
        }
        private void AddIsolatedSignalLight()
        {
            List<SignalLight> linked = new List<SignalLight>();
            foreach (var item in StopLine.List)
            {
                linked.AddRange(item.signalLights.Where(_ => _ != null));
            }
            foreach (var item in SignalLight.List)
            {
                if (!linked.Contains(item))
                {
                    var csvPoint = new CsvPoint() { Position = item.transform.position };
                    csvPoints.Add(csvPoint);
                    var csvVector = new CsvVector() { Point = csvPoint, Rotation = item.transform.rotation };
                    csvVectors.Add(csvVector);
                    var csvSignalLight = new CsvSignalLight() { Vector = csvVector };
                    csvSignalLights.Add(csvSignalLight);
                }
            }
        }
        private void GetCurbs()
        {
            foreach (var item in Curb.List)
            {
                item.GetCsvData(
                    out List<CsvPoint> points,
                    out List<CsvLine> lines,
                    out List<CsvCurb> curbs);
                csvPoints.AddRange(points);
                csvLines.AddRange(lines);
                csvCurbs.AddRange(curbs);
            }
        }
        private void GetRoadEdges()
        {
            foreach (var item in RoadEdge.List)
            {
                item.GetCsvData(
                    out List<CsvPoint> points,
                    out List<CsvLine> lines,
                    out List<CsvRoadEdge> roadEdges);
                csvPoints.AddRange(points);
                csvLines.AddRange(lines);
                csvRoadEdges.AddRange(roadEdges);
            }
        }
        private void GetWhiteLines()
        {
            foreach (var item in WhiteLine.List)
            {
                item.GetCsvData(
                    out List<CsvPoint> points,
                    out List<CsvLine> lines,
                    out List<CsvWhiteLine> whiteLines);
                csvPoints.AddRange(points);
                csvLines.AddRange(lines);
                csvWhiteLines.AddRange(whiteLines);
            }
        }
        private void GetStopLines()
        {
            foreach (var item in StopLine.List)
            {
                item.GetCsvData(finalLanes,
                    out List<CsvPoint> points,
                    out List<CsvLine> lines,
                    out List<CsvVector> vectors,
                    out List<CsvSignalLight> signalLights,
                    out List<CsvStopLine> stopLines);
                csvPoints.AddRange(points);
                csvLines.AddRange(lines);
                csvVectors.AddRange(vectors);
                csvSignalLights.AddRange(signalLights);
                csvStopLines.AddRange(stopLines);
            }
        }
        private void GetLanes()
        {
            foreach (var item in Lane.List)
            {
                item.GetCsvData(
                    out List<CsvPoint> points,
                    out List<CsvNode> nodes,
                    out List<CsvDtLane> dtlanes,
                    out List<CsvLane> lanes);
                csvPoints.AddRange(points);
                csvNodes.AddRange(nodes);
                csvDtLanes.AddRange(dtlanes);
                csvLanes.AddRange(lanes);
                finalLanes.Add(lanes.LastOrDefault(), lanes.LastOrDefault().FinalNode.Point.Position);
            }
        }
        private void ClearAll()
        {
            finalLanes = new PointOctree<CsvLane>(1000, Vector3.zero, 100);
            csvPoints = new List<CsvPoint>();
            csvNodes = new List<CsvNode>();
            csvDtLanes = new List<CsvDtLane>();
            csvLanes = new List<CsvLane>();
            csvLines = new List<CsvLine>();
            csvVectors = new List<CsvVector>();
            csvSignalLights = new List<CsvSignalLight>();
            csvStopLines = new List<CsvStopLine>();
            csvStopLines = new List<CsvStopLine>();
            csvWhiteLines = new List<CsvWhiteLine>();
            csvRoadEdges = new List<CsvRoadEdge>();
            csvCurbs = new List<CsvCurb>();
        }
        private void SetupIndex()
        {
            for (int i = 0; i < csvPoints.Count; i++)
            {
                csvPoints[i].PID = i + 1;
            }
            for (int i = 0; i < csvNodes.Count; i++)
            {
                csvNodes[i].NID = i + 1;
            }
            for (int i = 0; i < csvDtLanes.Count; i++)
            {
                csvDtLanes[i].DID = i + 1;
            }
            for (int i = 0; i < csvLanes.Count; i++)
            {
                csvLanes[i].LnID = i + 1;
            }
            for (int i = 0; i < csvLines.Count; i++)
            {
                csvLines[i].LID = i + 1;
            }
            for (int i = 0; i < csvVectors.Count; i++)
            {
                csvVectors[i].VID = i + 1;
            }
            for (int i = 0; i < csvSignalLights.Count; i++)
            {
                csvSignalLights[i].ID = i + 1;
            }
            for (int i = 0; i < csvStopLines.Count; i++)
            {
                csvStopLines[i].ID = i + 1;
            }
            for (int i = 0; i < csvWhiteLines.Count; i++)
            {
                csvWhiteLines[i].ID = i + 1;
            }
            for (int i = 0; i < csvRoadEdges.Count; i++)
            {
                csvRoadEdges[i].ID = i + 1;
            }
            for (int i = 0; i < csvCurbs.Count; i++)
            {
                csvCurbs[i].ID = i + 1;
            }
        }
        public void WriteFiles(string folder)
        {
            if (csvPoints.Count > 0)
            {
                var ls = new List<string>() { CsvPoint.header };
                ls.AddRange(csvPoints.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvPoint.fileName), ls);
            }
            if (csvNodes.Count > 0)
            {
                var ls = new List<string>() { CsvNode.header };
                ls.AddRange(csvNodes.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvNode.fileName), ls);
            }
            if (csvDtLanes.Count > 0)
            {
                var ls = new List<string>() { CsvDtLane.header };
                ls.AddRange(csvDtLanes.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvDtLane.fileName), ls);
            }
            if (csvLanes.Count > 0)
            {
                var ls = new List<string>() { CsvLane.header };
                ls.AddRange(csvLanes.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvLane.fileName), ls);
            }
            if (csvLines.Count > 0)
            {
                var ls = new List<string>() { CsvLine.header };
                ls.AddRange(csvLines.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvLine.fileName), ls);
            }
            if (csvVectors.Count > 0)
            {
                var ls = new List<string>() { CsvVector.header };
                ls.AddRange(csvVectors.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvVector.fileName), ls);
            }
            if (csvSignalLights.Count > 0)
            {
                var ls = new List<string>() { CsvSignalLight.header };
                ls.AddRange(csvSignalLights.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvSignalLight.fileName), ls);
            }
            if (csvStopLines.Count > 0)
            {
                var ls = new List<string>() { CsvStopLine.header };
                ls.AddRange(csvStopLines.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvStopLine.fileName), ls);
            }
            if (csvWhiteLines.Count > 0)
            {
                var ls = new List<string>() { CsvWhiteLine.header };
                ls.AddRange(csvWhiteLines.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvWhiteLine.fileName), ls);
            }
            if (csvRoadEdges.Count > 0)
            {
                var ls = new List<string>() { CsvRoadEdge.header };
                ls.AddRange(csvRoadEdges.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvRoadEdge.fileName), ls);
            }
            if (csvCurbs.Count > 0)
            {
                var ls = new List<string>() { CsvCurb.header };
                ls.AddRange(csvCurbs.Select(_ => _.CsvString));
                File.WriteAllLines(Path.Combine(folder, CsvCurb.fileName), ls);
            }
        }
    }
}