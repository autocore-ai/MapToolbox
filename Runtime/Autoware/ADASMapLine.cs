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


using System.Collections.Generic;
using System.Linq;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASMapLine : ADASMapElement<ADASMapLine>
    {
        public int BPID { get; set; }
        public int FPID { get; set; }
        public int BLID { get; set; }
        public int FLID { get; set; }
        ADASMapPoint bPoint;
        public ADASMapPoint BPoint
        {
            set => bPoint = value;
            get
            {
                if (bPoint == null && ADASMapPoint.Dic.TryGetValue(BPID, out ADASMapPoint value))
                {
                    bPoint = value;
                }
                return bPoint;
            }
        }
        ADASMapPoint fPoint;
        public ADASMapPoint FPoint
        {
            set => fPoint = value;
            get
            {
                if (fPoint == null && ADASMapPoint.Dic.TryGetValue(FPID, out ADASMapPoint value))
                {
                    fPoint = value;
                }
                return fPoint;
            }
        }
        ADASMapLine bLine;
        public ADASMapLine BLine
        {
            set => bLine = value;
            get
            {
                if (bLine == null && Dic.TryGetValue(BLID, out ADASMapLine value))
                {
                    bLine = value;
                }
                return bLine;
            }
        }
        ADASMapLine fLine;
        public ADASMapLine FLine
        {
            set => fLine = value;
            get
            {
                if (fLine == null && Dic.TryGetValue(FLID, out ADASMapLine value))
                {
                    fLine = value;
                }
                return fLine;
            }
        }
        public ADASMapLine FirstLine { get; set; }
        public ADASMapLine FinalLine { get; set; }
        private ADASMapLine GetFirstLine() => BLID == 0 ? this : BLine.GetFirstLine();
        private ADASMapLine GetFinalLine() => FLID == 0 ? this : FLine.GetFinalLine();
        public override string ToString() => $"{ID},{BPoint.ID},{FPoint.ID},{BLine.ID},{FLine.ID}";
        const string file = "line.csv";
        const string header = "LID,BPID,FPID,BLID,FLID";
        public IEnumerable<ADASMapLine> GetRangeToEnd(ADASMapLine end)
        {
            List<ADASMapLine> ret = new List<ADASMapLine> { this };
            for (ADASMapLine i = this; i.FLine != null; i = i.FLine)
            {
                ret.Add(i.FLine);
                if (i.FLine.Equals(end))
                {
                    return ret;
                }
            }
            ret.Clear();
            return ret;
        }
        public static void ReadCsv(string path)
        {
            Clear();
            var data = Utils.ReadLinesExcludeFirstLine(path, file);
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data.Split(','))
                {
                    new ADASMapLine
                    {
                        ID = int.Parse(item[0]),
                        BPID = int.Parse(item[1]),
                        FPID = int.Parse(item[2]),
                        BLID = int.Parse(item[3]),
                        FLID = int.Parse(item[4]),
                    };
                }
                foreach (var item in List)
                {
                    item.FirstLine = item.GetFirstLine();
                    item.FinalLine = item.GetFinalLine();
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