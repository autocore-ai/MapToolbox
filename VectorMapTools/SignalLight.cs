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
using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools
{
    [ExecuteInEditMode]
    public class SignalLight : MonoBehaviour
    {
        public enum LightColor
        {
            Red,
            Yellow,
            Green
        }
        public LightColor lightColor = LightColor.Green;
        public static List<SignalLight> List { get; set; } = new List<SignalLight>();
        private void Start() => List.Add(this);
        private void OnDrawGizmos()
        {
            if (lightColor == LightColor.Green)
            {
                Gizmos.color = Color.green;
            }
            else if (lightColor == LightColor.Yellow)
            {
                Gizmos.color = Color.yellow;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, 30, 10, 0.2f, 1);
        }
        private void OnDestroy() => List.Remove(this);
    }
}