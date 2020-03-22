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


using UnityEditor;
using UnityEngine;
using static AutoCore.MapToolbox.Editor.Utils;

namespace AutoCore.MapToolbox.Editor
{
    [CanEditMultipleObjects]
    class LineSegmentEditor<T> : SiblingChildEditor<T> where T : LineSegment<T>
    {
        static bool ArrowDisplay { get; set; }
        bool Subdivision { get; set; } = false;
        Vector3 StartTangent { get; set; }
        Vector3 EndTangent { get; set; }
        protected override void AfterDefaultInspectorDraw()
        {
            base.AfterDefaultInspectorDraw();
            if (Targets != null && Targets.Length == 1)
            {
                Target.LocalFrom = EditorGUILayout.Vector3Field(GetMemberName((T t) => t.LocalFrom), Target.LocalFrom);
                Target.LocalTo = EditorGUILayout.Vector3Field(GetMemberName((T t) => t.LocalTo), Target.LocalTo);
                if (Subdivision)
                {
                    StartTangent = EditorGUILayout.Vector3Field(GetMemberName((LineSegmentEditor<T> t) => t.StartTangent), StartTangent);
                    EndTangent = EditorGUILayout.Vector3Field(GetMemberName((LineSegmentEditor<T> t) => t.EndTangent), EndTangent);
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Targets != null)
            {
                if (Targets.Length == 1)
                {
                    InspectorGUIAddButton();
                    SubdivisionEditorGUI();
                }
                else
                {
                    GUI.color = Color.yellow;
                    if (GUILayout.Button(GetMethodName<T[]>(Target.Merge)))
                    {
                        Target.Merge(Targets);
                        GUI.color = Color.white;
                        return;
                    }
                    GUI.color = Color.white;
                }
            }
            InspectorGUIRemoveButton();
            ArrowDisplay = GUILayout.Toggle(ArrowDisplay, "Direction Arrow");
        }

        private void SubdivisionEditorGUI()
        {
            if (Subdivision)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Normal way"))
                {
                    Subdivision = false;
                    Target.Subdivision(StartTangent, EndTangent, 1);
                }
                else if (GUILayout.Button("High Way"))
                {
                    Subdivision = false;
                    Target.Subdivision(StartTangent, EndTangent, 5);
                }
                GUI.color = Color.white;
                if (GUILayout.Button("Cancel"))
                {
                    Subdivision = false;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Subdivision"))
                {
                    Subdivision = true;
                    StartTangent = Target.From + (Target.To - Target.From) / 4;
                    EndTangent = Target.From + 3 * (Target.To - Target.From) / 4;
                }
                else if (GUILayout.Button("Auto Subdivision"))
                {
                    Target.AutoSubdivision();
                }
            }
        }

        protected virtual void OnSceneGUI()
        {
            Tools.current = Tool.None;
            if (Subdivision)
            {
                var oldStartTangent = StartTangent;
                var oldEndTangent = EndTangent;
                var newStartTangent = Handles.PositionHandle(StartTangent, Quaternion.identity);
                var newEndTangent = Handles.PositionHandle(EndTangent, Quaternion.identity);
                if (oldStartTangent != newStartTangent)
                {
                    StartTangent = newStartTangent;
                    Repaint();
                }
                else if (oldEndTangent != newEndTangent)
                {
                    EndTangent = newEndTangent;
                    Repaint();
                }
                Handles.DrawBezier(Target.From, Target.To, StartTangent, EndTangent, Color.green, null, (Target.From - Target.To).magnitude / 10);
            }
            else
            {
                if (ArrowDisplay)
                {
                    Handles.color = Color.cyan;
                    Handles.ArrowHandleCap(0, Target.From, Quaternion.FromToRotation(Vector3.forward, Target.To - Target.From), (Target.To - Target.From).magnitude, EventType.Repaint);
                }
                var oldFrom = Target.From;
                var oldTo = Target.To;
                var newFrom = Handles.PositionHandle(oldFrom, Quaternion.identity);
                var newTo = Handles.PositionHandle(oldTo, Quaternion.identity);
                if (newFrom != oldFrom)
                {
                    Target.From = newFrom;
                    Repaint();
                }
                else if (newTo != oldTo)
                {
                    Target.To = newTo;
                    Repaint();
                }
            }
        }
    }
}