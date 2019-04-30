#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Packages.UnityTools.PcdTools
{
    public class PcdReader : StreamReader
    {
        public string FilePath { get; private set; }
        public NativeArray<float3byte4> PcdData_xyz_rgb { get; set; }
        public NativeArray<float4> PcdData_xyz_intensity { get; set; }
        public NativeArray<float3> PcdData_xyz { get; private set; }
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
                        default:
                            Debug.LogError($"Unknow field type {fields}");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("Binary format pcd surported only currently !");
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
            base.Dispose(disposing);
        }
    }
}
