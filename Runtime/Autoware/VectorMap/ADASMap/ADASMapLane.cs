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
    class ADASMapLane : ADASMapElement<ADASMapLane>
    {
        #region DATA
        public int DID { get; set; }
        public int BLID { get; set; }
        public int FLID { get; set; }
        public int BNID { get; set; }
        public int FNID { get; set; }
        public enum Jct : int
        {
            NORMAL = 0,
            LEFT_BRANCHING = 1,
            RIGHT_BRANCHING = 2,
            LEFT_MERGING = 3,
            RIGHT_MERGING = 4,
            COMPOSITION = 5
        }
        public Jct JCT { get; set; }
        public int BLID2 { get; set; }
        public int BLID3 { get; set; }
        public int BLID4 { get; set; }
        public int FLID2 { get; set; }
        public int FLID3 { get; set; }
        public int FLID4 { get; set; }
        public int ClossID { get; set; }
        public float Span { get; set; }
        public int LCnt { get; set; }
        public int Lno { get; set; }
        public enum Type : int
        {
            STRAIGHT = 0,
            LEFT_TURN = 1,
            RIGHT_TURN = 2
        }
        public Type LaneType { get; set; }
        public int LimitVel { get; set; }
        public int RefVel { get; set; }
        public int RoadSecID { get; set; }
        public enum ChgFG : int
        {
            PASS = 0,
            FAIL = 1
        }
        public ChgFG LaneChgFG { get; set; }
        #endregion
        #region Ref
        ADASMapDTLane dTLane;
        public ADASMapDTLane DTLane
        {
            set => dTLane = value;
            get
            {
                if (dTLane == null && ADASMapDTLane.Dic.TryGetValue(DID, out ADASMapDTLane value))
                {
                    dTLane = value;
                }
                return dTLane;
            }
        }
        ADASMapLane blane;
        public ADASMapLane BLane
        {
            set => blane = value;
            get
            {
                if (blane == null && Dic.TryGetValue(BLID, out ADASMapLane value))
                {
                    blane = value;
                }
                return blane;
            }
        }
        ADASMapLane blane2;
        public ADASMapLane BLane2
        {
            set => blane2 = value;
            get
            {
                if (blane2 == null && Dic.TryGetValue(BLID2, out ADASMapLane value))
                {
                    blane2 = value;
                }
                return blane2;
            }
        }
        ADASMapLane blane3;
        public ADASMapLane BLane3
        {
            set => blane3 = value;
            get
            {
                if (blane3 == null && Dic.TryGetValue(BLID3, out ADASMapLane value))
                {
                    blane3 = value;
                }
                return blane3;
            }
        }

        internal static ADASMapLane NearestLane(Vector3 position)
        {
            if (List.Count > 0)
            {
                return List.OrderBy(_ => Vector3.Distance(position, _.BNode.Point.Position)).First();
            }
            return null;
        }

        ADASMapLane blane4;
        public ADASMapLane BLane4
        {
            set => blane4 = value;
            get
            {
                if (blane4 == null && Dic.TryGetValue(BLID4, out ADASMapLane value))
                {
                    blane4 = value;
                }
                return blane4;
            }
        }
        ADASMapLane flane;
        public ADASMapLane FLane
        {
            set => flane = value;
            get
            {
                if (flane == null && Dic.TryGetValue(FLID, out ADASMapLane value))
                {
                    flane = value;
                }
                return flane;
            }
        }
        ADASMapLane flane2;
        public ADASMapLane FLane2
        {
            set => flane2 = value;
            get
            {
                if (flane2 == null && Dic.TryGetValue(FLID2, out ADASMapLane value))
                {
                    flane2 = value;
                }
                return flane2;
            }
        }
        ADASMapLane flane3;
        public ADASMapLane FLane3
        {
            set => flane3 = value;
            get
            {
                if (flane3 == null && Dic.TryGetValue(FLID3, out ADASMapLane value))
                {
                    flane3 = value;
                }
                return flane3;
            }
        }
        ADASMapLane flane4;
        public ADASMapLane FLane4
        {
            set => flane4 = value;
            get
            {
                if (flane4 == null && Dic.TryGetValue(FLID4, out ADASMapLane value))
                {
                    flane4 = value;
                }
                return flane4;
            }
        }
        ADASMapNode bnode;
        public ADASMapNode BNode
        {
            set => bnode = value;
            get
            {
                if (bnode == null && ADASMapNode.Dic.TryGetValue(BNID, out ADASMapNode value))
                {
                    bnode = value;
                }
                return bnode;
            }
        }
        ADASMapNode fnode;
        public ADASMapNode FNode
        {
            set => fnode = value;
            get
            {
                if (fnode == null && ADASMapNode.Dic.TryGetValue(FNID, out ADASMapNode value))
                {
                    fnode = value;
                }
                return fnode;
            }
        }
        public ADASMapLane FirstLane { get; set; }
        bool EdgeOrCross()
        {
            if (JCT > Jct.NORMAL || BLID == 0)
            {
                return true;
            }
            if (BLID2 > 0 || BLID3 > 0 || BLID4 > 0)
            {
                return true;
            }
            if (Dic.TryGetValue(BLID, out ADASMapLane value))
            {
                return value.FLID2 > 0 || value.FLID3 > 0 || value.FLID4 > 0 || value.JCT > Jct.NORMAL;
            }
            else
            {
                return true;
            }
        }
        bool IsFirstLane()
        {
            if (EdgeOrCross())
            {
                return true;
            }
            return BLane.ID != ID - 1;
        }
        ADASMapLane GetFirstLane() => IsFirstLane() ? this : BLane.GetFirstLane();
        #endregion
        public override string ToString() => $"{ID},{DTLane.ID},{(BLane != null ? BLane.ID : 0)},{(FLane != null ? FLane.ID : 0)},{BNode.ID},{FNode.ID},{(int)JCT},{(BLane2 != null ? BLane2.ID : 0)},{(BLane3 != null ? BLane3.ID : 0)},{(BLane4 != null ? BLane4.ID : 0)},{(FLane2 != null ? FLane2.ID : 0)},{(FLane3 != null ? FLane3.ID : 0)},{(FLane4 != null ? FLane4.ID : 0)},{ClossID},{Span},{LCnt},{Lno},{(int)LaneType},{LimitVel},{RefVel},{RoadSecID},{(int)LaneChgFG}";
        const string file = "lane.csv";
        const string header = "LnID,DID,BLID,FLID,BNID,FNID,JCT,BLID2,BLID3,BLID4,FLID2,FLID3,FLID4,ClossID,Span,LCnt,Lno,LaneType,LimitVel,RefVel,RoadSecID,LaneChgFG";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapLane
                    {
                        ID = int.Parse(item[0]),
                        DID = int.Parse(item[1]),
                        BLID = int.Parse(item[2]),
                        FLID = int.Parse(item[3]),
                        BNID = int.Parse(item[4]),
                        FNID = int.Parse(item[5]),
                        JCT = (Jct)int.Parse(item[6]),
                        BLID2 = int.Parse(item[7]),
                        BLID3 = int.Parse(item[8]),
                        BLID4 = int.Parse(item[9]),
                        FLID2 = int.Parse(item[10]),
                        FLID3 = int.Parse(item[11]),
                        FLID4 = int.Parse(item[12]),
                        ClossID = int.Parse(item[13]),
                        Span = float.Parse(item[14]),
                        LCnt = int.Parse(item[15]),
                        Lno = int.Parse(item[16]),
                        LaneType = (Type)int.Parse(item[17]),
                        LimitVel = int.Parse(item[18]),
                        RefVel = int.Parse(item[19]),
                        RoadSecID = int.Parse(item[20]),
                        LaneChgFG = (ChgFG)int.Parse(item[21]),
                        DTLane = ADASMapDTLane.Dic[int.Parse(item[1])]
                    };
                }
                foreach (var item in List)
                {
                    item.FirstLane = item.GetFirstLane();
                }
            }
        }
        public static void WriteCsv(string path)
        {
            ReIndex();
            Utils.CleanOrCreateNew(path, file, header);
            Utils.AppendData(path, file, List.Select(_ => _.ToString()));
            Utils.RemoveEmpty(path, file);
        }
    }
}