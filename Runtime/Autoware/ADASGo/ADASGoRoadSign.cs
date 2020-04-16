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
    class ADASGoRoadSign : MonoBehaviour, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public ADASGoPole pole;
        public ADASGoLane linkLane;
        public ADASMapRoadSign.Type roadSignType;
        public CollectionLane CollectionLane { get; set; }
        public CollectionPole CollectionPole { get; set; }
        public CollectionRoadSign CollectionRoadSign { get; set; }
        Mesh Mesh { get; set; }
        ADASMapRoadSign data;
        public ADASMapRoadSign RoadSign
        {
            set
            {
                data = value;
                if (data != null)
                {
                    CollectionRoadSign?.Add(data.ID, this);
                    name = data.ID.ToString();
                    roadSignType = data.RoadSignType;
                    transform.position = data.Vector.Point.Position;
                    transform.rotation = data.Vector.Rotation;
                    Mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
                    if (CollectionPole != null && data.Pole != null)
                    {
                        pole = CollectionPole[data.Pole.ID];
                    }
                    if (CollectionLane != null && data.LinkLane != null)
                    {
                        linkLane = CollectionLane[data.LinkLane.ID];
                    }
                }
            }
            get
            {
                if (data == null)
                {
                    data = new ADASMapRoadSign
                    {

                    };
                }
                return data;
            }
        }
        private void OnDrawGizmos()
        {
            switch (roadSignType)
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