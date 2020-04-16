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


namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoWhiteLine : ADASGoLine
    {
        public ADASMapWhiteLine.Color color;
        public float width = 0.2f;
        ADASMapWhiteLine whiteLine;
        public ADASMapWhiteLine WhiteLine
        {
            set
            {
                whiteLine = value;
                if (whiteLine != null)
                {
                    Line = whiteLine.Line;
                    name = whiteLine.ID.ToString();
                    width = whiteLine.Width;
                    color = whiteLine.COLOR;
                }
            }
            get
            {
                if (whiteLine == null)
                {
                    whiteLine = new ADASMapWhiteLine
                    {
                        Line = Line,
                        Width = width,
                        COLOR = color
                    };
                }
                return whiteLine;
            }
        }
        internal override void BuildData()
        {
            Line = null;
            WhiteLine = null;
            whiteLine = WhiteLine;
        }
        protected override void DataCopy(LineSegment<ADASGoLine> target)
        {
            base.DataCopy(target);
            var t = target as ADASGoWhiteLine;
            color = t.color;
            width = t.width;
        }
    }
}