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

using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools.Export
{
    public class CsvVector
    {
        public const string fileName = "vector.csv";
        public const string header = "VID,PID,Hang,Vang";
        public Quaternion Rotation { get; set; }
        public CsvPoint Point { get; set; }
        public int VID { get; set; }
        public int? PID => Point?.PID;
        public float Hang => Rotation.eulerAngles.y + 90;
        public float Vang => Rotation.eulerAngles.x + 90;
        public string CsvString => $"{VID},{PID ?? 0},{Hang:F2},{Vang:F2}";
    }
}