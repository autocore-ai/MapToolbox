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
    class ADASMapCurveMirror : ADASMapElement<ADASMapCurveMirror>
    {
        public int VID { get; set; }
        public int PLID { get; set; }
        public int CurveMirrorType { get; set; }
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
        public override string ToString() => $"{ID},{Vector.ID},{Pole.ID},{CurveMirrorType},{LinkLane.ID}";
        const string file = "curvemirror.csv";
        const string header = "ID,VID,PLID,Type,LinkID";
        public static void ReadCsv(string path)
        {
            Reset();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapCurveMirror
                    {
                        ID = int.Parse(item[0]),
                        VID = int.Parse(item[1]),
                        PLID = int.Parse(item[2]),
                        CurveMirrorType = int.Parse(item[3]),
                        LinkID = int.Parse(item[4])
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