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


using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoCore.MapToolbox.Autoware
{
    static class Utils
    {
        public static void RemoveEmpty(string folder, string fileName)
        {
            string file = Path.Combine(folder, fileName);
            if (File.Exists(file))
            {
                if (File.ReadAllLines(file).Length <= 1)
                {
                    File.Delete(file);
                }
            }
        }
        public static void CleanOrCreateNew(string folder, string fileName, string header)
        {
            string file = Path.Combine(folder, fileName);
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            File.Create(file).Dispose();
            File.AppendAllLines(file, new[] { header });
        }
        public static void AppendData(string folder, string fileName, IEnumerable<string> data)
        {
            string file = Path.Combine(folder, fileName);
            if (File.Exists(file))
            {
                File.AppendAllLines(file, data);
            }
        }
        public static IEnumerable<string> ReadLinesExcludeFirstLine(string folder, string fileName)
        {
            string file = Path.Combine(folder, fileName);
            if (File.Exists(file))
            {
                var data = File.ReadAllLines(file).ToList();
                if (data.Count > 1)
                {
                    data.RemoveAt(0);
                    return data;
                }
            }
            return null;
        }
        public static IEnumerable<string[]> Split(this IEnumerable<string> lineStr, params char[] separator) => lineStr?.Select(_ => _.Split(separator));

    }
}