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
    public class CsvDtLane
    {
        public const string fileName = "dtlane.csv";
        public const string header = "DID,Dist,PID,Dir,Apara,r,slope,cant,LW,RW";
        public CsvDtLane lastDtLane;
        public CsvDtLane nextDtLane;
        public CsvDtLane LastDtLane
        {
            get => lastDtLane;
            set
            {
                lastDtLane = value;
                lastDtLane.nextDtLane = this;
            }
        }
        public CsvPoint Point { get; set; }
        public int DID { get; set; }
        public int? PID => Point?.PID;
        public float Dist => AddSegmentDistance(0);
        public float? Dir => nextDtLane?.DirectionFrom(this);
        private float AddSegmentDistance(float currentDistance) => lastDtLane == null ? currentDistance : (Vector3.Distance(lastDtLane.Point.Position, Point.Position) + lastDtLane.AddSegmentDistance(currentDistance));
        private float DirectionFrom(CsvDtLane target) => Vector3.SignedAngle(Point.Position - target.Point.Position, Vector3.forward, Vector3.up) / 180 * Mathf.PI;
        public string CsvString => $"{DID},{Dist:F2},{PID ?? 0},{Dir ?? lastDtLane.Dir:F2},0,90000000000,0,0,2,2";
    }
}