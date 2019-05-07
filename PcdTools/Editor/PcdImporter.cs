#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEditorInternal;
using UnityEngine;

namespace Packages.UnityTools.PcdTools.Editor
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
