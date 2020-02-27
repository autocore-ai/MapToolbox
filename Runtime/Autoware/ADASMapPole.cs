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
    class ADASMapPole : ADASMapElement<ADASMapPole>
    {
        public int VID { get; set; }
        public float Length { get; set; }
        public float Dim { get; set; }
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
        public override string ToString() => $"{ID},{Vector.ID},{Length},{Dim}";
        const string file = "pole.csv";
        const string header = "PLID,VID,Length,Dim";
        public static void ReadPath(string path)
        {
            Clear();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapPole
                    {
                        ID = int.Parse(item[0]),
                        VID = int.Parse(item[1]),
                        Length = float.Parse(item[2]),
                        Dim = float.Parse(item[3])
                    };
                }
            }
        }
        public static void PreWrite(string path)
        {
            Clear();
            Utils.CleanOrCreateNew(path, file, header);
        }
        public static void WriteCsv(string path)
        {
            ReIndex();
            Utils.AppendData(path, file, List.Select(_ => _.ToString()));
        }
    }
}