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
    [ExecuteInEditMode]
    public class SiblingIndex<T> : MonoBehaviour where T : SiblingIndex<T>
    {
        [ReadOnly] [SerializeField] T last;
        [ReadOnly] [SerializeField] T next;
        public Transform Last_Transform { get; private set; }
        public Transform Next_Transform { get; private set; }
        public virtual bool EnableAdd { get; } = true;
        public virtual bool EnableMove { get; } = true;
        public virtual bool EnableRemove { get; } = true;
        public virtual T Last
        {
            get
            {
                if (Next_Transform == null && Index < transform.parent.childCount - 1)
                {
                    Next_Transform = transform.parent.GetChild(Index + 1);
                }
                if (last == null && Last_Transform)
                {
                    last = Last_Transform.GetComponent<T>();
                    last.Next_Transform = transform;
                }
                return last;
            }
        }
        public virtual T Next
        {
            get
            {
                if (Last_Transform == null && Index > 0)
                {
                    Last_Transform = transform.parent.GetChild(Index - 1);
                }
                if (next == null && Next_Transform)
                {
                    next = Next_Transform.GetComponent<T>();
                    next.Last_Transform = transform;
                }
                return next;
            }
        }
        public int Index
        {
            set
            {
                if (value >= 0 && value < transform.parent.childCount)
                {
                    OnDestroyReset();
                    transform.SetSiblingIndex(value);
                    UpdateLastNext();
                }
            }
            get => transform.GetSiblingIndex();
        }
        public virtual void UpdateIndex() => Index = Index;
        public virtual void MoveBefore() => Index--;
        public virtual void MoveAfter() => Index++;
        public virtual T AddBefore(T target)
        {
            target.transform.SetParent(transform.parent);
            target.Index = Index;
            if (last)
            {
                last.next = target;
                last.Next_Transform = target.transform;
                last = target;
            }
            return target;
        }
        public virtual T AddAfter(T target)
        {
            target.transform.SetParent(transform.parent);
            target.Index = Index + 1;
            if (next)
            {
                next.last = target;
                next.Last_Transform = target.transform;
                next = target;
            }
            return target;
        }
        protected void UpdateLastNext()
        {
            Last_Transform = Next_Transform = null;
            last = next = null;
            if (transform.parent)
            {
                if (Index > 0)
                {
                    Last_Transform = transform.parent.GetChild(Index - 1);
                }
                if (Index < transform.parent.childCount - 1)
                {
                    Next_Transform = transform.parent.GetChild(Index + 1);
                }
            }
        }
        protected void OnDestroyReset()
        {
            if (last)
            {
                last.Next_Transform = Next_Transform;
                last.next = null;
                last = null;
            }
            if (next)
            {
                next.Last_Transform = Last_Transform;
                next.last = null;
                next = null;
            }
        }
        protected virtual void Awake() => UpdateLastNext();
        protected virtual void OnDestroy() => OnDestroyReset();
    }
}