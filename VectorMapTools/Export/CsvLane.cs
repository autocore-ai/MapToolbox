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

namespace Packages.MapToolbox.VectorMapTools.Export
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
        public static implicit operator Roslin.Msg.vector_map_msgs.Lane(CsvLane csvLane)
        {
            return new Roslin.Msg.vector_map_msgs.Lane
            {
                lnid = csvLane.LnID,
                did = csvLane.DID ?? 0,
                blid = csvLane.BLID ?? 0,
                flid = csvLane.FLID ?? 0,
                bnid = csvLane.BNID ?? 0,
                fnid = csvLane.FNID ?? 0,
                jct = 0,
                blid2 = csvLane.BLID2 ?? 0,
                blid3 = csvLane.BLID3 ?? 0,
                blid4 = csvLane.BLID4 ?? 0,
                flid2 = csvLane.FLID2 ?? 0,
                flid3 = csvLane.FLID3 ?? 0,
                flid4 = csvLane.FLID4 ?? 0,
                clossid = 0,
                span = 1,
                lcnt = 1,
                lno = 1,
                lanetype = 0,
                limitvel = (int)csvLane.Velocity,
                refvel = (int)csvLane.Velocity,
                roadsecid = 0,
                lanecfgfg = 0,
                linkwaid = 0
            };
        }
    }
}