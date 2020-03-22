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
    [CustomEditor(typeof(ADASGoLane))]
    class ADASGoLaneEditor : LineSegmentEditor<ADASGoLane>
    {
        SerializedProperty bLane;
        SerializedProperty fLane;
        static bool Visible_b { get; set; }
        static bool Visible_f { get; set; }
        protected override void OnEnable()
        {
            base.OnEnable();
            bLane = serializedObject.FindProperty(GetMemberName((ADASGoLane t) => t.bLane));
            fLane = serializedObject.FindProperty(GetMemberName((ADASGoLane t) => t.fLane));
            Target.OnEditorEnable();
        }

        protected override void AfterDefaultInspectorDraw()
        {
            base.AfterDefaultInspectorDraw();
            DrawBFLane();
            GUI.enabled = false;
            EditorGUILayout.EnumFlagsField(GetMemberName((ADASGoLane t) => t.jct), Target.jct);
            GUI.enabled = true;
        }

        private void DrawBFLane()
        {
            if (bLane != null)
            {
                EditorGUILayout.BeginHorizontal();
                Visible_b = EditorGUILayout.Foldout(Visible_b, bLane.displayName);
                GUI.enabled = false;
                EditorGUILayout.IntField("    size : ", bLane.arraySize);
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                GUI.enabled = false;
                if (Visible_b)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < bLane.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(bLane.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                }
                GUI.enabled = true;
            }
            if (fLane != null)
            {
                EditorGUILayout.BeginHorizontal();
                Visible_f = EditorGUILayout.Foldout(Visible_f, fLane.displayName);
                GUI.enabled = false;
                EditorGUILayout.IntField("    size : ", fLane.arraySize);
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                GUI.enabled = false;
                if (Visible_f)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < fLane.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(fLane.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                }
                GUI.enabled = true;
            }
        }
    }
}