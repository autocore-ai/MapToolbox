﻿#region License
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
            left.Way.Ref.TryAdd(Relation);
            right.Way.Ref.TryAdd(Relation);
            Relation.Members.TryAdd(left.Way);
            Relation.Members.TryAdd(right.Way);
            UpdateRenderer();
        }
        private void OnNodeMoved(Node node) => UpdateRenderer();
        public MeshFilter MeshFilter => GetComponent<MeshFilter>() ?? gameObject.AddComponent<MeshFilter>();
        public MeshRenderer MeshRenderer => GetComponent<MeshRenderer>() ?? gameObject.AddComponent<MeshRenderer>();
        public Relation Relation => GetComponent<Relation>() ?? gameObject.AddComponent<Relation>();
        public LineThin left;
        public LineThin right;
        public float width = 3.75f;
        public enum TurnDirection
        {
            Null,
            Straight,
            Left,
            Right
        }
        public TurnDirection turnDirection = TurnDirection.Null;
        internal Lanelet AddNew() => AddNew(Lanelet2Map);
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
            left.RemoveNode();
            right.RemoveNode();
        }
        private void AddPoints()
        {
            if(left.Nodes.Count > 1 || right.Nodes.Count > 1)
            {
                var center = Utils.MousePointInSceneView;
                if (left.NearLastPoint(center) || right.NearLastPoint(center))
                {
                    center.y = 0;
                    var lastPoint = (left.Nodes.Last().Position + right.Nodes.Last().Position) / 2;
                    var leftPoint = center + Vector3.Cross(center - lastPoint, Vector3.up).normalized * width / 2;
                    var rightPoint = center + Vector3.Cross(center - lastPoint, Vector3.down).normalized * width / 2;
                    left.AddPointFinal(leftPoint);
                    right.AddPointFinal(rightPoint);
                }
                else
                {
                    center.y = 0;
                    var lastPoint = (left.Nodes.First().Position + right.Nodes.First().Position) / 2;
                    var leftPoint = center - Vector3.Cross(center - lastPoint, Vector3.up).normalized * width / 2;
                    var rightPoint = center - Vector3.Cross(center - lastPoint, Vector3.down).normalized * width / 2;
                    left.AddPointFirst(leftPoint);
                    right.AddPointFirst(rightPoint);
                }
            }
            else if (left.Nodes.Count == 1 || right.Nodes.Count == 1)
            {
                var center = Utils.MousePointInSceneView;
                center.y = 0;
                var lastPoint = (left.Nodes.Last().Position + right.Nodes.Last().Position) / 2;
                left.SetOrAddPointFinal(lastPoint + Vector3.Cross(center - lastPoint, Vector3.up).normalized * width / 2);
                right.SetOrAddPointFinal(lastPoint + Vector3.Cross(center - lastPoint, Vector3.down).normalized * width / 2);
                left.AddPointFinal(center + Vector3.Cross(center - lastPoint, Vector3.up).normalized * width / 2);
                right.AddPointFinal(center + Vector3.Cross(center - lastPoint, Vector3.down).normalized * width / 2);
            }
            else if (left.Nodes.Count == 0 && right.Nodes.Count == 0)
            {
                var center = Utils.MousePointInSceneView;
                center.y = 0;
                left.AddPointFirst(center);
                right.AddPointFirst(center);
            }
        }
        internal void OnEditorEnable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            SceneView.duringSceneGui += DuringSceneGui;
            foreach (var item in Relation.Members)
            {
                if (item is Relation)
                {
                    var re = item.GetComponent<RegulatoryElement>();
                    if (re)
                    {
                        re.drawGizmos = true;
                    }
                }
            }
        }
        internal void OnEditorDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            if (destroyed)
            {
                return;
            }
            if (gameObject != null)
            {
                SceneVisibilityManager.instance.EnablePicking(gameObject, true);
            }
            foreach (var item in Relation.Members)
            {
                if (item is Relation)
                {
                    var re = item.GetComponent<RegulatoryElement>();
                    if (re)
                    {
                        re.drawGizmos = false;
                    }
                }
            }
        }
        private void OnDestroy() => destroyed = true;
        private void DuringSceneGui(SceneView obj)
        {
            if (EditorUpdate.MouseInSceneView)
            {
                if (EditorUpdate.MouseLeftButtonDownWithCtrl && EditorUpdate.MouseLeftButtonDownWithShift)
                {
                    RemovePoints();
                    UpdateRenderer();
                    SceneVisibilityManager.instance.DisablePicking(gameObject, true);
                }
                else if (EditorUpdate.MouseLeftButtonDownWithCtrl)
                {
                    AddPoints();
                    UpdateRenderer();
                    SceneVisibilityManager.instance.DisablePicking(gameObject, true);
                }
            }
        }
        public enum ReversedMode
        {
            None,
            Left,
            Right,
            All
        }
        public ReversedMode CurrentReversedMode = ReversedMode.None;
        private bool destroyed;
        internal void UpdateRenderer()
        {
            MeshFilter.sharedMesh?.Clear();
            if (left.Way.Nodes.Count > 1 || right.Way.Nodes.Count > 1)
            {
                List<Vector3> leftPoints = left.Way.Nodes.Select(_ => _.Position).ToList();
                List<Vector3> rightPoints = right.Way.Nodes.Select(_ => _.Position).ToList();
                MeshFilter.sharedMesh = LinkLeftRightPointsMesh(leftPoints, rightPoints);
                for (int i = 0; i < 4; i++)
                {
                    if (NeedReUpdateRenderer(ref leftPoints, ref rightPoints))
                    {
                        MeshFilter.sharedMesh.Clear();
                        MeshFilter.sharedMesh = LinkLeftRightPointsMesh(leftPoints, rightPoints);
                    }
                    else
                    {
                        break;
                    }
                }
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
        private bool NeedReUpdateRenderer(ref List<Vector3> leftPoints, ref List<Vector3> rightPoints)
        {
            List<Vector3> normals = new List<Vector3>();
            MeshFilter.sharedMesh.RecalculateNormals();
            MeshFilter.sharedMesh.GetNormals(normals);
            if (normals.Count > 1)
            {
                for (int i = 0; i < normals.Count; i++)
                {
                    if (Vector3.Angle(Vector3.up, normals[i]) > 90)
                    {
                        switch (CurrentReversedMode)
                        {
                            case ReversedMode.None:
                                CurrentReversedMode = ReversedMode.Left;
                                leftPoints.Reverse();
                                return true;
                            case ReversedMode.Left:
                                CurrentReversedMode = ReversedMode.Right;
                                leftPoints.Reverse();
                                rightPoints.Reverse();
                                return true;
                            case ReversedMode.Right:
                                CurrentReversedMode = ReversedMode.All;
                                leftPoints.Reverse();
                                return true;
                            case ReversedMode.All:
                                CurrentReversedMode = ReversedMode.None;
                                leftPoints.Reverse();
                                rightPoints.Reverse();
                                return true;
                            default:
                                return false;
                        }
                    }
                }
            }
            return false;
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
            if (GUILayout.Button("Add Lanelet"))
            {
                Selection.activeObject = Target.AddNew();
            }
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
            if (GUILayout.Button("UpdateRenderer"))
            {
                Target.UpdateRenderer();
            }
        }
    }
}