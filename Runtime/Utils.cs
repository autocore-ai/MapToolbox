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

namespace AutoCore.MapToolbox
{
    static class Utils
    {
        //http://wiki.unity3d.com/index.php/3d_Math_functions
        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            //lines are not parallel
            if (d != 0.0f)
            {

                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }
            else
            {
                return false;
            }
        }
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
        internal static float GetHeight(Vector3 position)
        {
            const float maxHeight = 500;
            var from = new Vector3(position.x, maxHeight, position.z);
            if (Physics.Raycast(new Ray(from, Vector3.down), out RaycastHit hit, maxHeight * 2))
            {
                return hit.point.y;
            }
            else
            {
                return 0;
            }
        }
    }
}
