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


using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoLine : LineSegment<ADASGoLine>, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        [HideInInspector] public ADASGoLine bLine;
        [HideInInspector] public ADASGoLine fLine;
        public override Vector3 From
        {
            get => base.From;
            set
            {
                base.From = value;
                if (bLine)
                {
                    bLine.localTo = localFrom;
                }
            }
        }
        public override Vector3 To
        {
            get => base.To;
            set
            {
                base.To = value;
                if (fLine)
                {
                    fLine.localFrom = localTo;
                }
            }
        }
        ADASMapLine line;
        public ADASMapLine Line
        {
            set
            {
                line = value;
                if (line != null)
                {
                    From = line.BPoint.Position;
                    To = line.FPoint.Position;
                    transform.position = (From + To) / 2;
                }
            }
            get
            {
                if (line == null)
                {
                    line = new ADASMapLine
                    {
                        BPoint = new ADASMapPoint { Position = From }
                    };
                    if (last)
                    {
                        var lastLine = last.GetComponent<ADASGoLine>();
                        lastLine.line.FPoint = line.BPoint;
                        line.BLine = lastLine.line;
                        lastLine.line.FLine = line;
                    }
                    if (next == null)
                    {
                        line.FPoint = new ADASMapPoint { Position = To };
                    }
                }
                return line;
            }
        }
        public override void UpdateRef()
        {
            base.UpdateRef();
            if (bLine == null && last != null)
            {
                bLine = last.GetComponent<ADASGoLine>();
            }
            if (fLine == null && next != null)
            {
                fLine = next.GetComponent<ADASGoLine>();
            }
        }
        internal virtual void BuildData()
        {
            line = null;
            line = Line;
        }
    }
}