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

using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEditorInternal;
using UnityEngine;

namespace Packages.MapToolbox.PcdTools.Editor
{
    [ScriptedImporter(1, "pcd")]
    class PcdImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            using (var pcd = new PcdReader(Path.Combine(Directory.GetCurrentDirectory(), ctx.assetPath)))
            {
                var pointsMesh = pcd.PointsMesh();
                if (pointsMesh)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.GetComponent<MeshFilter>().mesh = pointsMesh;
                    pointsMesh.name = "points";
                    var material = new Material(Shader.Find("Particles/Standard Unlit"));
                    material.name = "material";
                    go.GetComponent<MeshRenderer>().sharedMaterial = material;
                    var heightMesh = pointsMesh.GetHeightMesh();
                    heightMesh.name = "height";
                    var goHeightMesh = new GameObject(heightMesh.name);
                    goHeightMesh.transform.parent = go.transform;
                    goHeightMesh.AddComponent<MeshFilter>().sharedMesh = heightMesh;
                    InternalEditorUtility.SetIsInspectorExpanded(goHeightMesh.GetComponent<MeshFilter>(), false);
                    InternalEditorUtility.SetIsInspectorExpanded(goHeightMesh.AddComponent<MeshCollider>(), false);
                    ctx.AddObjectToAsset(pointsMesh.name, pointsMesh);
                    ctx.AddObjectToAsset(material.name, material);
                    ctx.AddObjectToAsset(heightMesh.name, heightMesh);
                    ctx.AddObjectToAsset(go.name, go);
                    ctx.SetMainObject(go);
                }
            }
        }
    }
}