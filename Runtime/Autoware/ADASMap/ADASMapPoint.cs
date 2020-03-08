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
    class ADASMapPoint : ADASMapElement<ADASMapPoint>
    {
        #region DATA
        public float B { get; set; }
        public float L { get; set; }
        public float H { get; set; }
        public float Bx { get; set; }
        public float Ly { get; set; }
        public int ReF { get; set; }
        public int MCODE1 { get; set; }
        public int MCODE2 { get; set; }
        public int MCODE3 { get; set; }
        public Vector3 Position
        {
            get => new Vector3(Ly, H, Bx);
            set
            {
                Ly = value.x;
                H = value.y;
                Bx = value.z;
            }
        }
        #endregion
        public override string ToString() => $"{ID},{B},{L},{H},{Bx},{Ly},{ReF},{MCODE1},{MCODE2},{MCODE3}";
        const string file = "point.csv";
        const string header = "PID,B,L,H,Bx,Ly,ReF,MCODE1,MCODE2,MCODE3";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapPoint
                    {
                        ID = int.Parse(item[0]),
                        B = float.Parse(item[1]),
                        L = float.Parse(item[2]),
                        H = float.Parse(item[3]),
                        Bx = float.Parse(item[4]),
                        Ly = float.Parse(item[5]),
                        ReF = int.Parse(item[6]),
                        MCODE1 = int.Parse(item[7]),
                        MCODE2 = int.Parse(item[8]),
                        MCODE3 = int.Parse(item[9])
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