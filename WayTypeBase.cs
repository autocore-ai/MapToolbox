#region License
/******************************************************************************
* Copyright 2018-2021 The AutoCore Authors. All Rights Reserved.
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
    public abstract class WayTypeBase<T> : MonoBehaviour where T : WayTypeBase<T>
    {
        public Way Way => GetComponent<Way>() ?? gameObject.AddComponent<Way>();
        public LineRenderer LineRenderer => GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
        internal static T AddNew(Lanelet2Map map)
        {
            var ret = map.AddChildGameObject<T>(map.transform.ChildMapId());
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
        private void OnPointsMoved(Node node) => LineRenderer.SetPosition(Way.Nodes.IndexOf(node), node.Position);
        private void OnAddNode(Node node)
        {
            var index = Way.Nodes.IndexOf(node);
            LineRenderer.positionCount++;
            for (int i = LineRenderer.positionCount - 1; i > index; i--)
            {
                LineRenderer.SetPosition(i - 1, LineRenderer.GetPosition(i));
            }
            LineRenderer.SetPosition(index, node.Position);
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
        protected virtual Vector3 GetClickedPoint() => GetClickedPoint(Vector3.zero);
        protected virtual Vector3 GetClickedPoint(Vector3 offset)
        {
            var point = Utils.MousePointInSceneView;
            point.y = Utils.GetHeight(point);
            return point + offset;
        }
        internal virtual void AddNextPoint(Vector3 point)
        {
            Node node = Way.InsertNode(point);
            var list = Selection.objects.ToList();
            list.Remove(node.gameObject);
            Selection.objects = list.ToArray();
        }
        internal virtual void RemoveLastNode() => Way.RemoveNode(Way.Nodes.Count - 1);
        internal bool OnlyUsedBy(Member member) => Way.Ref.Count == 1 && Way.Ref.Contains(member);
    }
}