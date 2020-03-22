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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    [ExecuteInEditMode]
    abstract class CollectionADASMapGo : MonoBehaviour
    {
        AutowareADASMap autowareADASMap;
        public AutowareADASMap AutowareADASMap
        {
            get
            {
                if (autowareADASMap == null)
                {
                    autowareADASMap = GetComponentInParent<AutowareADASMap>();
                }
                return autowareADASMap;
            }
        }
        public virtual void Csv2Go() { }
        public virtual void Go2Csv() { }
    }
    class CollectionADASMapGo<T> : CollectionADASMapGo, IEnumerable<KeyValuePair<int, T>> where T : IADASMapGameObject
    {
        protected Dictionary<int, T> Dic { get; set; } = new Dictionary<int, T>();
        public T this[int index]
        {
            get
            {
                if (Dic.TryGetValue(index, out T value))
                {
                    return value;
                }
                return default;
            }
        }
        public void Add(int id, T value)
        {
            if (Dic.ContainsKey(id))
            {
                Dic[id] = value;
            }
            else
            {
                Dic.Add(id, value);
            }
        }
        public IEnumerator<KeyValuePair<int, T>> GetEnumerator() => Dic.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Dic.GetEnumerator();
    }
}