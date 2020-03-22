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


using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace AutoCore.MapToolbox.PCL
{
    public static class Externs
    {
        public static float IntensityPrecentMin
        {
            get
            {
                return PlayerPrefs.GetFloat(Const.IntensityPrecentMin, 0);
            }
            set
            {
                PlayerPrefs.SetFloat(Const.IntensityPrecentMin, value);
            }
        }
        public static float IntensityPrecentMax
        {
            get
            {
                return PlayerPrefs.GetFloat(Const.IntensityPrecentMax, 30);
            }
            set
            {
                PlayerPrefs.SetFloat(Const.IntensityPrecentMax, value);
            }
        }
        [BurstCompile]
        struct JobPclCoordinateRosToUnity : IJobParallelFor
        {
            internal NativeArray<PointXYZRGBA> points;
            public void Execute(int index)
            {
                var temp = points[index];
                points[index] = new PointXYZRGBA { xyz = new Vector3(temp.xyz.x, temp.xyz.z, temp.xyz.y), bgra = temp.bgra };
            }
        }
        public static NativeArray<PointXYZRGBA> CoordinateRosToUnity(this NativeArray<PointXYZRGBA> points)
        {
            new JobPclCoordinateRosToUnity { points = points }.Schedule(points.Length, 1024).Complete();
            return points;
        }
        [BurstCompile]
        struct JobPclToUnityColor : IJobParallelFor
        {
            internal NativeArray<PointXYZRGBA> points;
            public void Execute(int index)
            {
                var temp = points[index];
                points[index] = new PointXYZRGBA { xyz = temp.xyz, bgra = new Color32(temp.bgra.b, temp.bgra.g, temp.bgra.r, temp.bgra.a) };
            }
        }
        public static NativeArray<PointXYZRGBA> ToUnityColor(this NativeArray<PointXYZRGBA> points)
        {
            new JobPclToUnityColor { points = points }.Schedule(points.Length, 1024).Complete();
            return points;
        }
        [BurstCompile]
        struct JobIntensityToColor : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<PointXYZI> points_xyzi;
            [WriteOnly] internal NativeArray<PointXYZRGBA> points_xyzrgba;
            [ReadOnly] internal float intensity_min;
            [ReadOnly] internal float intensity_max;
            public void Execute(int index)
            {
                int3 rgb = int3.zero;
                var value = Mathf.InverseLerp(intensity_min, intensity_max, points_xyzi[index].intensity) * 10;
                if (value <= 1.0f)
                {
                    // black -> purple
                    rgb.xz = (int2)math.round(new float2(value * 120, value * 200));
                }
                else if (value <= 2.0f)
                {
                    // purple -> blue
                    rgb.xz = new int2(120, 200) + new int2(-1, 1) * (int2)math.round(new float2((value - 1.0f) * 120, (value - 1.0f) * 55));
                }
                else if (value <= 3.0f)
                {
                    // blue -> turquoise
                    rgb.yz = new int2(0, 255) + new int2(1, -1) * (int2)math.round(new float2((value - 2.0f) * 200, (value - 2.0f) * 55));
                }
                else if (value <= 4.0f)
                {
                    // turquoise -> green
                    rgb.yz = new int2(200, 200) + new int2(1, -1) * (int2)math.round(new float2((value - 3.0f) * 55, (value - 3.0f) * 200));
                }
                else if (value <= 5.0f)
                {
                    // green -> greyish green
                    rgb.xy = new int2(0, 255) + new int2(1, -1) * (int2)math.round(new float2((value - 4.0f) * 120, (value - 4.0f) * 100));
                }
                else if (value <= 6.0f)
                {
                    // greyish green -> red
                    rgb.xyz = new int3(100, 120, 120) + new int3(1, -1, -1) * (int3)math.round(new float3((value - 5.0f) * 155, (value - 5.0f) * 120, (value - 5.0f) * 120));
                }
                else if (value <= 7.0f)
                {
                    // red -> yellow
                    rgb.xy = new int2(255, (int)math.round((value - 6.0f) * 255));
                }
                else
                {
                    // yellow -> white
                    rgb.xyz = new int3(255, 255, (int)math.round((value - 7.0f) * 255.0f / 3.0f));
                }
                points_xyzrgba[index] = new PointXYZRGBA
                {
                    xyz = points_xyzi[index].xyz,
                    bgra = new Color32((byte)rgb.x, (byte)rgb.y, (byte)rgb.z, byte.MaxValue)
                };
            }
        }
        public static NativeArray<PointXYZRGBA> IntensityToColor(this NativeArray<PointXYZI> points)
        {
            points.Reinterpret<float4>().GetMinMax(out float4 min, out float4 max);
            var ret = new NativeArray<PointXYZRGBA>(points.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            new JobIntensityToColor
            {
                points_xyzi = points,
                points_xyzrgba = ret,
                intensity_min = min.w + (max.w - min.w) * IntensityPrecentMin / 100,
                intensity_max = max.w - (max.w - min.w) * (1 - IntensityPrecentMax / 100)
            }.Schedule(points.Length, 1024).Complete();
            return ret;
        }
        [BurstCompile]
        struct GetMeshData : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<PointXYZRGBA> points;
            [WriteOnly] internal NativeArray<Vector3> vertices;
            [WriteOnly] internal NativeArray<Color32> colors32;
            [WriteOnly] internal NativeArray<int> indices;
            public void Execute(int index)
            {
                vertices[index] = points[index].xyz;
                colors32[index] = points[index].bgra;
                indices[index] = index;
            }
        }
        public static Mesh ToMesh(this NativeArray<PointXYZRGBA> points)
        {
            NativeArray<Vector3> vertices = new NativeArray<Vector3>(points.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<Color32> colors32 = new NativeArray<Color32>(points.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            NativeArray<int> indices = new NativeArray<int>(points.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            new GetMeshData
            {
                points = points,
                vertices = vertices,
                colors32 = colors32,
                indices = indices
            }.Schedule(points.Length, 2048).Complete();
            Mesh mesh = new Mesh()
            {
                indexFormat = points.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16,
                vertices = vertices.ToArray(),
                colors32 = colors32.ToArray()
            };
            mesh.SetIndices(indices.ToArray(), MeshTopology.Points, 0);
            vertices.Dispose();
            colors32.Dispose();
            indices.Dispose();
            return mesh;
        }
    }
}