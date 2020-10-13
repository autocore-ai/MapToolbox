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

namespace Packages.MapToolbox
{
    class Area : WayTypeBase<Area>
    {
        public enum SubType
        {
            None,
            Floors,
            Kerbs,
            Columns,
            Walls,
            Windows,
            Doors,
            parking_access,
            parking_accesses,
            parking_spot,
            junction
        }
        [SerializeField] SubType subType = SubType.None;
        public SubType Type
        {
            get => subType;
            set
            {
                switch (value)
                {
                    case SubType.None:
                        break;
                    case SubType.Floors:
                        break;
                    case SubType.Kerbs:
                        LineRenderer.startColor = LineRenderer.endColor = Color.blue;
                        break;
                    case SubType.Columns:
                        break;
                    case SubType.Walls:
                        break;
                    case SubType.Windows:
                        break;
                    case SubType.Doors:
                        break;
                    case SubType.parking_access:
                    case SubType.parking_accesses:
                    case SubType.parking_spot:
                        LineRenderer.startColor = LineRenderer.endColor = Color.magenta;
                        break;
                    case SubType.junction:
                        break;
                    default:
                        break;
                }
                subType = value;
            }
        }
        protected override void Start()
        {
            base.Start();
            LineRenderer.startWidth = LineRenderer.endWidth = 0.2f;
        }
    }
    class Area<T> : WayTypeBase<T> where T : Area<T>
    {
        private bool destroyed;

        internal void OnEditorEnable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            SceneView.duringSceneGui += DuringSceneGui;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            destroyed = true;
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
        }
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
        private void AddPoints()
        {
            var point = Utils.MousePointInSceneView;
            point.y = 0;
            Way.InsertNode(point);
        }
        private void RemovePoints()
        {
            if (Way.Nodes.Count > 0)
            {
                Way.RemoveNode(Way.Nodes.Count - 1);
            }
        }
    }
    class AreaEditor<T> : Editor where T:Area<T>
    {
        T Target => target as T;
        private void OnEnable() => Target.OnEditorEnable();
        private void OnDisable() => Target.OnEditorDisable();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Tools.current = Tool.None;
            if (GUILayout.Button("UpdateRenderer"))
            {
                Target.UpdateRenderer();
            }
        }
    }
}