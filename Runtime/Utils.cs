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

namespace AutoCore.MapToolbox
{
    static class Utils
    {
        public static bool AllParentActived(this Component component)
        {
            if (component.transform.parent != null)
            {
                if (component.transform.parent.gameObject.activeSelf)
                {
                    return AllParentActived(component.transform.parent);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return component.gameObject.activeSelf;
            }
        }
    }
}
