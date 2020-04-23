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
    class ADASMapUtilityPole : ADASMapElement<ADASMapUtilityPole>
    {
        public int PLID { get; set; }
        public int LinkID { get; set; }
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
        public override string ToString() => $"{ID},{Pole.ID},{LinkLane.ID}";
        const string file = "utilitypole.csv";
        const string header = "ID,PLID,LinkID";
        public static void ReadPath(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapUtilityPole
                    {
                        ID = int.Parse(item[0]),
                        PLID = int.Parse(item[1]),
                        LinkID = int.Parse(item[2])
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