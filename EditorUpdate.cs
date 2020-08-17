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
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    class EditorUpdate : Singleton<EditorUpdate>
    {
        public static bool MouseInSceneView { get; set; }
        public static bool CtrlKey { get; set; }
        public static bool ShiftKey { get; set; }
        public static bool MouseLeftButtonDownWithCtrl { get; set; }
        public static bool MouseLeftButtonDownWithShift { get; set; }
        private void OnEnable()
        {
            name = typeof(EditorUpdate).Name;
            EditorApplication.update -= Update;
            SceneView.duringSceneGui -= DuringSceneGui;
            EditorApplication.update += Update;
            SceneView.duringSceneGui += DuringSceneGui;
        }
        private void OnDisable()
        {
            EditorApplication.update -= Update;
            SceneView.duringSceneGui -= DuringSceneGui;
        }
        private void DuringSceneGui(SceneView obj)
        {
            MouseLeftButtonDownWithCtrl = false;
            MouseLeftButtonDownWithShift = false;
            switch (Event.current.type)
            {
                case EventType.Layout:
                    CtrlKey = Event.current.control;
                    ShiftKey = Event.current.shift;
                    break;
                case EventType.MouseEnterWindow:
                    MouseInSceneView = true;
                    break;
                case EventType.MouseLeaveWindow:
                    MouseInSceneView = false;
                    break;
                case EventType.MouseDown:
                    if (Event.current.control && Event.current.button == 0)
                    {
                        MouseLeftButtonDownWithCtrl = true;
                    }
                    if(Event.current.shift && Event.current.button == 0)
                    {
                        MouseLeftButtonDownWithShift = true;
                    }
                    break;
                default:
                    break;
            }
        }
        public List<UnityAction> OnUpdate { get; private set; } = new List<UnityAction>();
        private void Update()
        {
            foreach (var item in OnUpdate)
            {
                item?.Invoke();
            }
        }
    }
}