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
    class ADASMapNode : ADASMapElement<ADASMapNode>
    {
        #region DATA
        public int PID { get; set; }
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
        public override string ToString() => $"{ID},{Point.ID}";
        const string file = "node.csv";
        const string header = "NID,PID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapNode
                    {
                        ID = int.Parse(item[0]),
                        PID = int.Parse(item[1])
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