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
    class ADASMapSignal : ADASMapElement<ADASMapSignal>
    {
        public int VID { get; set; }
        public int PLID { get; set; }
        public enum Type : int
        {
            RED = 1,
            BLUE = 2,
            YELLOW = 3,
            PEDESTRIAN_RED = 4,
            PEDESTRIAN_BLUE = 5,
            OTHER = 9,
            RED_LEFT = 21,
            BLUE_LEFT = 22,
            YELLOW_LEFT = 23
        }
        public Type SignalType { get; set; }
        public int LinkID { get; set; }
        ADASMapVector vector;
        public ADASMapVector Vector
        {
            set => vector = value;
            get
            {
                if (vector == null && ADASMapVector.Dic.TryGetValue(VID, out ADASMapVector value))
                {
                    vector = value;
                }
                return vector;
            }
        }
        ADASMapPole pole;
        public ADASMapPole Pole
        {
            set => pole = value;
            get
            {
                if (pole == null && ADASMapPole.Dic.TryGetValue(PLID, out ADASMapPole value))
                {
                    pole = value;
                }
                return pole;
            }
        }
        ADASMapLane link;
        public ADASMapLane LinkLane
        {
            set => link = value;
            get
            {
                if (link == null && ADASMapLane.Dic.TryGetValue(LinkID, out ADASMapLane value))
                {
                    link = value;
                }
                return link;
            }
        }
        public override string ToString() => $"{ID},{Vector.ID},{(Pole != null ? Pole.ID : 0)},{(int)SignalType},{(LinkLane != null ? LinkLane.ID : 0)}";
        const string file = "signaldata.csv";
        const string header = "ID,VID,PLID,Type,LinkID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapSignal
                    {
                        ID = int.Parse(item[0]),
                        VID = int.Parse(item[1]),
                        PLID = int.Parse(item[2]),
                        SignalType = (Type)int.Parse(item[3]),
                        LinkID = int.Parse(item[4])
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