#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Packages.AutowareUnityTools.PcdTools
{
    public class PcdWriter : StreamWriter
    {
        public string FilePath { get; private set; }
        public PcdWriter(string filePath) : base(filePath)
        {
            FilePath = filePath;
        }

        public void WritePoints(NativeArray<float3> points)
        {
            string header = "# .PCD v0.7 - Point Cloud Data file format\n" +
            "VERSION 0.7\n" +
            "FIELDS x y z\n" +
            "SIZE 4 4 4\n" +
            "TYPE F F F\n" +
            "COUNT 1 1 1\n" +
            $"WIDTH {points.Length}\n" +
            "HEIGHT 1\n" +
            "VIEWPOINT 0 0 0 1 0 0 0\n" +
            $"POINTS {points.Length}\n" +
            "DATA binary\n";
            Write(header);
            Flush();
            unsafe
            {
                using (var binary = new UnmanagedMemoryStream((byte*)points.GetUnsafePtr(), points.Length * UnsafeUtility.SizeOf<float3>()))
                {
                    binary.CopyTo(BaseStream);
                }
            }
        }
    }
}