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

namespace AutoCore.MapToolbox.Autoware
{
    class ADASMapStopLine : ADASMapElement<ADASMapStopLine>
    {
        public int LID { get; set; }
        public int TLID { get; set; }
        public int SignID { get; set; }
        public int LinkID { get; set; }
        ADASMapLine line;
        public ADASMapLine Line
        {
            set => line = value;
            get
            {
                if (line == null && ADASMapLine.Dic.TryGetValue(LID, out ADASMapLine value))
                {
                    line = value;
                }
                return line;
            }
        }
        ADASMapSignal signal;
        public ADASMapSignal Signal
        {
            set => signal = value;
            get
            {
                if (signal == null && ADASMapSignal.Dic.TryGetValue(LID, out ADASMapSignal value))
                {
                    signal = value;
                }
                return signal;
            }
        }
        ADASMapRoadSign roadSign;
        public ADASMapRoadSign RoadSign
        {
            set => roadSign = value;
            get
            {
                if (roadSign == null && ADASMapRoadSign.Dic.TryGetValue(SignID, out ADASMapRoadSign value))
                {
                    roadSign = value;
                }
                return roadSign;
            }
        }
        ADASMapLane linkLane;
        public ADASMapLane LinkLane
        {
            set => linkLane = value;
            get
            {
                if (linkLane == null && ADASMapLane.Dic.TryGetValue(LinkID, out ADASMapLane value))
                {
                    linkLane = value;
                }
                return linkLane;
            }
        }
        public override string ToString() => $"{ID},{Line.ID},{(Signal != null ? Signal.ID : 0)},{(RoadSign != null ? RoadSign.ID : 0)},{(LinkLane != null ? LinkLane.ID : 0)}";
        const string file = "stopline.csv";
        const string header = "ID,LID,TLID,SignID,LinkID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapStopLine
                    {
                        ID = int.Parse(item[0]),
                        LID = int.Parse(item[1]),
                        TLID = int.Parse(item[2]),
                        SignID = int.Parse(item[3]),
                        LinkID = int.Parse(item[4]),
                    };
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