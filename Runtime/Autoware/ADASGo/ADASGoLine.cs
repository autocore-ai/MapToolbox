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


using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoLine : LineSegment<ADASGoLine>, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        [HideInInspector] public ADASGoLine bLine;
        [HideInInspector] public ADASGoLine fLine;
        ADASMapLine data;
        public ADASMapLine Line
        {
            set
            {
                data = value;
                if (data != null)
                {
                    from = data.BPoint.Position;
                    to = data.FPoint.Position;
                    transform.position = (from + to) / 2;
                }
            }
            get
            {
                if (data == null)
                {
                    data = new ADASMapLine
                    {
                        BPoint = new ADASMapPoint { Position = from }
                    };
                    if (last)
                    {
                        var lastLine = last.GetComponent<ADASGoLine>();
                        lastLine.data.FPoint = data.BPoint;
                        data.BLine = lastLine.data;
                        lastLine.data.FLine = data;
                    }
                    if (next == null)
                    {
                        data.FPoint = new ADASMapPoint { Position = to };
                    }
                }
                return data;
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
            data = null;
            data = Line;
        }
    }
}