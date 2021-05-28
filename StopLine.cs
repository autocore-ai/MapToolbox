#region License
/******************************************************************************
* Copyright 2018-2021 The AutoCore Authors. All Rights Reserved.
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

using System.Linq;
using UnityEngine;

namespace Packages.MapToolbox
{
    public class StopLine : WayTypeBase<StopLine>
    {
        protected override void Start()
        {
            base.Start();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
            LineRenderer.startColor = LineRenderer.endColor = Color.red;
        }

        internal Lanelet GetLanelet()
        {
            var lineThins = Way.Nodes.SelectMany(_ => _.Ref).Select(_ => _.GetComponent<LineThin>()).Where(_ => _ != null);
            var lanelets = lineThins.SelectMany(_=>_.Way.Ref).Select(_ => _.GetComponent<Lanelet>()).Where(_ => _ != null);
            var count = lanelets.GroupBy(_ => _).Where(_ => _.Count() > 1);
            if (count.Count() > 0)
            {
                return count.First().Key;
            }
            return null;
        }
    }
}