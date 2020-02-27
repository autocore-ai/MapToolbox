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

namespace AutoCore.MapToolbox.Autoware
{
    class RoadSign : MonoBehaviour
    {
        public ADASMapRoadSign VectorMapRoadSign { get; set; }
        Mesh Mesh { get; set; }
        internal void SetData(ADASMapRoadSign item)
        {
            VectorMapRoadSign = item;
            name = item.ID.ToString();
            transform.position = item.Vector.Point.Position;
            transform.rotation = item.Vector.Rotation;
            Mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
        }
        private void OnDrawGizmos()
        {
            switch (VectorMapRoadSign.RoadSignType)
            {
                case ADASMapRoadSign.Type.TYPE_NULL:
                    Gizmos.color = Color.yellow;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawMesh(Mesh, Vector3.zero);
                    break;
                case ADASMapRoadSign.Type.TYPE_STOP:
                    Gizmos.color = Color.red;
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawMesh(Mesh, Vector3.zero);
                    break;
                default:
                    break;
            }
        }
    }
}