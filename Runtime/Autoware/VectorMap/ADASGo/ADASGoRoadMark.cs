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
    class ADASGoRoadMark : ADASGoArea
    {
        public ADASMapRoadMark.Type type = ADASMapRoadMark.Type.MARK;
        ADASMapRoadMark roadMark;
        public ADASMapRoadMark RoadMark
        {
            set
            {
                roadMark = value;
                if (roadMark != null)
                {
                    Area = roadMark.Area;
                    name = roadMark.ID.ToString();
                    type = roadMark.RoadMarkType;
                    SetupRenderer();
                }
            }
            get
            {
                if (roadMark == null)
                {
                    roadMark = new ADASMapRoadMark
                    {
                        Area = Area,
                        RoadMarkType = type
                    };
                }
                return roadMark;
            }
        }
        public override void BuildData()
        {
            Area = null;
            RoadMark = null;
            roadMark = RoadMark;
        }
        public override void SetupRenderer()
        {
            base.SetupRenderer();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.1f;
            LineRenderer.startColor = LineRenderer.endColor = Color.white;
#if UNITY_EDITOR
            LineRenderer.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
#endif
        }
    }
}