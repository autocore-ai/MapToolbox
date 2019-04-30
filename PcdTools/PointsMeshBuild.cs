#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Packages.UnityTools.PcdTools
{
    public static class PointsMeshBuild
    {
        public static Mesh PointsMesh(this PcdReader pcdReader)
        {
            if (pcdReader.PcdData_xyz.IsCreated)
            {
                using (var vertices = new NativeArray<Vector3>(pcdReader.PcdData_xyz.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                {
                    new Ros2Unity_xyz() { pcd = pcdReader.PcdData_xyz, vertices = vertices }.Schedule(vertices.Length, 128).Complete();
                    Mesh mesh = new Mesh()
                    {
                        indexFormat = vertices.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16,
                        vertices = vertices.ToArray()
                    };
                    mesh.SetIndices(Enumerable.Range(0, mesh.vertexCount).ToArray(), MeshTopology.Points, 0);
                    return mesh;
                }
            }
            else if (pcdReader.PcdData_xyz_rgb.IsCreated)
            {
                using (var vertices = new NativeArray<Vector3>(pcdReader.PcdData_xyz_rgb.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                {
                    using (var colors = new NativeArray<Color32>(pcdReader.PcdData_xyz_rgb.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                    {
                        new Ros2Unity_xyz_rgb() { pcd = pcdReader.PcdData_xyz_rgb, vertices = vertices, colors = colors }.Schedule(vertices.Length, 128).Complete();
                        Mesh mesh = new Mesh()
                        {
                            indexFormat = vertices.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16,
                            vertices = vertices.ToArray(),
                            colors32 = colors.ToArray()
                        };
                        mesh.SetIndices(Enumerable.Range(0, mesh.vertexCount).ToArray(), MeshTopology.Points, 0);
                        return mesh;
                    }
                }
            }
            else if (pcdReader.PcdData_xyz_intensity.IsCreated)
            {
                using (var vertices = new NativeArray<Vector3>(pcdReader.PcdData_xyz_intensity.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                {
                    using (var colors = new NativeArray<Color32>(pcdReader.PcdData_xyz_intensity.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                    {
                        new Ros2Unity_xyz_intensity() { pcd = pcdReader.PcdData_xyz_intensity, vertices = vertices, colors = colors }.Schedule(vertices.Length, 128).Complete();
                        Mesh mesh = new Mesh()
                        {
                            indexFormat = vertices.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16,
                            vertices = vertices.ToArray(),
                            colors32 = colors.ToArray()
                        };
                        mesh.SetIndices(Enumerable.Range(0, mesh.vertexCount).ToArray(), MeshTopology.Points, 0);
                        return mesh;
                    }
                }
            }
            else
            {
                return new Mesh();
            }
        }

        [BurstCompile]
        struct Ros2Unity_xyz : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<float3> pcd;
            [WriteOnly] internal NativeArray<Vector3> vertices;
            public void Execute(int index) => vertices[index] = new Vector3(-pcd[index].y, pcd[index].z, pcd[index].x);
        }

        [BurstCompile]
        struct Ros2Unity_xyz_rgb : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<float3byte4> pcd;
            [WriteOnly] internal NativeArray<Vector3> vertices;
            [WriteOnly] internal NativeArray<Color32> colors;
            public void Execute(int index)
            {
                vertices[index] = new Vector3(-pcd[index].point.y, pcd[index].point.z, pcd[index].point.x);
                colors[index] = new Color32(pcd[index].r, pcd[index].g, pcd[index].b, 255);
            }
        }

        [BurstCompile]
        struct Ros2Unity_xyz_intensity : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<float4> pcd;
            [WriteOnly] internal NativeArray<Vector3> vertices;
            [WriteOnly] internal NativeArray<Color32> colors;
            public void Execute(int index)
            {
                vertices[index] = new Vector3(-pcd[index].y, pcd[index].z, pcd[index].x);
                colors[index] = Color32.Lerp(Color.red, Color.green, pcd[index].w / 100);
            }
        }
    }
}
