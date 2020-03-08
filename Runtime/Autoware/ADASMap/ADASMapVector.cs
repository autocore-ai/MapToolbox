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
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASMapVector : ADASMapElement<ADASMapVector>
    {
        public int PID { get; set; }
        public float Hang { get; set; }
        public float Vang { get; set; }
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
        public Quaternion Rotation
        {
            get => Quaternion.Euler(Vang - 90, Hang, 0);
            set
            {
                Vang = value.eulerAngles.x + 90;
                Hang = value.eulerAngles.y;
            }
        }
        public override string ToString() => $"{ID},{Point.ID},{Hang},{Vang}";
        const string file = "vector.csv";
        const string header = "VID,PID,Hang,Vang";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapVector
                    {
                        ID = int.Parse(item[0]),
                        PID = int.Parse(item[1]),
                        Hang = float.Parse(item[2]),
                        Vang = float.Parse(item[3])
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