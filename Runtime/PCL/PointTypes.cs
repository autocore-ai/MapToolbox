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


using Unity.Mathematics;
using UnityEngine;

namespace AutoCore.MapToolbox.PCL
{
    public struct PCLPointXYZRGBA
    {
        public float4 point;
        public Color32 bgra;
        public uint3 _;
    }
    public struct PCLPointXYZI
    {
        public float4 point;
        public float intensity;
        public uint3 _;
    }
    public struct PCLPointXYZ
    {
        public float4 point;
    }
    public struct PointXYZRGBA
    {
        public Vector3 xyz;
        public Color32 bgra;
    }
    public struct PointXYZI
    {
        public Vector3 xyz;
        public float intensity;
    }
}