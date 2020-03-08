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


using System.Linq;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASMapRoadEdge : ADASMapElement<ADASMapRoadEdge>
    {
        public int LID { get; set; }
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
        public override string ToString() => $"{ID},{Line.ID},{(LinkLane != null ? LinkLane.ID : 0)}";
        const string file = "roadedge.csv";
        const string header = "ID,LID,LinkID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapRoadEdge
                    {
                        ID = int.Parse(item[0]),
                        LID = int.Parse(item[1]),
                        LinkID = int.Parse(item[2]),
                    };
                }
            }
        }
        public static void WriteCsv(string path)
        {
            ReIndex();
            Utils.CleanOrCreateNew(path, file, header);
            Utils.AppendData(path, file, List.Select(_ => _.ToString()));
        }
    }
}