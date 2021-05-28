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

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AutoCore.MapToolbox.PCL
{
    public static class HeightMeshBuild
    {
        public static TerrainData GetTerrainData(this Mesh mesh)
        {
            var ret = new TerrainData();
            ret.name = "terrain_" + mesh.name;
            ret.size = new Vector3(50, 1000, 50);
            if (mesh.vertexCount > 0)
            {
                int2 grid = new int2(65);
                int2 size = new int2(100);
                var originVertices = new NativeArray<Vector3>(mesh.vertices, Allocator.TempJob);
                var hashMap = new NativeMultiHashMap<int2, float3>(mesh.vertices.Length, Allocator.TempJob);
                new GetHashMapPoints()
                {
                    size = size,
                    grid = grid,
                    vertices = originVertices,
                    hashMap = hashMap.AsParallelWriter(),
                }.Schedule(originVertices.Length, 128).Complete();
                originVertices.Dispose();
                var heightMap = new NativeMultiHashMap<int2, float>(grid.x * grid.y, Allocator.TempJob);
                new GetHeights()
                {
                    hashMap = hashMap,
                    grid = grid,
                    heightMap = heightMap.AsParallelWriter()
                }.Schedule(grid.x * grid.y, 128).Complete();
                hashMap.Dispose();
                float[,] heights = new float[grid.x, grid.y];
                for (int x = 0; x < grid.x; x++)
                {
                    for (int y = 0; y < grid.y; y++)
                    {
                        if (heightMap.TryGetFirstValue(new int2(x, y), out float height, out var _))
                        {
                            heights[y, x] = height / 1000.0f;
                        }
                    }
                }
                ret.baseMapResolution = 64;
                ret.heightmapResolution = grid.x;
                ret.SetDetailResolution(0, 8);
                ret.SetHeights(0, 0, heights);
                heightMap.Dispose();
            }
            return ret;
        }
        [BurstCompile]
        struct GetHashMapPoints : IJobParallelFor
        {
            [ReadOnly] internal int2 size;
            [ReadOnly] internal int2 grid;
            [ReadOnly] internal NativeArray<Vector3> vertices;
            [WriteOnly] internal NativeMultiHashMap<int2, float3>.ParallelWriter hashMap;
            public void Execute(int index)
            {
                var key = (int2)math.floor(((float3)vertices[index]).xz) % size * grid / size;
                hashMap.Add(key, vertices[index]);
            }
        }
        [BurstCompile]
        struct GetHeights : IJobParallelFor
        {
            [ReadOnly] internal int2 grid;
            [ReadOnly] internal NativeMultiHashMap<int2, float3> hashMap;
            [WriteOnly] internal NativeMultiHashMap<int2, float>.ParallelWriter heightMap;
            public void Execute(int index)
            {
                int2 id = new int2(index / grid.x, index % grid.y);
                float height = float.MaxValue;
                if (hashMap.TryGetFirstValue(id, out float3 point, out NativeMultiHashMapIterator<int2> it))
                {
                    do
                    {
                        if(point.y > 200)
                        {
                            height = math.min(height, point.y);
                        }
                    } while (hashMap.TryGetNextValue(out point, ref it));
                }
                if (height == float.MaxValue)
                {
                    height = 0;
                }
                heightMap.Add(id, height);
            }
        }
    }
}