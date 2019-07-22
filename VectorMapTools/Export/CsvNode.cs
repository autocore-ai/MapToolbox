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


namespace Packages.MapToolbox.VectorMapTools.Export
{
    public class CsvNode
    {
        public const string fileName = "node.csv";
        public const string header = "NID,PID";
        public CsvPoint Point { get; set; }
        public int NID { get; set; }
        public int? PID => Point?.PID;
        public string CsvString => $"{NID},{PID ?? 0}";
    }
}