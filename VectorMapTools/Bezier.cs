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
    public class Bezier : LRMapElement
    {
        [HideInInspector]
        public Vector3 startPosition;
        [HideInInspector]
        public Vector3 endPosition;
        [HideInInspector]
        public Vector3 startTangent;
        [HideInInspector] 
        public Vector3 endTangent;
        protected Vector3 startPositionMove;
        protected Vector3 endPositionMove;
        public virtual Vector3 StartPosition
        {
            set
            {
                startPositionMove = value - startPosition;
                startPosition = value;
                startTangent += startPositionMove;
            }
        }
        public virtual Vector3 EndPosition
        {
            set
            {
                endPositionMove = value - endPosition;
                endPosition = value;
                endTangent += endPositionMove;
            }
        }
        public override Vector3? Pivot
        {
            set
            {
                base.Pivot = value;
                startPosition += value ?? Vector3.zero;
                endPosition += value ?? Vector3.zero;
                startTangent += value ?? Vector3.zero;
                endTangent += value ?? Vector3.zero;
            }
        }
        public virtual Vector3[] Points
        {
            set
            {
                LineRenderer.positionCount = value.Length;
                LineRenderer.SetPositions(value);
            }
        }
    }
}