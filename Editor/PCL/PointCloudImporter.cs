#region License
/******************************************************************************
* Copyright 2018-2021 The AutoCore Authors. All Rights Reserved.
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
using Unity.Mathematics;
using UnityEditor.AssetImporters;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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
                    SaveMeshes(ctx, reader.PointXYZRGBAs.CoordinateRosToUnity().ToUnityColor().PointsCell(new int2(100)).ToMeshes());
                }
                else if (reader.PointXYZIs.IsCreated)
                {
                    var colored = reader.PointXYZIs.IntensityToColor();
                    SaveMeshes(ctx, colored.CoordinateRosToUnity().PointsCell(new int2(100)).ToMeshes());
                    colored.Dispose();
                }
            }
            AssetDatabase.Refresh();
            Tools.lockedLayers = 1 << 3;
        }
        private void SaveMeshes(AssetImportContext ctx, List<(Vector3, Mesh)> meshes)
        {
            var root = new GameObject();
            root.layer = 3;
            for(int i = 0; i < meshes.Count; i ++)
            {
                var mesh = meshes[i];
                mesh.Item2.name = "points" + i;
                var chunk = new GameObject();
                chunk.layer = 3;
                chunk.name = i.ToString();
                chunk.transform.parent = root.transform;
                chunk.transform.localPosition = mesh.Item1;
                chunk.AddComponent<MeshFilter>().mesh = mesh.Item2;
                chunk.AddComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("MapToolbox/PointCloud");
                var terrainData = mesh.Item2.GetTerrainData();
                var terrain = chunk.AddComponent<Terrain>();
                terrain.materialTemplate = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Terrain-Standard.mat");
                terrain.terrainData = terrainData;
                chunk.AddComponent<TerrainCollider>().terrainData = terrainData;
                ctx.AddObjectToAsset(chunk.name, chunk);
                var path = Path.Combine(Path.GetDirectoryName(ctx.assetPath), Path.GetFileNameWithoutExtension(ctx.assetPath));
                Directory.CreateDirectory(path);
                AssetDatabase.CreateAsset(terrainData, path + "/" + terrainData.name + ".asset");
                AssetDatabase.CreateAsset(mesh.Item2, path + "/" + mesh.Item2.name + ".asset");
            }
            AssetDatabase.SaveAssets();
            ctx.AddObjectToAsset(root.name, root);
            ctx.SetMainObject(root);
        }
    }
}