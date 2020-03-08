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
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoSlicesCurb : ADASGoSlicesLine
    {
        public IEnumerable<ADASMapCurb> VectorMapCurbs
        {
            set
            {
                Lines = value.Select(_ => _.Line);
                foreach (var item in value)
                {
                    var go = new GameObject(typeof(ADASMapCurb).Name);
                    go.transform.SetParent(transform);
                    go.transform.position = item.Line.FPoint.Position;
                    var curb = go.AddComponent<ADASGoCurb>();
                    curb.VectorMapCurb = item;
                }
                name = $"{value.First().ID}-{value.Last().ID}";
                LineRenderer.startWidth = value.First().Width;
                LineRenderer.endWidth = value.Last().Width;
                LineRenderer.startColor = LineRenderer.endColor = Color.cyan;
                LineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }
}