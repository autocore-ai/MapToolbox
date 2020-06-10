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
    class ADASGoCrossWalk : ADASGoArea
    {
        CollectionCrossWalk collectionCrossWalk;
        public CollectionCrossWalk CollectionCrossWalk
        {
            set => collectionCrossWalk = value;
            get
            {
                if (collectionCrossWalk == null)
                {
                    collectionCrossWalk = GetComponentInParent<AutowareADASMap>().GetComponentInChildren<CollectionCrossWalk>();
                }
                return collectionCrossWalk;
            }
        }
        public ADASGoCrossWalk border;
        public ADASMapCrossWalk.Type type;
        ADASMapCrossWalk crossWalk;
        public ADASMapCrossWalk CrossWalk
        {
            set
            {
                crossWalk = value;
                if (crossWalk != null)
                {
                    Area = crossWalk.Area;
                    name = crossWalk.ID.ToString();
                    type = crossWalk.CrossWalkType;
                    SetupRenderer();
                    CollectionCrossWalk.Add(crossWalk.ID, this);
                }
            }
            get
            {
                if (crossWalk == null)
                {
                    crossWalk = new ADASMapCrossWalk
                    {
                        Area = Area,
                        CrossWalkType = type
                    };
                }
                return crossWalk;
            }
        }

        public override void BuildData()
        {
            base.BuildData();
            Area = null;
            CrossWalk = null;
            crossWalk = CrossWalk;
        }
        internal void BuildDataRef()
        {
            if (border)
            {
                crossWalk.Border = border.crossWalk;
            }
        }
        internal void UpdateBorder()
        {
            if (crossWalk != null)
            {
                CollectionCrossWalk.TryGetValue(crossWalk.BdID, out border);
            }
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