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

    class ADASMapArea : ADASMapElement<ADASMapArea>
    {
        public int SLID { get; set; }
        public int ELID { get; set; }
        ADASMapLine sline;
        public ADASMapLine SLine
        {
            set => sline = value;
            get
            {
                if (sline == null && ADASMapLine.Dic.TryGetValue(SLID, out ADASMapLine value))
                {
                    sline = value;
                }
                return sline;
            }
        }
        ADASMapLine eline;
        public ADASMapLine ELine
        {
            set => eline = value;
            get
            {
                if (eline == null && ADASMapLine.Dic.TryGetValue(ELID, out ADASMapLine value))
                {
                    eline = value;
                }
                return eline;
            }
        }
        public override string ToString() => $"{ID},{SLine.ID},{ELine.ID}";
        const string file = "area.csv";
        const string header = "AID,SLID,ELID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapArea
                    {
                        ID = int.Parse(item[0]),
                        SLID = int.Parse(item[1]),
                        ELID = int.Parse(item[2])
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