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

namespace Packages.MapToolbox.VectorMapTools.Export
{
    public class CsvLine
    {
        public const string fileName = "line.csv";
        public const string header = "LID,BPID,FPID,BLID,FLID";
        public CsvLine lineLast;
        public CsvLine lineNext;
        public CsvPoint PointBegin { get; set; }
        public CsvPoint PointFinal { get; set; }
        public CsvLine LineLast
        {
            get => lineLast;
            set
            {
                lineLast = value;
                lineLast.lineNext = this;
            }
        }
        public CsvLine LineNext
        {
            get => lineNext;
            set
            {
                lineNext = value;
                lineLast.lineLast = this;
            }
        }
        public int LID { get; set; }
        public int? BPID => PointBegin?.PID;
        public int? FPID => PointFinal?.PID;
        public int? BLID => LineLast?.LID;
        public int? FLID => LineNext?.LID;
        public string CsvString => $"{LID},{BPID ?? 0},{FPID ?? 0},{BLID ?? 0},{FLID ?? 0}";
    }
}