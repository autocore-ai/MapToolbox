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


using AutoCore.MapToolbox.PCL;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace AutoCore.MapToolbox.Editor.PCL
{
    [ScriptedImporter(1, "pcd")]
    class PointCloudImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            using (var reader = new PointCloudReader(Path.Combine(Directory.GetCurrentDirectory(), ctx.assetPath)))
            {
                if (reader.PointXYZRGBAs.IsCreated)
                {
                    SaveMesh(ctx, reader.PointXYZRGBAs.CoordinateRosToUnity().ToUnityColor().ToMesh());
                }
                else if (reader.PointXYZIs.IsCreated)
                {
                    var colored = reader.PointXYZIs.IntensityToColor();
                    SaveMesh(ctx, colored.CoordinateRosToUnity().ToMesh());
                    colored.Dispose();
                }
            }
            AssetDatabase.Refresh();
        }
        private void SaveMesh(AssetImportContext ctx, Mesh mesh)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            DestroyImmediate(go.GetComponent<BoxCollider>());
            go.GetComponent<MeshFilter>().mesh = mesh;
            mesh.name = "points";
            go.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("MapToolbox/PointCloud");
            ctx.AddObjectToAsset(mesh.name, mesh);
            ctx.AddObjectToAsset(go.name, go);
            ctx.SetMainObject(go);
        }
    }
}