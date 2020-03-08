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
    class ADASMapGutter : ADASMapElement<ADASMapGutter>
    {
        public int AID { get; set; }
        public enum Type : int
        {
            NO_COVER = 0,
            COVER = 1,
            GRATING = 2
        }
        public Type RoadMarkType { get; set; }
        public int LinkID { get; set; }
        ADASMapArea area;
        public ADASMapArea Area
        {
            set => area = value;
            get
            {
                if (area == null && ADASMapArea.Dic.TryGetValue(AID, out ADASMapArea value))
                {
                    area = value;
                }
                return area;
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
        public override string ToString() => $"{ID},{Area.ID},{(int)RoadMarkType},{LinkLane.ID}";
        const string file = "gutter.csv";
        const string header = "ID,AID,Type,LinkID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapGutter
                    {
                        ID = int.Parse(item[0]),
                        AID = int.Parse(item[1]),
                        RoadMarkType = (Type)int.Parse(item[2]),
                        LinkID = int.Parse(item[3])
                    };
                }
            }
        }
        public static void PreWrite(string path)
        {
            Reset();
            Utils.CleanOrCreateNew(path, file, header);
        }
        public static void WriteCsv(string path)
        {
            ReIndex();
            Utils.AppendData(path, file, List.Select(_ => _.ToString()));
        }
    }
}