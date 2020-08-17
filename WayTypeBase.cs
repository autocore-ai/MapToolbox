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

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Way), typeof(LineRenderer))]
    abstract class WayTypeBase<T> : MonoBehaviour where T : WayTypeBase<T>
    {
        public Way Way => GetComponent<Way>() ?? gameObject.AddComponent<Way>();
        public LineRenderer LineRenderer => GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
        internal static T AddNew(Lanelet2Map map)
        {
            var ret = map.AddChildGameObject<T>(map.transform.childCount.ToString());
            ret.gameObject.RecordUndoCreateGo();
            return ret;
        }
        protected virtual void Start()
        {
            Way.OnNodeMoved -= OnPointsMoved;
            Way.OnNodeMoved += OnPointsMoved;
            Way.OnAddNode -= OnAddNode;
            Way.OnAddNode += OnAddNode;
            LineRenderer.positionCount = Way.Nodes.Count;
            LineRenderer.SetPositions(Way.Nodes.Select(_ => _.transform.localPosition).ToArray());
            LineRenderer.useWorldSpace = false;
            LineRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            UpdateRenderer();
        }
        protected virtual void OnDestroy()
        {
            Way.OnNodeMoved -= OnPointsMoved;
            Way.OnAddNode -= OnAddNode;
        }
        internal void UpdateRenderer()
        {
            Way.Nodes.RemoveNull();
            LineRenderer.positionCount = Way.Nodes.Count;
            LineRenderer.SetPositions(Way.Nodes.Select(_ => _.Position).ToArray());
        }
        private void OnPointsMoved(int index, Vector3 position) => LineRenderer.SetPosition(index, position);
        internal void AddPointFinal(Vector3 point) => Way.InsertNode(point);
        private void OnAddNode(int index, Vector3 position)
        {
            LineRenderer.positionCount++;
            for (int i = LineRenderer.positionCount - 1; i > index; i--)
            {
                LineRenderer.SetPosition(i - 1, LineRenderer.GetPosition(i));
            }
            LineRenderer.SetPosition(index, position);
        }
        internal void RemovePointFinal()
        {
            if (Way.Nodes.Count > 0)
            {
                var lastNode = Way.Nodes.Last();
                Way.Nodes.Remove(lastNode);
                lastNode.RemoveRef(Way);
                if (LineRenderer.positionCount > 0)
                {
                    LineRenderer.positionCount--;
                }
            }
        }
        internal bool OnlyUsedBy(Member member) => Way.Ref.Count == 1 && Way.Ref.Contains(member);
    }
}