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

namespace Packages.MapToolbox.PcdTools
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