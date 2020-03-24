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
    [ExecuteInEditMode]
    public class SiblingChild<T> : MonoBehaviour where T : SiblingChild<T>
    {
        [HideInInspector] public Transform last;
        [HideInInspector] public Transform next;
        public int Index
        {
            set => transform.SetSiblingIndex(value);
            get => transform.GetSiblingIndex();
        }
        public virtual void UpdateRef()
        {
            if (transform.parent)
            {
                var index = Index;
                UpdateLast(index);
                UpdateNext(index);
            }
        }
        private void UpdateLast(int index)
        {
            if (index == 0)
            {
                last = null;
            }
            else
            {
                last = transform.parent.GetChild(index - 1);
            }
        }
        private void UpdateNext(int index)
        {
            if (index == transform.parent.childCount - 1)
            {
                next = null;
            }
            else
            {
                next = transform.parent.GetChild(index + 1);
            }
        }
        internal virtual GameObject AddBefore()
        {
            var go = new GameObject(name);
            go.AddComponent(GetType());
            go.transform.parent = transform.parent;
            go.transform.SetSiblingIndex(Index);
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "AddBefore");
#endif
            return go;
        }

        internal virtual GameObject AddAfter()
        {
            var go = new GameObject(name);
            go.AddComponent(GetType());
            go.transform.parent = transform.parent;
            go.transform.SetSiblingIndex(Index + 1);
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "AddAfter");
#endif
            return go;
        }

        internal void MoveForward()
        {
            Index--;
#if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(transform, transform.parent, "MoveForward");
#endif
        }

            internal void MoveBack()
        {
            if (Index == transform.parent.childCount - 1)
            {
                transform.SetAsFirstSibling();
            }
            else
            {
                Index++;
            }
#if UNITY_EDITOR
            UnityEditor.Undo.SetTransformParent(transform, transform.parent, "MoveBack");
#endif
        }

        internal virtual void Remove()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(gameObject);
#endif
        }
    }
}