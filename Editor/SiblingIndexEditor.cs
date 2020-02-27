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

namespace AutoCore.MapToolbox.Editor
{
    class SiblingIndexEditor<T> : UnityEditor.Editor where T : SiblingIndex<T>
    {
        const string AddBefore = "Add Before";
        const string AddAfter = "Add After";
        const string MoveBefore = "Move Before";
        const string MoveAfter = "Move After";
        const string Remove = "Remove";
        private void OnEnable() => Undo.undoRedoPerformed += UndoRedoPerformed;
        private void OnDisable() => Undo.undoRedoPerformed -= UndoRedoPerformed;
        private void UndoRedoPerformed() => (target as T).UpdateIndex();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (targets.Length == 1)
            {
                var target = base.target as T;
                if (target.EnableAdd)
                {
                    GUI.color = Color.green;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(AddBefore))
                    {
                        var go = new GameObject(typeof(T).Name);
                        target.AddBefore(go.AddComponent(base.target.GetType()) as T);
                        Selection.activeGameObject = go;
                        Undo.RegisterCreatedObjectUndo(go, AddBefore);
                    }
                    if (GUILayout.Button(AddAfter))
                    {
                        var go = new GameObject(typeof(T).Name);
                        target.AddAfter(go.AddComponent(base.target.GetType()) as T);
                        Selection.activeGameObject = go;
                        Undo.RegisterCreatedObjectUndo(go, AddAfter);
                    }
                    GUILayout.EndHorizontal();
                }
                if (target.EnableMove)
                {
                    GUI.color = Color.yellow;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(MoveBefore))
                    {
                        Undo.SetTransformParent(target.transform, target.transform.parent, MoveBefore);
                        target.MoveBefore();
                    }
                    if (GUILayout.Button(MoveAfter))
                    {
                        Undo.SetTransformParent(target.transform, target.transform.parent, MoveAfter);
                        target.MoveAfter();
                    }
                    GUILayout.EndHorizontal();
                }
                if (target.EnableRemove)
                {
                    GUI.color = Color.red;
                    if (GUILayout.Button(Remove))
                    {
                        Undo.DestroyObjectImmediate(target.gameObject);
                    }
                }
            }
        }
    }
}