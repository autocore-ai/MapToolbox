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
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    class ADASGoPole : MonoBehaviour, IADASMapGameObject
    {
        public GameObject GameObject => gameObject;
        public MonoBehaviour MonoBehaviour => this;
        public ADASMapPole Pole { get; set; }
        public CollectionADASPole CollectionPole { get; set; }
        MeshFilter MeshFilter { get; set; }
        MeshRenderer MeshRenderer { get; set; }
        public void Csv2Go()
        {
            CollectionPole?.Add(Pole.ID, this);
            transform.position = Pole.Vector.Point.Position + Vector3.up * Pole.Length / 2;
            transform.localScale = new Vector3(Pole.Dim, Pole.Length, Pole.Dim) / 2;
            name = Pole.ID.ToString();
            MeshFilter = GetComponent<MeshFilter>();
            MeshFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
            MeshRenderer = GetComponent<MeshRenderer>();
#if UNITY_EDITOR
            MeshRenderer.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
#endif
        }
    }
}