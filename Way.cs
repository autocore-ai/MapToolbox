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
        public UnityAction<Node> OnNodeMoved { get; set; }
        public UnityAction<Node> OnAddNode { get; set; }
        public UnityAction<Node> OnRemoveNode { get; set; }
        public List<Tag> extermTags = new List<Tag>();
        Lanelet2Map Lanelet2Map => GetComponentInParent<Lanelet2Map>();
        protected void Start()
        {
            nodes.RemoveNull();
            foreach (var item in nodes)
            {
                item.Ref.RemoveNull();
                item.Ref.TryAdd(this);
                UnRegistNode(item);
                RegistNode(item);
            }
        }
        private void OnDestroy()
        {
            nodes.RemoveNull();
            foreach (var item in nodes)
            {
                item.Ref.TryRemove(this);
            }
        }
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
                                gameObject.GetOrAddComponent<LineThin>();
                                break;
                            case "stop_line":
                                gameObject.GetOrAddComponent<StopLine>();
                                break;
                            case "traffic_sign":
                                gameObject.GetOrAddComponent<TrafficSign>();
                                break;
                            case "traffic_light":
                                gameObject.GetOrAddComponent<TrafficLight>();
                                break;
                            case "light_bulbs":
                                gameObject.GetOrAddComponent<LightBulbs>();
                                break;
                            case "parking_space":
                                gameObject.GetOrAddComponent<ParkingSpot>();
                                break;
                            case "parking_lot":
                                gameObject.GetOrAddComponent<ParkingLot>();
                                break;
                            case "area":
                                break;
                            default:
                                extermTags.Add(new Tag(tag));
                                Debug.LogWarning($"Unsupported Way type {tag.Attributes["v"].Value} on {name}");
                                break;
                        }
                        break;
                    case "subtype":
                        switch (tag.Attributes["v"].Value)
                        {
                            case "solid":
                                gameObject.GetOrAddComponent<LineThin>().subType = LineThin.SubType.solid;
                                break;
                            case "dashed":
                                gameObject.GetOrAddComponent<LineThin>().subType = LineThin.SubType.dashed;
                                break;
                            case "stop_sign":
                                break;
                            case "parking_access":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.parking_access;
                                break;
                            case "parking_accesses":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.parking_accesses;
                                break;
                            case "parking_spot":
                                gameObject.GetOrAddComponent<ParkingSpot>();
                                //gameObject.GetOrAddComponent<Area>().Type = Area.SubType.parking_spot;
                                break;
                            case "Floors":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.Floors;
                                break;
                            case "Kerbs":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.Kerbs;
                                break;
                            case "Columns":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.Columns;
                                break;
                            case "Walls":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.Walls;
                                break;
                            case "Windows":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.Windows;
                                break;
                            case "Doors":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.Doors;
                                break;
                            case "junction":
                                gameObject.GetOrAddComponent<Area>().Type = Area.SubType.junction;
                                break;
                            default:
                                extermTags.Add(new Tag(tag));
                                Debug.LogWarning($"Unsupported Way subtype {tag.Attributes["v"].Value} on {name}");
                                break;
                        }
                        break;
                    case "height":
                        gameObject.GetOrAddComponent<TrafficLight>().height = tag.Attributes["v"].ToFloat();
                        break;
                    case "width":
                        gameObject.GetOrAddComponent<ParkingSpot>().Width = tag.Attributes["v"].ToFloat();
                        break;
                    case "area":
                    case "traffic_light_id":
                    case "cad_id":
                    case "drivable":
                    case "geom_height":
                    case "level":
                    case "parking_accesses":
                    case "parking_spots":
                    case "center":
                    case "ref_lanelet":
                    case "poi_type":
                        extermTags.Add(new Tag(tag));
                        break;
                    default:
                        extermTags.Add(new Tag(tag));
                        Debug.LogWarning($"Unsupported Way tag {tag.Attributes["k"].Value} on {name}");
                        break;
                }
            }
        }
        internal XmlElement Save(XmlDocument doc)
        {
            XmlElement way = doc.CreateElement("way");
            way.SetAttribute("id", name);
            way.SetAttribute("visible", "true");
            way.SetAttribute("version", "1");
            nodes.RemoveNull();
            foreach (var item in nodes)
            {
                XmlElement nd = doc.CreateElement("nd");
                nd.SetAttribute("ref", item.name);
                way.AppendChild(nd);
            }
            var line_thin = GetComponent<LineThin>();
            if (line_thin)
            {
                way.AppendChild(doc.AddTag("type", "line_thin"));
                way.AppendChild(doc.AddTag("subtype", line_thin.subType.ToString()));
            }
            else
            {
                var stop_line = GetComponent<StopLine>();
                if (stop_line)
                {
                    way.AppendChild(doc.AddTag("type", "stop_line"));
                    way.AppendChild(doc.AddTag("subtype", "solid"));
                }
                else
                {
                    var traffic_sign = GetComponent<TrafficSign>();
                    if (traffic_sign)
                    {
                        way.AppendChild(doc.AddTag("type", "traffic_sign"));
                        way.AppendChild(doc.AddTag("subtype", "stop_sign"));
                    }
                    else
                    {
                        var traffic_light = GetComponent<TrafficLight>();
                        if (traffic_light)
                        {
                            way.AppendChild(doc.AddTag("type", "traffic_light"));
                            way.AppendChild(doc.AddTag("height", traffic_light.height.ToString()));
                        }
                    }
                }
            }
            foreach (var item in extermTags)
            {
                way.AppendChild(doc.AddTag(item.k, item.v));
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
        internal void InsertNode(Node node, int index = int.MaxValue)
        {
            RegistNode(node);
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
            OnAddNode?.Invoke(node);
        }
        internal void InsertNode(Vector3 position, int index = int.MaxValue)
        {
            var node = Node.AddNew(Lanelet2Map, position);
            InsertNode(node, index);
        }
        internal void RemoveNode(Node node)
        {
            if (nodes.Contains(node))
            {
                node.Ref.TryRemove(this);
                UnRegistNode(node);
                OnRemoveNode?.Invoke(node);
                nodes.Remove(node);
                Undo.DestroyObjectImmediate(node.gameObject);
            }
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
            RemoveNode(nodes[index]);
        }
        private void RegistNode(Node node)
        {
            node.OnMoved += OnNodeMovedAction;
            node.OnMerged += OnNodeMerged;
            node.OnDestroyed += OnNodeDestroyed;
        }
        private void UnRegistNode(Node node)
        {
            node.OnMoved -= OnNodeMovedAction;
            node.OnMerged -= OnNodeMerged;
            node.OnDestroyed -= OnNodeDestroyed;
        }
        private void OnNodeMovedAction(Node node) => OnNodeMoved?.Invoke(node);
        private void OnNodeMerged(Node oldNode, Node newNode)
        {
            UnRegistNode(oldNode);
            RegistNode(newNode);
            var index = nodes.IndexOf(oldNode);
            nodes.Remove(oldNode);
            nodes.Insert(index, newNode);
        }
        private void OnNodeDestroyed(Node node)
        {
            UnRegistNode(node);
            OnRemoveNode?.Invoke(node);
            nodes.Remove(node);
        }
    }
    [CustomEditor(typeof(Way))]
    [CanEditMultipleObjects]
    class WayEditor : Editor
    {
        const int maxMultiEditorCount = 10;
        List<Way> Targets = new List<Way>();
        private void OnEnable()
        {
            if (targets.Length > maxMultiEditorCount)
            {
                return;
            }
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