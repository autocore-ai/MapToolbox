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

using UnityEditor;

namespace Packages.MapToolbox.VectorMapTools.Editor
{
    [CustomEditor(typeof(SignalLight))]
    class SignalLightEditor : UnityEditor.Editor
    {
        private void DuringSceneGUI(SceneView sv)
        {
            SignalLight t = target as SignalLight;
            var position = t.transform.position;
            var rotation = t.transform.rotation;
            Handles.TransformHandle(ref position, ref rotation);
            t.transform.position = position;
            t.transform.rotation = rotation;
            Undo.RecordObject(t.transform, "SignalLight");
        }
        void OnEnable() => SceneView.duringSceneGui += DuringSceneGUI;
        void OnDisable() => SceneView.duringSceneGui -= DuringSceneGUI;
    }
}