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


using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace AutoCore.MapToolbox.PCL
{
    public unsafe class PointCloudReader : IDisposable
    {
        #region native funtion
        [DllImport(Const.unity_pcl_plugin)]
        static extern int load([MarshalAs(UnmanagedType.LPStr)]string file);
        [DllImport(Const.unity_pcl_plugin)]
        static extern uint height();
        [DllImport(Const.unity_pcl_plugin)]
        static extern uint width();
        [DllImport(Const.unity_pcl_plugin)]
        static extern byte is_bigendian();
        [DllImport(Const.unity_pcl_plugin)]
        static extern uint point_step();
        [DllImport(Const.unity_pcl_plugin)]
        static extern uint row_step();
        [DllImport(Const.unity_pcl_plugin)]
        static extern byte is_dense();
        [DllImport(Const.unity_pcl_plugin)]
        static extern IntPtr data_blob();
        [DllImport(Const.unity_pcl_plugin)]
        static extern ulong size_blob();
        [DllImport(Const.unity_pcl_plugin)]
        static extern void clear();
        [DllImport(Const.unity_pcl_plugin)]
        static extern bool contains_field([MarshalAs(UnmanagedType.LPStr)]string name);
        [DllImport(Const.unity_pcl_plugin)]
        static extern bool contains_xyz();
        [DllImport(Const.unity_pcl_plugin)]
        static extern bool contains_i();
        [DllImport(Const.unity_pcl_plugin)]
        static extern bool contains_rgb();
        [DllImport(Const.unity_pcl_plugin)]
        static extern bool contains_rgba();
        [DllImport(Const.unity_pcl_plugin)]
        static extern ulong load_as_xyzrgba(out IntPtr data);
        [DllImport(Const.unity_pcl_plugin)]
        static extern ulong load_as_xyzi(out IntPtr data);
        [DllImport(Const.unity_pcl_plugin)]
        static extern ulong load_as_xyz(out IntPtr data);
        #endregion
        public NativeArray<PointXYZRGBA> PointXYZRGBAs { get; set; }
        public NativeArray<PointXYZI> PointXYZIs { get; set; }
        bool LoadPointCloudFile(string file)
        {
            var ret = load(file);
            if (ret == 0)
            {
                if (is_bigendian() == 0)
                {
                    if (is_dense() == 1)
                    {
                        ulong size = size_blob();
                        if (size > 0)
                        {
                            if (size > int.MaxValue)
                            {
                                Debug.LogError("load failed as size_blob > int.MaxValue");
                            }
                            else
                            {
                                if (contains_xyz())
                                {
                                    if (contains_rgb() || contains_rgba())
                                    {
                                        size = load_as_xyzrgba(out IntPtr data_xyzrgb);
                                        if (size > (ulong)(int.MaxValue / sizeof(PointXYZRGBA)))
                                        {
                                            Debug.LogError($"load failed as breached max points amount {int.MaxValue / sizeof(PointXYZRGBA)} , this file have {size} points ‬!");
                                        }
                                        else if (size > (ulong)(int.MaxValue / sizeof(PCLPointXYZRGBA)))
                                        {
                                            var origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZRGBA>(data_xyzrgb.ToPointer(), int.MaxValue / sizeof(PCLPointXYZRGBA), Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp_1 = new NativeArray<PCLPointXYZRGBA>(origin, Allocator.TempJob);
                                            int remain = (int)size - temp_1.Length;
                                            origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZRGBA>(IntPtr.Add(data_xyzrgb, remain * 32).ToPointer(), remain, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp_2 = new NativeArray<PCLPointXYZRGBA>(origin, Allocator.TempJob);
                                            clear();
                                            var densed_1 = Dense(temp_1);
                                            temp_1.Dispose();
                                            var densed_2 = Dense(temp_2);
                                            temp_2.Dispose();
                                            PointXYZRGBAs = new NativeArray<PointXYZRGBA>((int)size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                                            NativeArray<PointXYZRGBA>.Copy(densed_1, PointXYZRGBAs, densed_1.Length);
                                            NativeArray<PointXYZRGBA>.Copy(densed_2, 0, PointXYZRGBAs, densed_1.Length, densed_2.Length);
                                            densed_1.Dispose();
                                            densed_2.Dispose();
                                            return true;
                                        }
                                        else if (size > 0)
                                        {
                                            var origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZRGBA>(data_xyzrgb.ToPointer(), (int)size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp = new NativeArray<PCLPointXYZRGBA>(origin, Allocator.TempJob);
                                            clear();
                                            PointXYZRGBAs = Dense(temp);
                                            temp.Dispose();
                                            return true;
                                        }
                                        else
                                        {
                                            Debug.LogError("load failed as points count zero !");
                                        }
                                    }
                                    else if (contains_i())
                                    {
                                        size = load_as_xyzi(out IntPtr data_xyzi);
                                        if (size > (ulong)(int.MaxValue / sizeof(PointXYZI)))
                                        {
                                            Debug.LogError($"load failed as breached max points amount {int.MaxValue / sizeof(PointXYZI)} , this file have {size} points ‬!");
                                        }
                                        else if (size > (ulong)(int.MaxValue / sizeof(PCLPointXYZI)))
                                        {
                                            var origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZI>(data_xyzi.ToPointer(), int.MaxValue / sizeof(PCLPointXYZI), Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp_1 = new NativeArray<PCLPointXYZI>(origin, Allocator.TempJob);
                                            int remain = (int)size - temp_1.Length;
                                            origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZI>(IntPtr.Add(data_xyzi, remain * 32).ToPointer(), remain, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp_2 = new NativeArray<PCLPointXYZI>(origin, Allocator.TempJob);
                                            clear();
                                            var densed_1 = Dense(temp_1);
                                            temp_1.Dispose();
                                            var densed_2 = Dense(temp_2);
                                            temp_2.Dispose();
                                            PointXYZIs = new NativeArray<PointXYZI>((int)size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                                            NativeArray<PointXYZI>.Copy(densed_1, PointXYZIs, densed_1.Length);
                                            NativeArray<PointXYZI>.Copy(densed_2, 0, PointXYZIs, densed_1.Length, densed_2.Length);
                                            densed_1.Dispose();
                                            densed_2.Dispose();
                                            return true;
                                        }
                                        else if (size > 0)
                                        {
                                            var origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZI>(data_xyzi.ToPointer(), (int)size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp = new NativeArray<PCLPointXYZI>(origin, Allocator.TempJob);
                                            clear();
                                            PointXYZIs = Dense(temp);
                                            temp.Dispose();
                                            return true;
                                        }
                                        else
                                        {
                                            Debug.LogError("load failed as points count zero !");
                                        }
                                    }
                                    else
                                    {
                                        size = load_as_xyz(out IntPtr data_xyz);
                                        if (size > (ulong)(int.MaxValue / sizeof(PointXYZRGBA)))
                                        {
                                            Debug.LogError($"load failed as breached max points amount {int.MaxValue / sizeof(PointXYZRGBA)} , this file have {size} points ‬!");
                                        }
                                        else if (size > 0)
                                        {
                                            var origin = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<PCLPointXYZ>(data_xyz.ToPointer(), (int)size, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                                            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref origin, AtomicSafetyHandle.GetTempMemoryHandle());
#endif
                                            var temp = new NativeArray<PCLPointXYZ>(origin, Allocator.TempJob);
                                            clear();
                                            PointXYZRGBAs = Dense(temp);
                                            temp.Dispose();
                                            return true;
                                        }
                                        else
                                        {
                                            Debug.LogError("load failed as points count zero !");
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogError("load failed as missing field xyz!");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("load failed as size_blob == 0");
                        }
                    }
                    else
                    {
                        Debug.LogError($"load failed as is_dense != 1");
                    }
                }
                else
                {
                    Debug.LogError($"load failed as is_bigendian != 0");
                }
            }
            else
            {
                Debug.LogError($"load failed with return {ret}");
            }
            clear();
            return false;
        }
        public PointCloudReader(string file) => LoadPointCloudFile(file);
        public void Dispose()
        {
            if (size_blob() > 0)
            {
                clear();
            }
            if (PointXYZRGBAs.IsCreated)
            {
                PointXYZRGBAs.Dispose();
            }
            if (PointXYZIs.IsCreated)
            {
                PointXYZIs.Dispose();
            }
        }
        [BurstCompile]
        struct JobDensePointXYZ : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<PCLPointXYZ> data_in;
            [WriteOnly] internal NativeArray<PointXYZRGBA> data_out;
            public void Execute(int index) => data_out[index] = new PointXYZRGBA { xyz = data_in[index].point.xyz, bgra = Color.white };
        }
        public NativeArray<PointXYZRGBA> Dense(NativeArray<PCLPointXYZ> raw)
        {
            var job = new JobDensePointXYZ
            {
                data_in = raw,
                data_out = new NativeArray<PointXYZRGBA>(raw.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
            };
            job.Schedule(raw.Length, ushort.MaxValue).Complete();
            return job.data_out;
        }
        [BurstCompile]
        struct JobDensePointXYZRGBA : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<PCLPointXYZRGBA> data_in;
            [WriteOnly] internal NativeArray<PointXYZRGBA> data_out;
            public void Execute(int index) => data_out[index] = new PointXYZRGBA { xyz = data_in[index].point.xyz, bgra = data_in[index].bgra };
        }
        public NativeArray<PointXYZRGBA> Dense(NativeArray<PCLPointXYZRGBA> raw)
        {
            var job = new JobDensePointXYZRGBA
            {
                data_in = raw,
                data_out = new NativeArray<PointXYZRGBA>(raw.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
            };
            job.Schedule(raw.Length, ushort.MaxValue).Complete();
            return job.data_out;
        }
        [BurstCompile]
        struct JobDensePointXYZI : IJobParallelFor
        {
            [ReadOnly] internal NativeArray<PCLPointXYZI> data_in;
            [WriteOnly] internal NativeArray<PointXYZI> data_out;
            public void Execute(int index) => data_out[index] = new PointXYZI { xyz = data_in[index].point.xyz, intensity = data_in[index].intensity };
        }
        public NativeArray<PointXYZI> Dense(NativeArray<PCLPointXYZI> raw)
        {
            var job = new JobDensePointXYZI
            {
                data_in = raw,
                data_out = new NativeArray<PointXYZI>(raw.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
            };
            job.Schedule(raw.Length, ushort.MaxValue).Complete();
            return job.data_out;
        }
    }
}