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

namespace Packages.MapToolbox.VectorMapTools
{
    [RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
    public class LRMapElement : MonoBehaviour
    {
        public LineRenderer LineRenderer { get; private set; }
        [HideInInspector] public float width = 1;
        public float WidthMin { get; set; } = 0.1f;
        public float WidthMax { get; set; } = 1;
        public virtual Vector3? Pivot { get; set; }
        protected virtual void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
            SetLineRendererWidth(width);
        }
        public void SetLineRendererWidth(float width)
        {
            if (LineRenderer)
            {
                LineRenderer.startWidth = LineRenderer.endWidth = width;
            }
        }
    }
}