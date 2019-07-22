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

using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Packages.MapToolbox.PcdTools
{
    public class PcdReader : StreamReader
    {
        public string FilePath { get; private set; }
        public NativeArray<float3byte4> PcdData_xyz_rgb { get; set; }
        public NativeArray<float4> PcdData_xyz_intensity { get; set; }
        public NativeArray<float3> PcdData_xyz { get; private set; }
        public NativeArray<xyz_intensity_ring> PcdData_xyz_intensity_ring { get; set; }
        public PcdReader(string filePath) : base(filePath)
        {
            FilePath = filePath;
            string headerLine;
            int byteCount = 0;
            int pointsCount = 0;
            string fields = string.Empty;
            do
            {
                headerLine = ReadLine();
                byteCount += headerLine.Length + 1;
                if (headerLine.ToLower().Contains("fields"))
                {
                    fields = headerLine;
                }
                else if (headerLine.ToLower().Contains("points"))
                {
                    pointsCount = int.Parse(headerLine.Split(' ')[1]);
                }
            } while (headerLine.StartsWith("#") || !headerLine.ToLower().Contains("data"));
            if (pointsCount > 0)
            {
                if (headerLine.ToLower().Equals("data binary"))
                {
                    BaseStream.Position = byteCount;
                    switch (fields.ToLower())
                    {
                        case "fields x y z":
                            PcdData_xyz = new NativeArray<float3>(pointsCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                            unsafe
                            {
                                fixed (byte* data = new BinaryReader(BaseStream).ReadBytes(pointsCount * sizeof(float3)))
                                {
                                    UnsafeUtility.MemCpy(PcdData_xyz.GetUnsafePtr(), data, pointsCount * sizeof(float3));
                                }
                            }
                            break;
                        case "fields x y z rgb":
                            PcdData_xyz_rgb = new NativeArray<float3byte4>(pointsCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                            unsafe
                            {
                                fixed (byte* data = new BinaryReader(BaseStream).ReadBytes(pointsCount * sizeof(float3byte4)))
                                {
                                    UnsafeUtility.MemCpy(PcdData_xyz_rgb.GetUnsafePtr(), data, pointsCount * sizeof(float3byte4));
                                }
                            }
                            break;
                        case "fields x y z intensity":
                            PcdData_xyz_intensity = new NativeArray<float4>(pointsCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                            unsafe
                            {
                                fixed (byte* data = new BinaryReader(BaseStream).ReadBytes(pointsCount * sizeof(float4)))
                                {
                                    UnsafeUtility.MemCpy(PcdData_xyz_intensity.GetUnsafePtr(), data, pointsCount * sizeof(float4));
                                }
                            }
                            break;
                        case "x y z _ intensity ring _":
                            PcdData_xyz_intensity_ring = new NativeArray<xyz_intensity_ring>(pointsCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                            unsafe
                            {
                                fixed (byte* data = new BinaryReader(BaseStream).ReadBytes(pointsCount * sizeof(xyz_intensity_ring)))
                                {
                                    UnsafeUtility.MemCpy(PcdData_xyz_intensity_ring.GetUnsafePtr(), data, pointsCount * sizeof(xyz_intensity_ring));
                                }
                            }
                            break;
                        default:
                            Debug.LogError($"Unknow field type {fields}");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("Binary format pcd supported only currently !");
                }
            }
            else
            {
                Debug.LogError("points count invalid !");
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (PcdData_xyz.IsCreated)
                PcdData_xyz.Dispose();
            if (PcdData_xyz_rgb.IsCreated)
                PcdData_xyz_rgb.Dispose();
            if (PcdData_xyz_intensity.IsCreated)
                PcdData_xyz_intensity.Dispose();
            if (PcdData_xyz_intensity_ring.IsCreated)
                PcdData_xyz_intensity_ring.Dispose();
            base.Dispose(disposing);
        }
    }
}