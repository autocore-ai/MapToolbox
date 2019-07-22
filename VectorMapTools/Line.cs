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

namespace Packages.MapToolbox.VectorMapTools
{
    [RequireComponent(typeof(LineRenderer)), ExecuteInEditMode]
    public class Line : MonoBehaviour
    {
        [SerializeField]
        public virtual Vector3[] Points
        {
            get
            {
                var ret = new Vector3[LineRenderer.positionCount];
                LineRenderer.GetPositions(ret);
                return ret;
            }
            set
            {
                LineRenderer.positionCount = value.Length;
                LineRenderer.SetPositions(value);
            }
        }
        public LineRenderer LineRenderer { get; private set; }
        [SerializeField]
        public int Count
        {
            get => LineRenderer.positionCount;
            set => LineRenderer.positionCount = value;
        }
        [SerializeField]
        public float StartWidth
        {
            get => LineRenderer.startWidth;
            set => LineRenderer.startWidth = value;
        }
        [SerializeField]
        public float EndWidth
        {
            get => LineRenderer.endWidth;
            set => LineRenderer.endWidth = value;
        }
        protected virtual void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
        }
        protected virtual void Start()
        {
            if (LineRenderer.positionCount == 2 && LineRenderer.GetPosition(0).Equals(Vector3.zero) && LineRenderer.GetPosition(1).Equals(Vector3.forward))
            {
                LineRenderer.SetPosition(0, transform.position);
                LineRenderer.SetPosition(1, transform.position + Vector3.right);
            }
        }
        public virtual void SetPosition(int index, Vector3 position)
        {
            if (index == 0)
            {
                transform.position = position;
            }
            else if (index > 1 && position.Equals(Vector3.zero))
            {
                position = LineRenderer.GetPosition(index - 1) + (LineRenderer.GetPosition(index - 1) - LineRenderer.GetPosition(index - 2));
            }
            LineRenderer.SetPosition(index, position);
        }
    }
}