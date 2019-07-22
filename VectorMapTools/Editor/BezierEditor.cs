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

using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox.VectorMapTools.Editor
{
    [CustomEditor(typeof(Bezier))]
    class BezierEditor : LRMapElementEditor
    {
        internal virtual void OnSceneGUI()
        {
            Tools.current = Tool.Custom;
            Bezier t = target as Bezier;
            EditorGUI.BeginChangeCheck();
            t.StartPosition = Handles.PositionHandle(t.startPosition, Quaternion.identity);
            t.EndPosition = Handles.PositionHandle(t.endPosition, Quaternion.identity);
            t.startTangent = Handles.PositionHandle(t.startTangent, Quaternion.identity);
            t.endTangent = Handles.PositionHandle(t.endTangent, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(t);
                if (!t.startPosition.Equals(t.endPosition))
                    t.Points = Handles.MakeBezierPoints(t.startPosition, t.endPosition, t.startTangent, t.endTangent, Mathf.CeilToInt(Vector3.Distance(t.startPosition, t.endPosition)));
            }
        }
    }
}