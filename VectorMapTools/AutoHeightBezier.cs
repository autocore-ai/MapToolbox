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
    public class AutoHeightBezier : Bezier
    {
        readonly float maxHeight = 500;
        public override Vector3 StartPosition { set => base.StartPosition = SetHeight(value, maxHeight); }
        public override Vector3 EndPosition { set => base.EndPosition = SetHeight(value, maxHeight); }
        public override Vector3[] Points
        {
            set
            {
                for (int i = 0; i < value.Length; i++)
                {
                    value[i] = SetHeight(value[i], maxHeight);
                }
                base.Points = value;
            }
        }
        private Vector3 SetHeight(Vector3 point, float maxHeight)
        {
            if (Physics.Raycast(new Ray(new Vector3(point.x, maxHeight, point.z), Vector3.down), out RaycastHit hit, maxHeight * 2))
            {
                return hit.point;
            }
            else
            {
                return new Vector3(point.x, 0, point.z);
            }
        }
    }
}