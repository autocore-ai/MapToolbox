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

using UnityEngine;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Relation))]
    class RegulatoryElement : MonoBehaviour
    {
        public Way ref_line;
        public Way refers;
        public enum SubType
        {
            road,
            traffic_light,
            traffic_sign
        }
        public SubType subType;
        public Relation Relation => GetComponent<Relation>() ?? gameObject.AddComponent<Relation>();
        internal static RegulatoryElement AddNew(Lanelet2Map map)
        {
            var ret = map.AddChildGameObject<RegulatoryElement>(map.transform.childCount.ToString());
            ret.gameObject.RecordUndoCreateGo();
            return ret;
        }
    }
}