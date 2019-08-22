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

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Packages.MapToolbox.PcdTools
{
    public static class HeightMeshBuild
    {
        public static Mesh GetHeightMesh(this Mesh pointsMesh)
        {
            if (pointsMesh.vertexCount > 0)
            {
                int2 minInt = (int2)math.floor(((float3)(pointsMesh.bounds.min)).xz);
                int2 maxInt = (int2)math.ceil(((float3)(pointsMesh.bounds.max)).xz);
                int2 sizeInt = maxInt - minInt;
                var originVertices = new NativeArray<Vector3>(pointsMesh.vertices, Allocator.TempJob);
                var hashMap = new NativeMultiHashMap<int, int>(pointsMesh.vertices.Length, Allocator.TempJob);
                var minHeightIndex = new NativeArray<int>(originVertices.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                new GetHashMap() { min = minInt, size = sizeInt, vertices = originVertices, hashMap = hashMap.AsParallelWriter(), minHeightIndex = minHeightIndex }.Schedule(originVertices.Length, 128).Complete();
                new GetMinHeightIndex() { vertices = originVertices, minHeightIndex = minHeightIndex }.Schedule(hashMap, 128).Complete();
                hashMap.Dispose();
                var list = new NativeList<int>(originVertices.Length, Allocator.TempJob);
                new FilterMinHeightPoints() { minHeightIndex = minHeightIndex }.ScheduleAppend(list, originVertices.Length, 128).Complete();
                var minHeightPoints = new NativeArray<Vector3>(list.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                new GetMinHeightPoints() { vertices = originVertices, list = list, minHeightIndex = minHeightIndex, minHeightPoints = minHeightPoints }.Schedule(minHeightPoints.Length, 128).Complete();
                minHeightIndex.Dispose();
                list.Dispose();
                originVertices.Dispose();
                var flatMeshVertices = new NativeArray<Vector3>((sizeInt.x + 1) * (sizeInt.y + 1), Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                new InitHeightMeshVertices() { min = minInt, size = sizeInt, flatMeshVertices = flatMeshVertices }.Schedule(flatMeshVertices.Length, 128).Complete();
                var indicesHeightMap = new NativeHashMap<int, float>(minHeightPoints.Length, Allocator.TempJob);
                new InsertIndicesHeightMap() { min = minInt, size = sizeInt, indicesHeightMap = indicesHeightMap.AsParallelWriter(), minHeightPoints = minHeightPoints }.Schedule(indicesHeightMap.Capacity, 128).Complete();
                minHeightPoints.Dispose();
                new AddMinHeightPointsOffset() { flatMeshVertices = flatMeshVertices, indicesHeightMap = indicesHeightMap }.Schedule(flatMeshVertices.Length, 128).Complete();
                indicesHeightMap.Dispose();
                var vertices = new NativeArray<Vector3>((sizeInt.x + 1) * (sizeInt.y + 1), Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                NativeArray<Vector3>.Copy(flatMeshVertices, vertices);
                new SetHeightOfEdgePoints() { size = sizeInt, flatMeshVertices = flatMeshVertices, heightMeshVertices_write = vertices }.Schedule(flatMeshVertices.Length, 128).Complete();
                flatMeshVertices.Dispose();
                var trianglesInt6 = new NativeArray<int6>(sizeInt.x * sizeInt.y, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                new GetTriangles() { size = sizeInt, triangles = trianglesInt6 }.Schedule(trianglesInt6.Length, 128).Complete();
                var triangles = new NativeArray<int>(trianglesInt6.Length * 6, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                unsafe
                {
                    UnsafeUtility.MemCpy(triangles.GetUnsafePtr(), trianglesInt6.GetUnsafeReadOnlyPtr(), triangles.Length * sizeof(int));
                }
                trianglesInt6.Dispose();
                Mesh mesh = new Mesh()
                {
                    indexFormat = vertices.Length > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16,
                    vertices = vertices.ToArray(),
                    triangles = triangles.ToArray()
                };
                vertices.Dispose();
                triangles.Dispose();
                return mesh;
            }
            else
            {
                return new Mesh();
            }
        }
        [BurstCompile]
        struct GetHashMap : IJobParallelFor
        {
            [ReadOnly] internal int2 min;
            [ReadOnly] internal int2 size;
            [ReadOnly] internal NativeArray<Vector3> vertices;
            [WriteOnly] internal NativeMultiHashMap<int, int>.ParallelWriter hashMap;
            [WriteOnly] internal NativeArray<int> minHeightIndex;
            public void Execute(int index)
            {
                minHeightIndex[index] = -1;
                var offset = (int2)math.floor(((float3)vertices[index]).xz) - min;
                var hash = offset.y * size.x + offset.x;
                hashMap.Add(hash, index);
            }
        }
        [BurstCompile]
        struct GetMinHeightIndex : IJobNativeMultiHashMapMergedSharedKeyIndices
        {
            [ReadOnly] internal NativeArray<Vector3> vertices;
            internal NativeArray<int> minHeightIndex;
            public void ExecuteFirst(int index) => minHeightIndex[index] = index;
            public void ExecuteNext(int firstIndex, int index)
            {
                if (vertices[index].y < vertices[minHeightIndex[firstIndex]].y)
                {
                    minHeightIndex[firstIndex] = index;
                }
            }
        }
        [BurstCompile]
        struct FilterMinHeightPoints : IJobParallelForFilter
        {
            [ReadOnly] internal NativeArray<int> minHeightIndex;
            public bool Execute(int index) => minHeightIndex[index] >= 0;
        }
        [BurstCompile]
        struct GetMinHeightPoints : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<Vector3> vertices;
            [ReadOnly] internal NativeList<int> list;
            [ReadOnly] internal NativeArray<int> minHeightIndex;
            [WriteOnly] internal NativeArray<Vector3> minHeightPoints;
            public void Execute(int index) => minHeightPoints[index] = vertices[minHeightIndex[list[index]]];
        }
        [BurstCompile]
        struct InitHeightMeshVertices : IJobParallelFor
        {
            [ReadOnly] internal int2 min;
            [ReadOnly] internal int2 size;
            [WriteOnly] internal NativeArray<Vector3> flatMeshVertices;
            public void Execute(int index)
            {
                int2 indices = min + new int2(index % (size.x + 1), (index / (size.x + 1)));
                flatMeshVertices[index] = new Vector3(indices.x, 0, indices.y);
            }
        }
        [BurstCompile]
        struct InsertIndicesHeightMap : IJobParallelFor
        {
            [ReadOnly] internal int2 min;
            [ReadOnly] internal int2 size;
            [WriteOnly] internal NativeHashMap<int, float>.ParallelWriter indicesHeightMap;
            [ReadOnly] internal NativeArray<Vector3> minHeightPoints;
            public void Execute(int index)
            {
                var offset = (int2)math.floor(((float3)minHeightPoints[index]).xz) - min;
                indicesHeightMap.TryAdd(offset.y * (size.x + 1) + offset.x, minHeightPoints[index].y);
            }
        }
        [BurstCompile]
        struct AddMinHeightPointsOffset : IJobParallelFor
        {
            [ReadOnly] internal NativeHashMap<int, float> indicesHeightMap;
            internal NativeArray<Vector3> flatMeshVertices;
            public void Execute(int index)
            {
                if (indicesHeightMap.TryGetValue(index, out float height))
                {
                    flatMeshVertices[index] = new Vector3(flatMeshVertices[index].x, height, flatMeshVertices[index].z);
                }
            }
        }
        [BurstCompile]
        struct SetHeightOfEdgePoints : IJobParallelFor
        {
            [ReadOnly] internal int2 size;
            [ReadOnly] internal NativeArray<Vector3> flatMeshVertices;
            [WriteOnly] internal NativeArray<Vector3> heightMeshVertices_write;
            public void Execute(int index)
            {
                if (index % (size.x + 1) == size.x)
                {
                    heightMeshVertices_write[index] = new Vector3(flatMeshVertices[index].x, flatMeshVertices[index - 1].y, flatMeshVertices[index].z);
                }
                if (index / (size.x + 1) == size.y)
                {
                    heightMeshVertices_write[index] = new Vector3(flatMeshVertices[index].x, flatMeshVertices[index - size.x - 1].y, flatMeshVertices[index].z);
                }
            }
        }
        [BurstCompile]
        struct GetTriangles : IJobParallelFor
        {
            [ReadOnly] internal int2 size;
            [WriteOnly] internal NativeArray<int6> triangles;
            public void Execute(int index)
            {
                int2 pointOffset = new int2(index % size.x, index / size.x);
                int pointIndex = pointOffset.y * (size.x + 1) + pointOffset.x;
                triangles[index] = new int6(pointIndex + 1, pointIndex, pointIndex + size.x + 1, pointIndex + size.x + 1, pointIndex + size.x + 2, pointIndex + 1);
            }
        }
    }
    struct int6
    {
        public int3 a;
        public int3 b;
        public int6(int ax, int ay, int az, int bx, int by, int bz)
        {
            a = new int3(ax, ay, az);
            b = new int3(bx, by, bz);
        }
        public int6(int3 a, int3 b)
        {
            this.a = a;
            this.b = b;
        }
        public static readonly int6 zero = new int6();
    }
}