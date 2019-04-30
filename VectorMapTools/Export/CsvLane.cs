#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Collections.Generic;
using System.Linq;

namespace Packages.UnityTools.VectorMapTools.Export
{
    class CsvLane
    {
        internal const string fileName = "lane.csv";
        internal const string header = "LnID,DID,BLID,FLID,BNID,FNID,JCT,BLID2,BLID3,BLID4,FLID2,FLID3,FLID4,ClossID,Span,LCnt,Lno,LaneType,LimitVel,RefVel,RoadSecID,LaneChgFG";
        private CsvDtLane dtLaneFinal;
        public CsvDtLane DtLaneBegin { get; set; }
        public CsvDtLane DtLaneFinal
        {
            get => dtLaneFinal;
            set
            {
                dtLaneFinal = value;
                if (dtLaneFinal.LastDtLane != null)
                {
                    DtLaneBegin = dtLaneFinal.LastDtLane;
                }
            }
        }
        public CsvNode BeginNode { get; set; }
        public CsvNode FinalNode { get; set; }
        public List<CsvLane> BeginLanes { get; } = new List<CsvLane>();
        public List<CsvLane> FinalLanes { get; } = new List<CsvLane>();
        public void AddPreLane(CsvLane csvLane)
        {
            BeginLanes.Add(csvLane);
            csvLane.FinalLanes.Add(this);
        }
        public int LnID { get; set; }
        public int? DID => DtLaneBegin?.DID;
        public int? BLID => BeginLanes?.ElementAtOrDefault(0)?.LnID;
        public int? BLID2 => BeginLanes?.ElementAtOrDefault(1)?.LnID;
        public int? BLID3 => BeginLanes?.ElementAtOrDefault(2)?.LnID;
        public int? BLID4 => BeginLanes?.ElementAtOrDefault(3)?.LnID;
        public int? FLID => FinalLanes?.ElementAtOrDefault(0)?.LnID;
        public int? FLID2 => FinalLanes?.ElementAtOrDefault(1)?.LnID;
        public int? FLID3 => FinalLanes?.ElementAtOrDefault(2)?.LnID;
        public int? FLID4 => FinalLanes?.ElementAtOrDefault(3)?.LnID;
        public int? BNID => BeginNode?.NID;
        public int? FNID => FinalNode?.NID;
        public float Velocity { get; set; }
        public string CsvString => $"{LnID},{DID ?? 0},{BLID ?? 0},{FLID ?? 0},{BNID ?? 0},{FNID ?? 0},0,{BLID2 ?? 0},{BLID3 ?? 0},{BLID4 ?? 0},{FLID2 ?? 0},{FLID3 ?? 0},{FLID4 ?? 0},0,1,1,1,0,{Velocity:f3},{Velocity:f3},0,0";
    }
}