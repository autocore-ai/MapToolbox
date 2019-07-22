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

using Roslin.Msg.vector_map_msgs;
using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools.Export
{
    class CsvPoint
    {
        internal const string fileName = "point.csv";
        internal const string header = "PID,B,L,H,Bx,Ly,ReF,MCODE1,MCODE2,MCODE3";
        public Vector3 Position { get; set; }
        public int PID { get; set; }
        public float H => Position.y;
        public float Bx => -Position.x;
        public float Ly => Position.z;
        public string CsvString => $"{PID},0,0,{H:F2},{Bx:F2},{Ly:F2},0,0,0,0";
        public static implicit operator Point(CsvPoint csvPoint)
        {
            return new Point
            {
                pid = csvPoint.PID,
                b = 0,
                l = 0,
                h = csvPoint.H,
                bx = csvPoint.Bx,
                ly = csvPoint.Ly,
                @ref = 0,
                mcode1 = 0,
                mcode2 = 0,
                mcode3 = 0
            };
        }
    }
}