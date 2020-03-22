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


using AutoCore.MapToolbox.Autoware;
using UnityEditor;
using UnityEngine;
using static AutoCore.MapToolbox.Editor.Utils;

namespace AutoCore.MapToolbox.Editor.Autoware
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ADASGoLine))]
    class ADASGoLineEditor : LineSegmentEditor<ADASGoLine>
    {
        SerializedProperty bLine;
        SerializedProperty fLine;
        protected override void OnEnable()
        {
            base.OnEnable();
            bLine = serializedObject.FindProperty(GetMemberName((ADASGoLine t) => t.bLine));
            fLine = serializedObject.FindProperty(GetMemberName((ADASGoLine t) => t.fLine));
        }

        protected override void AfterDefaultInspectorDraw()
        {
            base.AfterDefaultInspectorDraw();
            DrawBFLine();
        }
        private void DrawBFLine()
        {
            GUI.enabled = false;
            if (bLine != null)
            {
                EditorGUILayout.PropertyField(bLine);
            }
            if (fLine != null)
            {
                EditorGUILayout.PropertyField(fLine);
            }
            GUI.enabled = true;
        }
    }
}