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


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Relation))]
    class Lanelet : MonoBehaviour
    {
        static Material Material { get; set; }
        Lanelet2Map Lanelet2Map => GetComponentInParent<Lanelet2Map>();
        private void Start()
        {
            if (!Material)
            {
                Material = new Material(AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat"));
                Color color = Color.green;
                color.a = 0.2f;
                Material.SetColor("_Color", color);
            }
            MeshRenderer.sharedMaterial = Material;
            left.Way.OnNodeMoved -= OnNodeMoved;
            left.Way.OnNodeMoved += OnNodeMoved;
            right.Way.OnNodeMoved -= OnNodeMoved;
            right.Way.OnNodeMoved += OnNodeMoved;
            UpdateRenderer();
        }

        private void OnNodeMoved(int arg0, Vector3 arg1) => UpdateRenderer();

        public MeshFilter MeshFilter => GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        public MeshRenderer MeshRenderer => GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
        public Relation Relation => GetComponent<Relation>() ?? gameObject.AddComponent<Relation>();
        public List<Vector3> CenterPoints { get; set; } = new List<Vector3>();
        public LineThin left;
        public LineThin right;
        public float width = 3.75f;
        internal static Lanelet AddNew(Lanelet2Map map)
        {
            var ret = map.AddChildGameObject<Lanelet>(map.transform.childCount.ToString());
            ret.gameObject.RecordUndoCreateGo();
            ret.left = LineThin.AddNew(map);
            ret.right = LineThin.AddNew(map);
            ret.left.Way.Ref.Add(ret.Relation);
            ret.right.Way.Ref.Add(ret.Relation);
            return ret;
        }
        private void RemovePoints()
        {
            if (CenterPoints.Count > 0)
            {
                CenterPoints.RemoveAt(CenterPoints.Count - 1);
            }
            left.RemovePointFinal();
            right.RemovePointFinal();
        }
        private void AddPoints()
        {
            var centerPoint = Utils.MousePointInSceneView;
            centerPoint.y = 0;
            if (CenterPoints.Count > 1)
            {
                var lastPoint = CenterPoints.Last();
                var leftPoint = centerPoint + Vector3.Cross(centerPoint - lastPoint, Vector3.up).normalized * width / 2;
                var rightPoint = centerPoint + Vector3.Cross(centerPoint - lastPoint, Vector3.down).normalized * width / 2;
                left.AddPointFinal(leftPoint);
                right.AddPointFinal(rightPoint);
            }
            else if (CenterPoints.Count == 1)
            {
                var lastPoint = CenterPoints.Last();
                var leftPoint0 = lastPoint + Vector3.Cross(centerPoint - lastPoint, Vector3.up).normalized * width / 2;
                var rightPoint0 = lastPoint + Vector3.Cross(centerPoint - lastPoint, Vector3.down).normalized * width / 2;
                var leftPoint1 = centerPoint + Vector3.Cross(centerPoint - lastPoint, Vector3.up).normalized * width / 2;
                var rightPoint1 = centerPoint + Vector3.Cross(centerPoint - lastPoint, Vector3.down).normalized * width / 2;
                left.AddPointFinal(leftPoint0);
                right.AddPointFinal(rightPoint0);
                left.AddPointFinal(leftPoint1);
                right.AddPointFinal(rightPoint1);
            }
            CenterPoints.Add(centerPoint);
        }
        internal void OnEditorEnable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            SceneView.duringSceneGui += DuringSceneGui;
        }
        internal void OnEditorDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            SceneVisibilityManager.instance.EnableAllPicking();
        }
        private void DuringSceneGui(SceneView obj)
        {
            if (EditorUpdate.MouseInSceneView)
            {
                if (EditorUpdate.MouseLeftButtonDownWithCtrl && EditorUpdate.MouseLeftButtonDownWithShift)
                {
                    RemovePoints();
                    UpdateRenderer();
                    SceneVisibilityManager.instance.DisableAllPicking();
                }
                else if (EditorUpdate.MouseLeftButtonDownWithCtrl)
                {
                    AddPoints();
                    UpdateRenderer();
                    SceneVisibilityManager.instance.DisableAllPicking();
                }
            }
        }
        internal void UpdateRenderer()
        {
            MeshFilter.sharedMesh?.Clear();
            if (left.Way.Nodes.Count > 1 || right.Way.Nodes.Count > 1)
            {
                List<Vector3> leftPoints = left.Way.Nodes.Select(_ => _.Position).ToList();
                List<Vector3> rightPoints = right.Way.Nodes.Select(_ => _.Position).ToList();
                Mesh mesh = LinkLeftRightPointsMesh(leftPoints, rightPoints);
                MeshFilter.sharedMesh = mesh;
            }
        }
        private Mesh LinkLeftRightPointsMesh(List<Vector3> left, List<Vector3> right)
        {
            Mesh ret = new Mesh
            {
                name = name
            };
            var count = left.Count + right.Count - 2;
            var lastLeft = left.First();
            var lastRight = right.First();
            var lastLeftIndex = 1;
            var lastRightIndex = 0;
            var leftCount = 1;
            var rightCount = 1;
            List<Vector3> points = new List<Vector3> { lastRight, lastLeft };
            List<int> indices = new List<int>();
            for (int i = 0; i < count; i++)
            {
                float dl = 0, dr = 0;
                bool addLeft = true, addRight = true;
                if (left.Count > leftCount)
                {
                    dl = Vector3.Distance(left.ElementAt(leftCount), lastRight);
                }
                else
                {
                    addLeft = false;
                }
                if (right.Count > rightCount)
                {
                    dr = Vector3.Distance(right.ElementAt(rightCount), lastLeft);
                }
                else
                {
                    addRight = false;
                }
                if (addLeft || addRight)
                {
                    indices.Add(lastRightIndex);
                    indices.Add(lastLeftIndex);
                }
                else
                {
                    return ret;
                }
                if (addLeft && addRight)
                {
                    if (dl > dr)
                    {
                        addLeft = false;
                    }
                }
                if (addLeft)
                {
                    lastLeft = left.ElementAt(leftCount++);
                    lastLeftIndex = i + 2;
                    points.Add(lastLeft);
                    indices.Add(lastLeftIndex);
                }
                else
                {
                    lastRight = right.ElementAt(rightCount++);
                    lastRightIndex = i + 2;
                    points.Add(lastRight);
                    indices.Add(lastRightIndex);
                }
            }
            ret.SetVertices(points);
            ret.SetIndices(indices, MeshTopology.Triangles, 0);
            return ret;
        }
        public bool CanDuplicateLeft => left.Way.Nodes.Count > 1 && left.OnlyUsedBy(Relation);
        public bool CanDuplicateRight => right.Way.Nodes.Count > 1 && right.OnlyUsedBy(Relation);
        internal void DuplicateLeft()
        {
            if (CanDuplicateLeft)
            {
                var lanelet = Lanelet2Map.AddChildGameObject<Lanelet>(Lanelet2Map.transform.childCount.ToString());
                lanelet.gameObject.RecordUndoCreateGo();
                lanelet.left = LineThin.AddNew(Lanelet2Map);
                lanelet.right = left;
                lanelet.left.Way.Ref.Add(lanelet.Relation);
                lanelet.right.Way.Ref.Add(lanelet.Relation);
                lanelet.left.DuplicateNodes(right, left);
                UpdateRenderer();
            }
        }
        internal void DuplicateRight()
        {
            if (CanDuplicateRight)
            {
                var lanelet = Lanelet2Map.AddChildGameObject<Lanelet>(Lanelet2Map.transform.childCount.ToString());
                lanelet.gameObject.RecordUndoCreateGo();
                lanelet.right = LineThin.AddNew(Lanelet2Map);
                lanelet.left = right;
                lanelet.left.Way.Ref.Add(lanelet.Relation);
                lanelet.right.Way.Ref.Add(lanelet.Relation);
                lanelet.right.DuplicateNodes(left, right);
                UpdateRenderer();
            }
        }
        internal void SelectLineThin() => Selection.objects = new[] { left.gameObject, right.gameObject };
    }
    [CustomEditor(typeof(Lanelet))]
    class LaneletEditor : Editor
    {
        Lanelet Target => target as Lanelet;
        private void OnEnable() => Target.OnEditorEnable();
        private void OnDisable() => Target.OnEditorDisable();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Tools.current = Tool.None;
            if (GUILayout.Button("Select Line Thin"))
            {
                Target.SelectLineThin();
            }
            if (Target.CanDuplicateLeft)
            {
                if (GUILayout.Button("Duplicate Left"))
                {
                    Target.DuplicateLeft();
                }
            }
            if (Target.CanDuplicateRight)
            {
                if (GUILayout.Button("Duplicate Right"))
                {
                    Target.DuplicateRight();
                }
            }
        }
    }
}