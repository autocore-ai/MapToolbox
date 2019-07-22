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

namespace Packages.MapToolbox.VectorMapTools.Export
{
    static class VectorMapExtern
    {
        public static void GetCsvData(this Lane lane,
            out List<CsvPoint> csvPoints,
            out List<CsvNode> csvNodes,
            out List<CsvDtLane> csvDtLanes,
            out List<CsvLane> csvLanes)
        {
            csvPoints = new List<CsvPoint>();
            csvNodes = new List<CsvNode>();
            csvDtLanes = new List<CsvDtLane>();
            csvLanes = new List<CsvLane>();
            for (int i = 0; i < lane.LineRenderer.positionCount; i++)
            {
                var csvPoint = new CsvPoint() { Position = lane.LineRenderer.GetPosition(i) };
                csvPoints.Add(csvPoint);
                var csvNode = new CsvNode() { Point = csvPoint };
                csvNodes.Add(csvNode);
                CsvDtLane csvDtLane;
                if (i > 0)
                {
                    csvDtLane = new CsvDtLane() { Point = csvPoint, LastDtLane = csvDtLanes[i - 1] };
                }
                else
                {
                    csvDtLane = new CsvDtLane() { Point = csvPoint };
                }
                csvDtLanes.Add(csvDtLane);
                if (i > 0)
                {
                    var csvLane = new CsvLane() { DtLaneFinal = csvDtLane, BeginNode = csvNodes[i - 1], FinalNode = csvNode };
                    if (i > 1)
                    {
                        csvLane.AddPreLane(csvLanes[i - 2]);
                    }
                    csvLane.Velocity = Mathf.Lerp(lane.velocityBegin, lane.velocityFinal, (float)i / (lane.LineRenderer.positionCount - 1));
                    csvLanes.Add(csvLane);
                }
            }
        }
        public static void GetCsvData(this StopLine stopLine,
            PointOctree<CsvLane> finalLanes,
            out List<CsvPoint> csvPoints,
            out List<CsvLine> csvLines,
            out List<CsvVector> csvVectors,
            out List<CsvSignalLight> csvSignalLights,
            out List<CsvStopLine> csvStopLines)
        {
            csvPoints = new List<CsvPoint>();
            csvLines = new List<CsvLine>();
            csvStopLines = new List<CsvStopLine>();
            var csvSLPoints = new List<CsvPoint>();
            csvVectors = new List<CsvVector>();
            csvSignalLights = new List<CsvSignalLight>();
            for (int i = 0; i < stopLine.Count; i++)
            {
                var csvPoint = new CsvPoint() { Position = stopLine.LineRenderer.GetPosition(i) };
                csvPoints.Add(csvPoint);
                if (i > 0)
                {
                    var csvLine = new CsvLine() { PointBegin = csvPoints[i - 1], PointFinal = csvPoint };
                    if (i > 1)
                    {
                        csvLine.LineLast = csvLines.Last();
                    }
                    csvLines.Add(csvLine);
                    var signalLight = stopLine.signalLights[i - 1];
                    var csvSLPoint = new CsvPoint() { Position = signalLight.transform.position };
                    csvSLPoints.Add(csvSLPoint);
                    var csvVector = new CsvVector() { Point = csvSLPoint, Rotation = signalLight.transform.rotation };
                    csvVectors.Add(csvVector);
                    var csvSignalLight = new CsvSignalLight() { Vector = csvVector  };
                    csvSignalLights.Add(csvSignalLight);
                    var midPoint = (csvLine.PointBegin.Position + csvLine.PointFinal.Position) / 2;
                    var csvStopLine = new CsvStopLine()
                    {
                        Line = csvLine,
                        SignalLight = csvSignalLight,
                        Lane = finalLanes
                        .GetNearby(midPoint, Vector3.Distance(csvLine.PointBegin.Position, csvLine.PointFinal.Position))
                        ?.OrderBy(_ => Vector3.Distance(_.FinalNode.Point.Position, midPoint))?
                        .FirstOrDefault()
                    };
                    csvStopLines.Add(csvStopLine);
                }
            }
            csvPoints.AddRange(csvSLPoints);
        }
        public static void GetCsvData(this WhiteLine whiteLine,
            out List<CsvPoint> csvPoints,
            out List<CsvLine> csvLines,
            out List<CsvWhiteLine> csvWhiteLines)
        {
            csvPoints = new List<CsvPoint>();
            csvLines = new List<CsvLine>();
            csvWhiteLines = new List<CsvWhiteLine>();
            for (int i = 0; i < whiteLine.LineRenderer.positionCount; i++)
            {
                var csvPoint = new CsvPoint() { Position = whiteLine.LineRenderer.GetPosition(i) };
                csvPoints.Add(csvPoint);
                if (i > 0)
                {
                    var csvLine = new CsvLine() { PointBegin = csvPoints[i - 1], PointFinal = csvPoint };
                    if (i > 1)
                    {
                        csvLine.LineLast = csvLines.Last();
                    }
                    csvLines.Add(csvLine);
                    csvWhiteLines.Add(new CsvWhiteLine() { Line = csvLine, Width = whiteLine.width });
                }
            }
        }
        public static void GetCsvData(this RoadEdge roadEdge,
            out List<CsvPoint> csvPoints,
            out List<CsvLine> csvLines,
            out List<CsvRoadEdge> csvRoadEdges)
        {
            csvPoints = new List<CsvPoint>();
            csvLines = new List<CsvLine>();
            csvRoadEdges = new List<CsvRoadEdge>();
            for (int i = 0; i < roadEdge.LineRenderer.positionCount; i++)
            {
                var csvPoint = new CsvPoint() { Position = roadEdge.LineRenderer.GetPosition(i) };
                csvPoints.Add(csvPoint);
                if (i > 0)
                {
                    var csvLine = new CsvLine() { PointBegin = csvPoints[i - 1], PointFinal = csvPoint };
                    if (i > 1)
                    {
                        csvLine.LineLast = csvLines.Last();
                    }
                    csvLines.Add(csvLine);
                    csvRoadEdges.Add(new CsvRoadEdge() { Line = csvLine });
                }
            }
        }
        public static void GetCsvData(this Curb curb,
            out List<CsvPoint> csvPoints,
            out List<CsvLine> csvLines,
            out List<CsvCurb> csvCurbs)
        {
            csvPoints = new List<CsvPoint>();
            csvLines = new List<CsvLine>();
            csvCurbs = new List<CsvCurb>();
            for (int i = 0; i < curb.LineRenderer.positionCount; i++)
            {
                var csvPoint = new CsvPoint() { Position = curb.LineRenderer.GetPosition(i) };
                csvPoints.Add(csvPoint);
                if (i > 0)
                {
                    var csvLine = new CsvLine() { PointBegin = csvPoints[i - 1], PointFinal = csvPoint };
                    if (i > 1)
                    {
                        csvLine.LineLast = csvLines.Last();
                    }
                    csvLines.Add(csvLine);
                    csvCurbs.Add(new CsvCurb() { Line = csvLine });
                }
            }
        }
    }
}