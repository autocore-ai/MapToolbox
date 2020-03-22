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


using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    class ADASGoSlicesWhiteLine : ADASGoSlicesLine
    {
        IEnumerable<ADASMapWhiteLine> data;
        float startWidth = 0.2f;
        float endWidth = 0.2f;
        Color startColor = Color.white;
        Color endColor = Color.white;
        public IEnumerable<ADASMapWhiteLine> WhiteLines
        {
            set
            {
                data = value;
                Lines = data.Select(_ => _.Line);
                name = $"{data.First().ID}-{data.Last().ID}";
                startWidth = data.First().Width;
                endWidth = data.Last().Width;
                startColor = data.First().COLOR == ADASMapWhiteLine.Color.White ? Color.white : Color.yellow;
                endColor = data.Last().COLOR == ADASMapWhiteLine.Color.White ? Color.white : Color.yellow;
                foreach (var item in data)
                {
                    var go = new GameObject(typeof(ADASGoWhiteLine).Name);
                    go.transform.SetParent(transform);
                    go.transform.position = item.Line.FPoint.Position;
                    var whiteline = go.AddComponent<ADASGoWhiteLine>();
                    whiteline.WhiteLine = item;
                }
                SetupRenderer();
            }
        }
        public override void BuildData()
        {
            foreach (var item in GetComponentsInChildren<ADASGoWhiteLine>())
            {
                item.UpdateRef();
                item.BuildData();
            }
        }
        internal void OnEnableEditor()
        {
            var children = GetComponentsInChildren<ADASGoWhiteLine>();
            if (children.Length > 0)
            {
                LineRenderer.startWidth = children.First().width;
                LineRenderer.endWidth = children.Last().width;
                LineRenderer.startColor = children.First().color == ADASMapWhiteLine.Color.White ? Color.white : Color.yellow;
                LineRenderer.endColor = children.Last().color == ADASMapWhiteLine.Color.White ? Color.white : Color.yellow;
            }
        }
        public void SetupRenderer()
        {
            LineRenderer.startWidth = startWidth;
            LineRenderer.endWidth = endWidth;
            LineRenderer.startColor = startColor;
            LineRenderer.endColor = endColor;
            LineRenderer.useWorldSpace = false;
#if UNITY_EDITOR
            LineRenderer.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
#endif
        }
    }
}