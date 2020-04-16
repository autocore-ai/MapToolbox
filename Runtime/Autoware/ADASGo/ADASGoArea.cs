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
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoArea : ADASGoSlicesLine
    {
        public ADASMapArea Area
        {
            set
            {
                if (value != null)
                {
                    Lines = value.SLine.GetRangeToEnd(value.ELine);
                    foreach (var item in Lines)
                    {
                        var go = new GameObject(typeof(ADASGoLine).Name);
                        go.transform.SetParent(transform);
                        var line = go.AddComponent<ADASGoLine>();
                        line.Line = item;
                    }
                    var lines = GetComponentsInChildren<ADASGoLine>();
                    lines.Last().fLine = lines.First();
                    lines.First().bLine = lines.Last();
                    LineRenderer.loop = true;
                }
            }
            get
            {
                Lines = GetComponentsInChildren<ADASGoLine>().Select(_ => _.Line);
                if (Lines.Count() > 0)
                {
                    return new ADASMapArea
                    {
                        SLine = Lines.First(),
                        ELine = Lines.Last()
                    };
                }
                return null;
            }
        }
    }
}