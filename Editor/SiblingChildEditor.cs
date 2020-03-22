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
    class SiblingChildEditor<T> : Editor<T> where T : SiblingChild<T>
    {
        SerializedProperty last;
        SerializedProperty next;
        protected override void OnEnable()
        {
            Target.UpdateRef();
            base.OnEnable();
            last = serializedObject.FindProperty(GetMemberName((T t) => t.last));
            next = serializedObject.FindProperty(GetMemberName((T t) => t.next));
        }
        protected void InspectorGUIAddButton()
        {
            GUI.color = Color.green;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetMethodName(Target.AddBefore)))
            {
                Selection.activeGameObject = Target.AddBefore();
            }
            else if (GUILayout.Button(GetMethodName(Target.AddAfter)))
            {
                Selection.activeGameObject = Target.AddAfter();
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }

        protected void InspectorGUIMoveButton()
        {
            GUI.color = Color.yellow;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(GetMethodName(Target.MoveForward)))
            {
                Target.MoveForward();
            }
            else if (GUILayout.Button(GetMethodName(Target.MoveBack)))
            {
                Target.MoveBack();
            }
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }

        protected void InspectorGUITransformRef()
        {
            GUI.color = Color.white;
            GUI.enabled = false;
            EditorGUILayout.IntField(GetMemberName((T t) => t.Index), Target.Index);
            EditorGUILayout.PropertyField(last);
            EditorGUILayout.PropertyField(next);
            GUI.enabled = true;
        }
        protected void InspectorGUIRemoveButton()
        {
            GUI.color = Color.red;
            if (GUILayout.Button(GetMethodName(Target.Remove)))
            {
                if (Targets != null)
                {
                    foreach (var item in Targets)
                    {
                        item.Remove();
                    }
                }
            }
            GUI.color = Color.white;
        }
    }
}