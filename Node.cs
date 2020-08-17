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
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    class Node : Member
    {
        public UnityAction<Node> OnNodeMoved { get; set; }
        public UnityAction<Node> OnDestroyed { get; set; }
        internal Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                OnNodeMoved?.Invoke(this);
            }
        }
        internal static Node AddNew(Lanelet2Map map, Vector3 position)
        {
            var ret = map.AddChildGameObject<Node>(map.transform.childCount.ToString());
            ret.Position = position;
            ret.gameObject.RecordUndoCreateGo();
            return ret;
        }
        internal static Node GetNearestNode(Node target)
        {
            var nodes = target.GetComponentInParent<Lanelet2Map>().GetComponentsInChildren<Node>().ToList();
            Node ret = null;
            float distance = float.MaxValue;
            for (int i = 0; i < nodes.Count; i++)
            {
                var nd = nodes[i];
                if (nd != target)
                {
                    var dis = Vector3.Distance(target.Position, nd.Position);
                    if (dis < distance)
                    {
                        distance = dis;
                        ret = nd;
                    }
                }
            }
            return ret;
        }
        private void Start() => gameObject.SetIcon(IconManager.ShapeIcon.CircleGray);
        protected void OnDestroy() => OnDestroyed?.Invoke(this);
        internal void Load(XmlNode xmlNode)
        {
            name = xmlNode.Attributes["id"].Value;
            foreach (XmlNode tag in xmlNode.SelectNodes("tag"))
            {
                switch (tag.Attributes["k"].Value)
                {
                    case "ele":
                        transform.SetLocalY(float.Parse(tag.Attributes["v"].Value));
                        break;
                    case "local_x":
                        transform.SetLocalX(float.Parse(tag.Attributes["v"].Value));
                        break;
                    case "local_y":
                        transform.SetLocalZ(float.Parse(tag.Attributes["v"].Value));
                        break;
                    default:
                        break;
                }
            }
        }
        internal XmlElement Save(XmlDocument xmlDocument)
        {
            XmlElement node = xmlDocument.CreateElement("node");
            node.SetAttribute("id", name);
            node.SetAttribute("lat", "0");
            node.SetAttribute("lon", "0");
            XmlElement ele = xmlDocument.CreateElement("tag");
            ele.SetAttribute("k", "ele");
            ele.SetAttribute("v", transform.localPosition.y.ToString());
            node.AppendChild(ele);
            XmlElement local_x = xmlDocument.CreateElement("tag");
            local_x.SetAttribute("k", "local_x");
            local_x.SetAttribute("v", transform.localPosition.x.ToString());
            node.AppendChild(local_x);
            XmlElement local_y = xmlDocument.CreateElement("tag");
            local_y.SetAttribute("k", "local_y");
            local_y.SetAttribute("v", transform.localPosition.z.ToString());
            node.AppendChild(local_y);
            return node;
        }
        internal void RemoveRef(Member member)
        {
            Ref.TryRemove(member);
            if (Ref.Count == 0)
            {
                Undo.DestroyObjectImmediate(gameObject);
            }
        }
        internal void Merge(Node node)
        {
            Position = node.Position;
        }
    }
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Node))]
    class NodeEditor : Editor
    {
        const int maxMultiEditorCount = 100;
        const float attachDistance = 1;
        List<Node> Targets = new List<Node>();
        static List<Node> SelectList { get; set; } = new List<Node>();
        private void OnEnable()
        {
            foreach (Node item in targets)
            {
                Targets.Add(item);
            }
            if (targets.Length == 1)
            {
                SelectList = Targets;
            }
            else if (targets.Length == SelectList.Count + 1)
            {
                var added = Targets.Where(_ => !SelectList.Contains(_)).First();
                SelectList.Add(added);
            }
            else if (targets.Length == SelectList.Count - 1)
            {
                var removed = SelectList.Where(_ => !Targets.Contains(_)).First();
                SelectList.Remove(removed);
            }
        }
        private void OnSceneGUI()
        {
            Tools.current = Tool.None;
            if (Targets.Count > 0 && Targets.Count < maxMultiEditorCount)
            {
                Node movingNode = null;
                foreach (var item in Targets)
                {
                    var pos = Handles.PositionHandle(item.Position, item.transform.rotation);
                    if (!pos.Equals(item.Position))
                    {
                        movingNode = item;
                        movingNode.Position = pos;
                    }
                }
                if (Targets.Count > 1 && movingNode)
                {
                    var nearestNode = Targets.OrderBy(_ => Vector3.Distance(movingNode.Position, _.Position)).ElementAt(1);
                    if (Vector3.Distance(nearestNode.Position, movingNode.Position) < attachDistance)
                    {
                        movingNode.Merge(nearestNode);
                    }
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Targets.Count == 2)
            {
                if (GUILayout.Button("Link Stop Line"))
                {
                    StopLine stopLine = StopLine.AddNew(Targets[0].GetComponentInParent<Lanelet2Map>());
                    stopLine.Way.Nodes.Add(Targets[0]);
                    stopLine.Way.Nodes.Add(Targets[1]);
                    Selection.activeObject = stopLine;
                }
                if (GUILayout.Button("Link Traffic Light"))
                {
                    TrafficLight trafficLight = TrafficLight.AddNew(Targets[0].GetComponentInParent<Lanelet2Map>());
                    trafficLight.Way.Nodes.Add(Targets[0]);
                    trafficLight.Way.Nodes.Add(Targets[1]);
                    Selection.activeObject = trafficLight;
                }
                if (GUILayout.Button("Link Traffic Sign"))
                {
                    TrafficSign trafficSign = TrafficSign.AddNew(Targets[0].GetComponentInParent<Lanelet2Map>());
                    trafficSign.Way.Nodes.Add(Targets[0]);
                    trafficSign.Way.Nodes.Add(Targets[1]);
                    Selection.activeObject = trafficSign;
                }
            }
            else if (Targets.Count > 2)
            {
            }
        }
    }
}