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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools
{
    public class StopLine : AutoHeightLine
    {
        public List<Lane> Lanes { get; set; } = new List<Lane>();
        [Range(0, 1)]
        public float displayWidth = 0.2f;
        [Range(0, 5)]
        public float distance_autoConnectLane = 3f;
        public static List<StopLine> List { get; set; } = new List<StopLine>();
        public SignalLight[] signalLights;
        protected override void Awake()
        {
            base.Awake();
            LineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.white
            };
            LineRenderer.startWidth = LineRenderer.endWidth = displayWidth;
        }
        protected override void Start()
        {
            base.Start();
            List.Add(this);
        }
        private void Update()
        {
            if (signalLights != null)
            {
                if (signalLights.Length != Count + 1)
                {
                    Count = signalLights.Length + 1;
                }
            }
        }
        public override void SetPosition(int index, Vector3 position)
        {
            base.SetPosition(index, position);
            Lanes.Clear();
            for (int i = 1; i < Count; i++)
            {
                Lanes.Add(GetLane((Points[i - 1] + Points[i]) / 2));
            }
        }
        private Lane GetLane(Vector3 vector3)
        {
            var finalPoints = Lane.FinalPoints.GetNearby(vector3, distance_autoConnectLane);
            if (finalPoints != null && finalPoints.Length > 0)
            {
                return finalPoints.OrderBy(_ => Vector3.Distance(vector3, _.endPosition)).First();
            }
            return null;
        }
        private void OnDestroy() => List.Remove(this);
        private void OnDrawGizmosSelected()
        {
            if (signalLights != null)
            {
                for (int i = 0; i < signalLights.Length; i++)
                {
                    if (signalLights[i])
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine((Points[i] + Points[i + 1]) / 2, signalLights[i].transform.position);
                    }
                }
            }
        }
    }
}