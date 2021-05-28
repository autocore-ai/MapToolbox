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

using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox
{
    [RequireComponent(typeof(IAddOrRemoveTarget))]
    public class AddOrRemovable : MonoBehaviour
    {
        IAddOrRemoveTarget addOrRemoveTarget;
        internal void OnEditorEnable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
            SceneView.duringSceneGui += DuringSceneGui;
        }
        internal void OnEditorDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGui;
        }
        private void DuringSceneGui(SceneView obj)
        {
            if (addOrRemoveTarget == null)
            {
                addOrRemoveTarget = GetComponent<IAddOrRemoveTarget>();
            }
            if (addOrRemoveTarget != null && EditorUpdate.MouseInSceneView)
            {
                if (EditorUpdate.MouseLeftButtonDownWithCtrl && EditorUpdate.MouseLeftButtonDownWithShift)
                {
                    addOrRemoveTarget.OnRemove();
                }
                else if (EditorUpdate.MouseLeftButtonDownWithCtrl)
                {
                    addOrRemoveTarget.OnAdd();
                }
            }
            else
            {
                addOrRemoveTarget.MouseEnterInspector();
            }
        }
    }
    [CustomEditor(typeof(AddOrRemovable))]
    class AddOrRemovableEditor : Editor
    {
        AddOrRemovable Target => target as AddOrRemovable;
        private void OnEnable() => Target.OnEditorEnable();
        private void OnDisable() => Target.OnEditorDisable();
    }
}