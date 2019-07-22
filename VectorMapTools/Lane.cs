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
    public class Lane : AutoHeightBezier
    {
        static PointOctree<Lane> BeginPoints { get; set; } = new PointOctree<Lane>(1000, Vector3.zero, 100);
        internal static PointOctree<Lane> FinalPoints { get; set; } = new PointOctree<Lane>(1000, Vector3.zero, 100);
        public static List<Lane> List { get; set; } = new List<Lane>();
        [Range(0, 20)]
        public float velocityBegin = 10;
        [Range(0, 20)]
        public float velocityFinal = 10;
        [Range(0, 5)]
        public static float autoConnectDistance = 2;
        public override Vector3 StartPosition
        {
            set
            {
                base.StartPosition = value;
                if (!startPositionMove.Equals(Vector3.zero))
                {
                    var target = FinalPoints.GetNearby(startPosition, autoConnectDistance)
                        ?.Where(_ => !_.Equals(this))
                        ?.OrderBy(_ => Vector3.Distance(startPosition, _.endPosition))
                        ?.FirstOrDefault();
                    if (target)
                    {
                        base.StartPosition = target.endPosition;
                    }
                    BeginPoints.Remove(this);
                    BeginPoints.Add(this, startPosition);
                }
            }
        }
        public override Vector3 EndPosition
        {
            set
            {
                base.EndPosition = value;
                if (!endPositionMove.Equals(Vector3.zero))
                {
                    var target = BeginPoints.GetNearby(endPosition, autoConnectDistance)
                        ?.Where(_ => !_.Equals(this))
                        ?.OrderBy(_ => Vector3.Distance(endPosition, _.startPosition))
                        ?.FirstOrDefault();
                    if (target)
                    {
                        base.EndPosition = target.startPosition;
                    }
                    FinalPoints.Remove(this);
                    FinalPoints.Add(this, endPosition);
                }
            }
        }
        protected override void Awake()
        {
            base.Awake();
            LineRenderer.textureMode = LineTextureMode.Tile;
            LineRenderer.sharedMaterial = Resources.Load<Material>("MapToolbox/Lane");
        }
        protected virtual void Start()
        {
            List.Add(this);
            BeginPoints.Add(this, startPosition);
            FinalPoints.Add(this, endPosition);
        }
        private void OnDestroy()
        {
            List.Remove(this);
            BeginPoints.Remove(this);
            FinalPoints.Remove(this);
        }
    }
}
