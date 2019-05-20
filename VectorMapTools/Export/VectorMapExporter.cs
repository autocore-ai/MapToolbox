#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools.Export
{
    public class VectorMapExporter
    {
        public static void ExportMaps(string folder)
        {
            PointOctree<CsvLane> finalLanes = new PointOctree<CsvLane>(1000, Vector3.zero, 100);
            List<CsvPoint> csvPoints = new List<CsvPoint>();
            List<CsvNode> csvNodes = new List<CsvNode>();
            List<CsvDtLane> csvDtLanes = new List<CsvDtLane>();
            List<CsvLane> csvLanes = new List<CsvLane>();
            List<CsvLine> csvLines = new List<CsvLine>();
            List<CsvVector> csvVectors = new List<CsvVector>();
            List<CsvSignalLight> csvSignalLights = new List<CsvSignalLight>();
            List<CsvStopLine> csvStopLines = new List<CsvStopLine>();
            List<CsvWhiteLine> csvWhiteLines = new List<CsvWhiteLine>();
            List<CsvRoadEdge> csvRoadEdges = new List<CsvRoadEdge>();
            List<CsvCurb> csvCurbs = new List<CsvCurb>();
            var lastLane = csvLanes.LastOrDefault();
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