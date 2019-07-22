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
    class CsvWhiteLine
    {
        internal const string fileName = "whiteline.csv";
        internal const string header = "ID,LID,Width,Color,type,LinkID";
        public CsvLine Line { get; set; }
        public int ID { get; set; }
        public int? LID => Line?.LID;
        public float Width { get; set; }
        public string CsvString => $"{ID},{LID ?? 0},{Width:F3},W,0,0";
        public static implicit operator Roslin.Msg.vector_map_msgs.WhiteLine(CsvWhiteLine csvWhiteLine)
        {
            return new Roslin.Msg.vector_map_msgs.WhiteLine
            {
                id = csvWhiteLine.ID,
                lid = csvWhiteLine.LID ?? 0,
                width = csvWhiteLine.Width,
                color = (sbyte)'w',
                type = 0,
                linkid = 0,
            };
        }
    }
}