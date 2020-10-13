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
            Way.OnRemoveNode -= OnRemoveNode;
            Way.OnRemoveNode += OnRemoveNode;
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
            Way.OnRemoveNode -= OnRemoveNode;
        }
        internal void UpdateRenderer()
        {
            Way.Nodes.RemoveNull();
            LineRenderer.positionCount = Way.Nodes.Count;
            LineRenderer.SetPositions(Way.Nodes.Select(_ => _.Position).ToArray());
        }
        private void OnPointsMoved(Node node) => LineRenderer.SetPosition(Way.Nodes.IndexOf(node), node.Position);
        internal void AddPointFirst(Vector3 point) => Way.InsertNode(point, 0);
        internal void AddPointFinal(Vector3 point) => Way.InsertNode(point);
        internal void SetOrAddPointFinal(Vector3 point)
        {
            if (Way.Nodes.Count > 0)
            {
                Way.Nodes.Last().Position = point;
            }
            else
            {
                AddPointFinal(point);
            }
        }
        private void OnAddNode(Node node)
        {
            var index = Way.Nodes.IndexOf(node);
            LineRenderer.positionCount++;
            for (int i = LineRenderer.positionCount - 1; i > index; i--)
            {
                LineRenderer.SetPosition(i, LineRenderer.GetPosition(i - 1));
            }
            LineRenderer.SetPosition(index, node.Position);
        }
        private void OnRemoveNode(Node node)
        {
            var index = Way.Nodes.IndexOf(node);
            if (LineRenderer.positionCount > index)
            {
                for (int i = index; i < LineRenderer.positionCount - 1; i++)
                {
                    LineRenderer.SetPosition(index, LineRenderer.GetPosition(index + 1));
                }
                LineRenderer.positionCount--;
            }
        }
        internal void RemoveNode()
        {
            if (NearLastPoint(Utils.MousePointInSceneView))
            {
                RemoveNodeLast();
            }
            else
            {
                RemoveNodeFirst();
            }
        }
        internal bool NearLastPoint(Vector3 center) => Way.Nodes.Count <= 1
|| Vector3.Distance(center, Way.Nodes.First().Position) >= Vector3.Distance(center, Way.Nodes.Last().Position);
        internal void RemoveNodeFirst()
        {
            if (Way.Nodes.Count > 0)
            {
                Way.RemoveNode(Way.Nodes.First());
            }
        }
        internal void RemoveNodeLast()
        {
            if (Way.Nodes.Count > 0)
            {
                Way.RemoveNode(Way.Nodes.Last());
            }
        }
        internal bool OnlyUsedBy(Member member) => Way.Ref.Count == 1 && Way.Ref.Contains(member);
    }
}