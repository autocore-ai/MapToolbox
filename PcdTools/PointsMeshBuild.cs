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

using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Packages.MapToolbox.PcdTools
{
    public static class PointsMeshBuild
    {
        const float max_intensity = 100;
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
            else if (pcdReader.PcdData_xyz_intensity_ring.IsCreated)
            {
                using (var vertices = new NativeArray<Vector3>(pcdReader.PcdData_xyz_intensity_ring.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                {
                    using (var colors = new NativeArray<Color32>(pcdReader.PcdData_xyz_intensity_ring.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory))
                    {
                        new Ros2Unity_xyz_intensity_ring() { pcd = pcdReader.PcdData_xyz_intensity_ring, vertices = vertices, colors = colors }.Schedule(vertices.Length, 128).Complete();
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
                colors[index] = Color32.Lerp(Color.red, Color.green, pcd[index].w / max_intensity);
            }
        }
        [BurstCompile]
        struct Ros2Unity_xyz_intensity_ring : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<xyz_intensity_ring> pcd;
            [WriteOnly] internal NativeArray<Vector3> vertices;
            [WriteOnly] internal NativeArray<Color32> colors;
            public void Execute(int index)
            {
                vertices[index] = new Vector3(-pcd[index].point.y, pcd[index].point.z, pcd[index].point.x);
                colors[index] = Color32.Lerp(Color.red, Color.green, pcd[index].intensity / max_intensity);
            }
        }
    }
}