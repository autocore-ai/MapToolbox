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
    class ADASMapDTLane : ADASMapElement<ADASMapDTLane>
    {
        #region DATA
        public float Dist { get; set; }
        public int PID { get; set; }
        public float Dir { get; set; }
        public float Apara { get; set; }
        public float R { get; set; }
        public float Slope { get; set; }
        public float Cant { get; set; }
        public float LW { get; set; }
        public float RW { get; set; }
        #endregion
        ADASMapPoint point;
        public ADASMapPoint Point
        {
            set => point = value;
            get
            {
                if (point == null && ADASMapPoint.Dic.TryGetValue(PID, out ADASMapPoint value))
                {
                    point = value;
                }
                return point;
            }
        }
        public override string ToString() => $"{ID},{Dist:0.##},{Point.ID},{Dir},{Apara},{R},{Slope},{Cant},{LW},{RW}";
        const string file = "dtlane.csv";
        const string header = "DID,Dist,PID,Dir,Apara,r,slope,cant,LW,RW";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapDTLane
                    {
                        ID = int.Parse(item[0]),
                        Dist = float.Parse(item[1]),
                        PID = int.Parse(item[2]),
                        Dir = float.Parse(item[3]),
                        Apara = float.Parse(item[4]),
                        R = float.Parse(item[5]),
                        Slope = float.Parse(item[6]),
                        Cant = float.Parse(item[7]),
                        LW = float.Parse(item[8]),
                        RW = float.Parse(item[9])
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