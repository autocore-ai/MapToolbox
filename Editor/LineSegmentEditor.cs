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


using AutoCore.MapToolbox.Autoware;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AutoCore.MapToolbox.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LineSegment))]
    class LineSegmentEditor : SiblingIndexEditor<LineSegment>
    {
        const string MultipleMove = "Multiple Move";
        IEnumerable<LineSegment> Selected { get; set; }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (targets.Length > 1)
            {
                var i = PlayerPrefs.GetInt(MultipleMove, 0);
                var t = GUILayout.Toggle(i > 0, MultipleMove);
                if (i > 0 != t)
                {
                    PlayerPrefs.SetInt(MultipleMove, t ? 1 : 0);
                }
                if (t)
                {
                    Selected = targets.Cast<LineSegment>();
                }
                else
                {
                    Selected = null;
                }
            }
        }
        protected virtual void OnSceneGUI()
        {
            Tools.current = Tool.None;
            var point = target as LineSegment;
            Handles.color = Color.cyan;
            Handles.ArrowHandleCap(0, point.From, Quaternion.FromToRotation(Vector3.forward, point.To - point.From), 1, EventType.Repaint);
            var pos1 = Handles.PositionHandle(point.From, Quaternion.identity);
            var pos2 = Handles.PositionHandle(point.To, Quaternion.identity);
            var move1 = pos1 - point.From;
            var move2 = pos2 - point.To;
            if (Selected?.Count() > 1)
            {
                if (!move1.Equals(Vector3.zero))
                {
                    foreach (var item in Selected)
                    {
                        item.From += move1;
                    }
                }
                if (!move2.Equals(Vector3.zero))
                {
                    foreach (var item in Selected)
                    {
                        item.To += move2;
                    }
                }
                if (!(move1.Equals(Vector3.zero) && move2.Equals(Vector3.zero)))
                {
                    foreach (var item in Selected)
                    {
                        item.UpdateLineRendererPosition();
                    }
                }
            }
            else
            {
                if (!move1.Equals(Vector3.zero))
                {
                    point.From += move1;
                }
                if (!move2.Equals(Vector3.zero))
                {
                    point.To += move2;
                }
                if (!(move1.Equals(Vector3.zero) && move2.Equals(Vector3.zero)))
                {
                    point.UpdateLineRendererPosition();
                }
            }
        }
        private void OnDestroy() => Selected = null;
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Line))]
    class LineEditor : LineSegmentEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(WhiteLine))]
    class WhiteLineEditor : LineEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(StopLine))]
    class StopLineEditor : LineEditor { }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Curb))]
    class CurbEditor : LineEditor { }
}