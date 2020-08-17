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
    class Way : Member
    {
        [SerializeField] List<Node> nodes = new List<Node>();
        public List<Node> Nodes => nodes;
        public UnityAction<int, Vector3> OnNodeMoved { get; set; }
        public UnityAction<int, Vector3> OnAddNode { get; set; }
        Lanelet2Map Lanelet2Map => GetComponentInParent<Lanelet2Map>();
        protected void Start()
        {
            nodes.RemoveNull();
            foreach (var item in nodes)
            {
                item.Ref.RemoveNull();
                item.Ref.TryAdd(this);
                item.OnNodeMoved -= OnNodeMovedAction;
                item.OnNodeMoved += OnNodeMovedAction;
            }
        }
        private void OnNodeMovedAction(Node node) => OnNodeMoved?.Invoke(nodes.IndexOf(node), node.Position);
        internal void Load(XmlNode xmlNode)
        {
            name = xmlNode.Attributes["id"].Value;
            nodes.RemoveNull();
            foreach (XmlNode nd in xmlNode.SelectNodes("nd"))
            {
                nodes.Add(Lanelet2Map.transform.Find(nd.Attributes["ref"].Value).GetComponent<Node>());
            }
            foreach (XmlNode tag in xmlNode.SelectNodes("tag"))
            {
                switch (tag.Attributes["k"].Value)
                {
                    case "type":
                        switch (tag.Attributes["v"].Value)
                        {
                            case "line_thin":
                                gameObject.AddComponent<LineThin>();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        internal XmlElement Save(XmlDocument xmlDocument)
        {
            XmlElement way = xmlDocument.CreateElement("way");
            way.SetAttribute("id", name);
            nodes.RemoveNull();
            foreach (var item in nodes)
            {
                XmlElement nd = xmlDocument.CreateElement("nd");
                nd.SetAttribute("ref", item.name);
                way.AppendChild(nd);
            }
            var line_thin = GetComponent<LineThin>();
            if (line_thin)
            {
                XmlElement type = xmlDocument.CreateElement("tag");
                type.SetAttribute("k", "type");
                type.SetAttribute("v", "line_thin");
                way.AppendChild(type);
                XmlElement subType = xmlDocument.CreateElement("tag");
                subType.SetAttribute("k", "subtype");
                subType.SetAttribute("v", line_thin.subType.ToString());
                way.AppendChild(subType);
            }
            else
            {
                var stop_line = GetComponent<StopLine>();
                if (stop_line)
                {
                    XmlElement type = xmlDocument.CreateElement("tag");
                    type.SetAttribute("k", "type");
                    type.SetAttribute("v", "stop_line");
                    way.AppendChild(type);
                    XmlElement subType = xmlDocument.CreateElement("tag");
                    subType.SetAttribute("k", "subtype");
                    subType.SetAttribute("v", "solid");
                    way.AppendChild(subType);
                }
                else
                {
                    var traffic_sign = GetComponent<TrafficSign>();
                    if (traffic_sign)
                    {
                        XmlElement type = xmlDocument.CreateElement("tag");
                        type.SetAttribute("k", "type");
                        type.SetAttribute("v", "traffic_sign");
                        way.AppendChild(type);
                        XmlElement subType = xmlDocument.CreateElement("tag");
                        subType.SetAttribute("k", "subtype");
                        subType.SetAttribute("v", "stop_sign");
                        way.AppendChild(subType);
                    }
                    else
                    {
                        var traffic_light = GetComponent<TrafficLight>();
                        if (traffic_light)
                        {
                            XmlElement type = xmlDocument.CreateElement("tag");
                            type.SetAttribute("k", "type");
                            type.SetAttribute("v", "traffic_light");
                            way.AppendChild(type);
                            XmlElement subType = xmlDocument.CreateElement("tag");
                            subType.SetAttribute("k", "height");
                            subType.SetAttribute("v", traffic_light.height.ToString());
                            way.AppendChild(subType);
                        }
                    }
                }
            }
            return way;
        }
        internal void EditPoints()
        {
            if (nodes.Count > 0)
            {
                var originPositions = nodes.Select(_ => _.transform.position);
                for (int i = 0; i < originPositions.Count(); i++)
                {
                    var oldPosition = originPositions.ElementAt(i);
                    var newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);
                    if (!newPosition.Equals(oldPosition))
                    {
                        nodes[i].Position = newPosition;
                    }
                }
            }
        }
        private void OnNodeDestroyed(Node node)
        {
            node.OnDestroyed -= OnNodeDestroyed;
            node.OnNodeMoved -= OnNodeMovedAction;
            nodes.Remove(node);
        }
        internal void InsertNode(Vector3 position, int index = int.MaxValue)
        {
            var node = Node.AddNew(Lanelet2Map, position);
            node.OnNodeMoved += OnNodeMovedAction;
            node.OnDestroyed += OnNodeDestroyed;
            node.Ref.Add(this);
            Undo.RegisterCreatedObjectUndo(node, "add node");
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= Nodes.Count)
            {
                index = nodes.Count;
            }
            nodes.Insert(index, node);
            OnAddNode?.Invoke(index, position);
        }
        internal void RemoveNode(int index)
        {
            if (nodes.Count == 0)
            {
                return;
            }
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= nodes.Count)
            {
                index = nodes.Count - 1;
            }
            var tmp = nodes[index];
            nodes.RemoveAt(index);
            Undo.DestroyObjectImmediate(tmp.gameObject);
        }
    }
    [CustomEditor(typeof(Way))]
    [CanEditMultipleObjects]
    class WayEditor : Editor
    {
        List<Way> Targets = new List<Way>();
        private void OnEnable()
        {
            if (targets.Length > 0)
            {
                foreach (Way item in targets)
                {
                    Targets.Add(item);
                }
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Targets.Count > 1)
            {
                bool contains_stop_line = Targets.Any(_ => _.GetComponent<StopLine>() != null);
                if (contains_stop_line)
                {
                    bool contains_traffic_light = Targets.Any(_ => _.GetComponent<TrafficLight>() != null);
                    bool contains_traffic_sign = Targets.Any(_ => _.GetComponent<TrafficSign>() != null);
                    if (contains_traffic_light)
                    {
                        if (GUILayout.Button("Add traffic light regulatory element"))
                        {
                            var stop_line = Targets.Select(_ => _.GetComponent<StopLine>()).Where(_ => _ != null).First();
                            var re = RegulatoryElement.AddNew(Targets.First().GetComponentInParent<Lanelet2Map>());
                            re.subType = RegulatoryElement.SubType.traffic_light;
                            re.ref_line = Targets.Where(_ => _.GetComponent<StopLine>() != null).First();
                            re.refers = Targets.Where(_ => _.GetComponent<TrafficLight>() != null).First();
                            re.Relation.Members.Add(re.ref_line);
                            re.Relation.Members.Add(re.refers);
                            var relation = stop_line.GetLanelet();
                            if (relation)
                            {
                                relation.Relation.Members.Add(re.Relation);
                            }
                        }
                    }
                    if (contains_traffic_sign)
                    {
                        if (GUILayout.Button("Add traffic sign regulatory element"))
                        {
                            var stop_line = Targets.Select(_ => _.GetComponent<StopLine>()).Where(_ => _ != null).First();
                            var re = RegulatoryElement.AddNew(Targets.First().GetComponentInParent<Lanelet2Map>());
                            re.subType = RegulatoryElement.SubType.traffic_sign;
                            re.ref_line = Targets.Where(_ => _.GetComponent<StopLine>() != null).First();
                            re.refers = Targets.Where(_ => _.GetComponent<TrafficSign>() != null).First();
                            re.Relation.Members.Add(re.ref_line);
                            re.Relation.Members.Add(re.refers);
                            var relation = stop_line.GetLanelet();
                            if (relation)
                            {
                                relation.Relation.Members.Add(re.Relation);
                            }
                        }
                    }
                }
            }
        }
        private void OnSceneGUI()
        {
            if (Targets.Count > 0)
            {
                Tools.current = Tool.None;
                foreach (Way item in Targets)
                {
                    item.EditPoints();
                }
            }
        }
    }
}