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
    class ADASMapBox : ADASMapElement<ADASMapBox> 
    {
        public int PID1 { get; set; }
        public int PID2 { get; set; }
        public int PID3 { get; set; }
        public int PID4 { get; set; }
        public float Height { get; set; }
        ADASMapPoint point1;
        public ADASMapPoint Point1
        {
            set => point1 = value;
            get
            {
                if (point1 == null && ADASMapPoint.Dic.TryGetValue(PID1, out ADASMapPoint value))
                {
                    point1 = value;
                }
                return point1;
            }
        }
        ADASMapPoint point2;
        public ADASMapPoint Point2
        {
            set => point2 = value;
            get
            {
                if (point2 == null && ADASMapPoint.Dic.TryGetValue(PID2, out ADASMapPoint value))
                {
                    point2 = value;
                }
                return point2;
            }
        }
        ADASMapPoint point3;
        public ADASMapPoint Point3
        {
            set => point3 = value;
            get
            {
                if (point3 == null && ADASMapPoint.Dic.TryGetValue(PID3, out ADASMapPoint value))
                {
                    point3 = value;
                }
                return point3;
            }
        }
        ADASMapPoint point4;
        public ADASMapPoint Point4
        {
            set => point4 = value;
            get
            {
                if (point4 == null && ADASMapPoint.Dic.TryGetValue(PID4, out ADASMapPoint value))
                {
                    point4 = value;
                }
                return point4;
            }
        }
        public override string ToString() => $"{ID},{Point1.ID},{Point2.ID},{Point3.ID},{Point4.ID},{Height}";
        const string file = "box.csv";
        const string header = "BID,PID1,PID2,PID3,PID4,Height";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapBox
                    {
                        ID = int.Parse(item[0]),
                        PID1 = int.Parse(item[1]),
                        PID2 = int.Parse(item[2]),
                        PID3 = int.Parse(item[3]),
                        PID4 = int.Parse(item[4]),
                        Height = float.Parse(item[5])
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