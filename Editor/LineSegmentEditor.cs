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
using static AutoCore.MapToolbox.Editor.Utils;

namespace AutoCore.MapToolbox.Editor
{
    [CanEditMultipleObjects]
    class LineSegmentEditor<T> : SiblingChildEditor<T> where T : LineSegment<T>
    {
        bool BezierEditing { get; set; } = false;
        Vector3 StartTangent { get; set; }
        Vector3 EndTangent { get; set; }
        protected override void AfterDefaultInspectorDraw()
        {
            base.AfterDefaultInspectorDraw();
            if (Targets != null && Targets.Length == 1)
            {
                Target.From = EditorGUILayout.Vector3Field(GetMemberName((T t) => t.From), Target.From);
                Target.To = EditorGUILayout.Vector3Field(GetMemberName((T t) => t.To), Target.To);
                if (BezierEditing)
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
                    BezierEditorGUI();
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
        }

        private void BezierEditorGUI()
        {
            if (BezierEditing)
            {
                GUILayout.BeginHorizontal();
                GUI.color = Color.yellow;
                if (GUILayout.Button("Apply"))
                {
                    BezierEditing = false;
                    Target.ApplyBezierPoints(Handles.MakeBezierPoints(Target.from, Target.to, StartTangent, EndTangent, (int)(Target.from - Target.to).magnitude * 10));
                }
                GUI.color = Color.white;
                if (GUILayout.Button("Cancel"))
                {
                    BezierEditing = false;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button("Bezier Edit"))
                {
                    BezierEditing = true;
                    StartTangent = Target.from + (Target.to - Target.from) / 4;
                    EndTangent = Target.from + 3 * (Target.to - Target.from) / 4;
                }
            }
        }

        protected virtual void OnSceneGUI()
        {
            Tools.current = Tool.None;
            if (BezierEditing)
            {
                StartTangent = Handles.PositionHandle(StartTangent, Quaternion.identity);
                EndTangent = Handles.PositionHandle(EndTangent, Quaternion.identity);
                Handles.DrawBezier(Target.from, Target.to, StartTangent, EndTangent, Color.green, null, (Target.from - Target.to).magnitude / 10);
            }
            else
            {
                Handles.color = Color.cyan;
                Handles.ArrowHandleCap(0, Target.From, Quaternion.FromToRotation(Vector3.forward, Target.To - Target.From), (Target.To - Target.From).magnitude, EventType.Repaint);
                Target.From = Handles.PositionHandle(Target.From, Quaternion.identity);
                Target.To = Handles.PositionHandle(Target.To, Quaternion.identity);
            }
        }
    }
}