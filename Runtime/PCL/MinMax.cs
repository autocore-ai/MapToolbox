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
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AutoCore.MapToolbox.PCL
{
    public static class MinMax
    {
        public const float max_float = float.MaxValue;
        public static float2 max_float2 = new float2(max_float, max_float);
        public static float3 max_float3 = new float3(max_float2, max_float);
        public static float4 max_float4 = new float4(max_float3, max_float);

        public const float min_float = float.MinValue;
        public static float2 min_float2 = new float2(min_float, min_float);
        public static float3 min_float3 = new float3(min_float2, min_float);
        public static float4 min_float4 = new float4(min_float3, min_float);

        #region ArrayMin
        public static float ArrayMin(this NativeArray<float> array)
        {
            if (array.Length > 0)
            {
                float ret = array[0];
                for (int i = 0; i < array.Length; i++)
                {
                    ret = math.min(ret, array[i]);
                }
                return ret;
            }
            return default;
        }
        public static float4 ArrayMin(this NativeArray<float4> array)
        {
            if (array.Length > 0)
            {
                float4 ret = array[0];
                for (int i = 0; i < array.Length; i++)
                {
                    ret = math.min(ret, array[i]);
                }
                return ret;
            }
            return default;
        }
        #endregion

        #region ArrayMax
        public static float ArrayMax(this NativeArray<float> array)
        {
            if (array.Length > 0)
            {
                float ret = array[0];
                for (int i = 0; i < array.Length; i++)
                {
                    ret = math.max(ret, array[i]);
                }
                return ret;
            }
            return default;
        }
        public static float4 ArrayMax(this NativeArray<float4> array)
        {
            if (array.Length > 0)
            {
                float4 ret = array[0];
                for (int i = 0; i < array.Length; i++)
                {
                    ret = math.max(ret, array[i]);
                }
                return ret;
            }
            return default;
        }
        #endregion

        private static int MinIndicesPerJobCount(int length) => length / SystemInfo.processorCount;

        [BurstCompile]
        struct JobGetMinMax_float : IJobParallelForBatch
        {
            [ReadOnly] public NativeArray<float> target;
            [ReadOnly] public int maxCount;
            [NativeDisableContainerSafetyRestriction] public NativeArray<float> min;
            [NativeDisableContainerSafetyRestriction] public NativeArray<float> max;
            public void Execute(int startIndex, int count)
            {
                int offset = count == maxCount ? (startIndex / count) : (min.Length - 1);
                for (int i = 0; i < count; i++)
                {
                    min[offset] = math.min(min[offset], target[startIndex + i]);
                    max[offset] = math.max(max[offset], target[startIndex + i]);
                }
            }
        }

        [BurstCompile]
        struct JobGetMinMax_float4 : IJobParallelForBatch
        {
            [ReadOnly] public NativeArray<float4> target;
            [ReadOnly] public int maxCount;
            [NativeDisableContainerSafetyRestriction] public NativeArray<float4> min;
            [NativeDisableContainerSafetyRestriction] public NativeArray<float4> max;
            public void Execute(int startIndex, int count)
            {
                int offset = count == maxCount ? (startIndex / count) : (min.Length - 1);
                for (int i = 0; i < count; i++)
                {
                    min[offset] = math.min(min[offset], target[startIndex + i]);
                    max[offset] = math.max(max[offset], target[startIndex + i]);
                }
            }
        }

        public static void GetMinMax(this NativeArray<float> target, out float min, out float max)
        {
            min = max_float;
            max = min_float;
            int minIndicesPerJobCount = MinIndicesPerJobCount(target.Length);
            var minArray = new NativeArray<float>(SystemInfo.processorCount + 1, Allocator.TempJob);
            var maxArray = new NativeArray<float>(SystemInfo.processorCount + 1, Allocator.TempJob);
            minArray.FillNativeArray(min);
            maxArray.FillNativeArray(max);
            var job = new JobGetMinMax_float
            {
                target = target,
                maxCount = minIndicesPerJobCount,
                min = minArray,
                max = maxArray
            };
            job.ScheduleBatch(target.Length, minIndicesPerJobCount).Complete();
            min = minArray.ArrayMin();
            max = maxArray.ArrayMax();
            minArray.Dispose();
            maxArray.Dispose();
        }

        public static void GetMinMax(this NativeArray<float4> target, out float4 min, out float4 max)
        {
            min = max_float4;
            max = min_float4;
            int minIndicesPerJobCount = MinIndicesPerJobCount(target.Length);
            var minArray = new NativeArray<float4>(SystemInfo.processorCount + 1, Allocator.TempJob);
            var maxArray = new NativeArray<float4>(SystemInfo.processorCount + 1, Allocator.TempJob);
            minArray.FillNativeArray(min);
            maxArray.FillNativeArray(max);
            var job = new JobGetMinMax_float4
            {
                target = target,
                maxCount = minIndicesPerJobCount,
                min = minArray,
                max = maxArray
            };
            job.ScheduleBatch(target.Length, minIndicesPerJobCount).Complete();
            min = minArray.ArrayMin();
            max = maxArray.ArrayMax();
            minArray.Dispose();
            maxArray.Dispose();
        }
    }
}