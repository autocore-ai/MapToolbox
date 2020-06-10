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
                    ConnectLoopLineRef();
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

        protected virtual void OnEnable() => ConnectLoopLineRef();

        public override void BuildData()
        {
            ConnectLoopLineRef();
            base.BuildData();
        }

        public void ConnectLoopLineRef()
        {
            var lines = GetComponentsInChildren<ADASGoLine>();
            foreach (var item in lines)
            {
                item.UpdateRef();
            }
            if (lines.Length > 1)
            {
                lines.First().bLine = lines.Last();
                lines.First().fLine = lines[1];
                lines.Last().bLine = lines[lines.Length - 2];
                lines.Last().fLine = lines.First();
                for (int i = 1; i < lines.Length - 1; i++)
                {
                    lines[i].bLine = lines[i - 1];
                    lines[i].fLine = lines[i + 1];
                }
            }
        }
        public void Init4Lines()
        {
            var p1 = new GameObject(typeof(ADASGoLine).Name);
            var p2 = new GameObject(typeof(ADASGoLine).Name);
            var p3 = new GameObject(typeof(ADASGoLine).Name);
            var p4 = new GameObject(typeof(ADASGoLine).Name);
            p1.transform.SetParent(transform);
            p2.transform.SetParent(transform);
            p3.transform.SetParent(transform);
            p4.transform.SetParent(transform);
            var l1 = p1.AddComponent<ADASGoLine>();
            var l2 = p2.AddComponent<ADASGoLine>();
            var l3 = p3.AddComponent<ADASGoLine>();
            var l4 = p4.AddComponent<ADASGoLine>();
            l1.LocalTo = l2.LocalFrom = new Vector3(1, 0, 1);
            l2.LocalTo = l3.LocalFrom = new Vector3(-1, 0, 1);
            l3.LocalTo = l4.LocalFrom = new Vector3(-1, 0, -1);
            l4.LocalTo = l1.LocalFrom = new Vector3(1, 0, -1);
            ConnectLoopLineRef();
        }
        public override void SetupRenderer()
        {
            base.SetupRenderer();
            LineRenderer.loop = true;
        }
    }
}