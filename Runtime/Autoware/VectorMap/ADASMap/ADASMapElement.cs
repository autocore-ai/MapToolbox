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


using System.Collections.Generic;
using System.Linq;

namespace AutoCore.MapToolbox.Autoware
{
    abstract class ADASMapElement<T> where T : ADASMapElement<T>
    {
        public int ID { get; set; }
        static public List<T> List { get; set; } = new List<T>();
        static Dictionary<int, T> dic;
        protected static Dictionary<int, T> Dic
        {
            get
            {
                if (dic == null)
                {
                    dic = List.ToDictionary(_ => _.ID);
                }
                return dic;
            }
            set => dic = value;
        }
        public static void Reset()
        {
            List.Clear();
            dic?.Clear();
            dic = null;
        }
        public static void ReIndex()
        {
            for (int i = 0; i < List.Count; i++)
            {
                List[i].ID = i + 1;
            }
            dic = List.ToDictionary(_ => _.ID);
        }
        public ADASMapElement() { List.Add(this as T); }
    }
}