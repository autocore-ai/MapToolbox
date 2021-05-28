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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    public class Way : Member
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
                                gameObject.AddComponent<LineThin>();
                                break;
                            case "stop_line":
                                gameObject.AddComponent<StopLine>();
                                break;
                            case "traffic_sign":
                                gameObject.AddComponent<TrafficSign>();
                                break;
                            case "traffic_light":
                                gameObject.AddComponent<TrafficLight>();
                                break;
                            case "light_bulbs":
                                gameObject.AddComponent<LightBulbs>();
                                break;
                            case "parking_space":
                                gameObject.AddComponent<ParkingSpace>();
                                break;
                            case "parking_lot":
                                gameObject.AddComponent<ParkingLot>();
                                break;
                            case "":
                                extermTags.Add(new Tag(tag));
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
                                var lineThin = gameObject.GetComponent<LineThin>();
                                if (lineThin)
                                {
                                    lineThin.subType = LineThin.SubType.solid;
                                }
                                break;
                            case "dashed":
                                lineThin = gameObject.GetComponent<LineThin>();
                                if (lineThin)
                                {
                                    lineThin.subType = LineThin.SubType.dashed;
                                }
                                break;
                            case "stop_sign":
                                break;
                            case "":
                                extermTags.Add(new Tag(tag));
                                break;
                            default:
                                extermTags.Add(new Tag(tag));
                                Debug.LogWarning($"Unsupported Way subtype {tag.Attributes["v"].Value} on {name}");
                                break;
                        }
                        break;
                    case "height":
                        var traffic_light = gameObject.GetComponent<TrafficLight>();
                        if (traffic_light)
                        {
                            traffic_light.height = float.Parse(tag.Attributes["v"].Value);
                        }
                        break;
                    case "width":
                        var parking_space = gameObject.GetComponent<ParkingSpace>();
                        if (parking_space)
                        {
                            parking_space.Width = float.Parse(tag.Attributes["v"].Value);
                        }
                        break;
                    case "area":
                        extermTags.Add(new Tag(tag));
                        break;
                    case "traffic_light_id":
                        extermTags.Add(new Tag(tag));
                        break;
                    default:
                        Debug.LogWarning($"Unsupported Way tag {tag.Attributes["k"].Value} on {name}");
                        break;
                }
            }
        }
        internal XmlElement Save(XmlDocument doc)
        {
            XmlElement way = doc.CreateElement("way");
            way.SetAttribute("id", name);
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
            var stop_line = GetComponent<StopLine>();
            if (stop_line)
            {
                way.AppendChild(doc.AddTag("type", "stop_line"));
                way.AppendChild(doc.AddTag("subtype", "solid"));
            }
            var traffic_sign = GetComponent<TrafficSign>();
            if (traffic_sign)
            {
                way.AppendChild(doc.AddTag("type", "traffic_sign"));
                way.AppendChild(doc.AddTag("subtype", "stop_sign"));
            }
            var traffic_light = GetComponent<TrafficLight>();
            if (traffic_light)
            {
                way.AppendChild(doc.AddTag("type", "traffic_light"));
                way.AppendChild(doc.AddTag("height", traffic_light.height.ToString()));
            }
            var parking_lot = GetComponent<ParkingLot>();
            if (parking_lot)
            {
                way.AppendChild(doc.AddTag("type", "parking_lot"));
                way.AppendChild(doc.AddTag("area", "yes"));
            }
            var parking_space = GetComponent<ParkingSpace>();
            if (parking_space)
            {
                way.AppendChild(doc.AddTag("type", "parking_space"));
                way.AppendChild(doc.AddTag("width", parking_space.width.ToString()));
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
        internal Node InsertNode(Node node, int index = int.MaxValue)
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
            return node;
        }
        internal Node InsertNode(Vector3 position, int index = int.MaxValue)
        {
            var node = Node.AddNew(Lanelet2Map, position);
            return InsertNode(node, index);
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
            UnRegistNode(tmp);
            OnRemoveNode?.Invoke(tmp);
            nodes.RemoveAt(index);
            Undo.DestroyObjectImmediate(tmp.gameObject);
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
#if UNITY_EDITOR
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
#endif
}