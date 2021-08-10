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
using System.Xml;
using UnityEngine;
namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    public class Relation : Member
    {
        [SerializeField] List<Member> members = new List<Member>();
        internal List<Member> Members => members;
        internal List<string> RelationId { get; set; } = new List<string>();
        Lanelet2Map Lanelet2Map => GetComponentInParent<Lanelet2Map>();
        internal void Load(XmlNode xmlNode)
        {
            name = xmlNode.Attributes["id"].Value;
            members.RemoveNull();
            foreach (XmlNode tag in xmlNode.SelectNodes("tag"))
            {
                switch (tag.Attributes["k"].Value)
                {
                    case "type":
                        switch (tag.Attributes["v"].Value)
                        {
                            case "lanelet":
                                gameObject.AddComponent<Lanelet>();
                                break;
                            case "regulatory_element":
                                gameObject.AddComponent<RegulatoryElement>();
                                break;
                            default:
                                Debug.LogWarning($"Unsupported Relation type {tag.Attributes["v"].Value} on {name}");
                                break;
                        }
                        break;
                    case "subtype":
                        var relation_element = gameObject.GetComponent<RegulatoryElement>();
                        if (relation_element)
                        {
                            switch (tag.Attributes["v"].Value)
                            {
                                case "traffic_light":
                                    relation_element.subType = RegulatoryElement.SubType.traffic_light;
                                    break;
                                case "traffic_sign":
                                    relation_element.subType = RegulatoryElement.SubType.traffic_sign;
                                    break;
                                case "road":
                                    relation_element.subType = RegulatoryElement.SubType.road;
                                    break;
                                default:
                                    break;
                            }
                        }
                            break;
                    case "turn_direction":
                        var lanelet = gameObject.GetComponent<Lanelet>();
                        if (lanelet)
                        {
                            switch (tag.Attributes["v"].Value)
                            {
                                case "straight":
                                    lanelet.turnDirection = Lanelet.TurnDirection.Straight;
                                    break;
                                case "left":
                                    lanelet.turnDirection = Lanelet.TurnDirection.Left;
                                    break;
                                case "right":
                                    lanelet.turnDirection = Lanelet.TurnDirection.Right;
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    case "speed_limit":
                        lanelet = gameObject.GetComponent<Lanelet>();
                        if (lanelet)
                        {
                            if(tag.Attributes["v"].Value.Contains("km/h"))
                            {
                                lanelet.speed_limit = float.Parse(tag.Attributes["v"].Value.Replace("km/h",""));
                            }
                            else
                            {
                                lanelet.speed_limit = Lanelet.default_speed_limit;
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning($"Unsupported Relation tag {tag.Attributes["k"].Value} on {name}");
                        break;
                }
            }
            foreach (XmlNode member in xmlNode.SelectNodes("member"))
            {
                switch (member.Attributes["type"].Value)
                {
                    case "node":
                        members.Add(Lanelet2Map.transform.Find(member.Attributes["ref"].Value).GetComponent<Node>());
                        break;
                    case "way":
                        var way = Lanelet2Map.transform.Find(member.Attributes["ref"].Value).GetComponent<Way>();
                        members.Add(way);
                        switch (member.Attributes["role"].Value)
                        {
                            case "left":
                                GetComponent<Lanelet>().left = way.GetOrAddComponent<LineThin>();
                                break;
                            case "right":
                                GetComponent<Lanelet>().right = way.GetOrAddComponent<LineThin>();
                                break;
                            case "ref_line":
                                GetComponent<RegulatoryElement>().ref_line = way;
                                break;
                            case "refers":
                                GetComponent<RegulatoryElement>().refers = way;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "relation":
                        RelationId.Add(member.Attributes["ref"].Value);
                        break;
                    default:
                        break;
                }
            }
        }
        internal XmlElement Save(XmlDocument doc)
        {
            XmlElement relation = doc.CreateElement("relation");
            relation.SetAttribute("id", name);
            members.RemoveNull();
            var lanelet = GetComponent<Lanelet>();
            if (lanelet)
            {
                relation.AppendChild(doc.AddTag("type", "lanelet"));
                relation.AppendChild(doc.AddTag("subtype", "road"));
                switch (lanelet.turnDirection)
                {
                    case Lanelet.TurnDirection.Straight:
                        relation.AppendChild(doc.AddTag("turn_direction", "straight"));
                        break;
                    case Lanelet.TurnDirection.Left:
                        relation.AppendChild(doc.AddTag("turn_direction", "left"));
                        break;
                    case Lanelet.TurnDirection.Right:
                        relation.AppendChild(doc.AddTag("turn_direction", "right"));
                        break;
                    default:
                        break;
                }
                relation.AppendChild(doc.AddTag("speed_limit", string.Format("{0}km/h", lanelet.speed_limit)));
                relation.AppendChild(doc.AddMember("way", lanelet.left.name, "left"));
                relation.AppendChild(doc.AddMember("way", lanelet.right.name, "right"));
                foreach (var item in members)
                {
                    if (item is Relation)
                    {
                        if (item.GetComponent<RegulatoryElement>())
                        {
                            relation.AppendChild(doc.AddMember("relation", item.name, "regulatory_element"));
                        }
                    }
                }
            }
            else
            {
                var regulatory_element = GetComponent<RegulatoryElement>();
                if (regulatory_element)
                {
                    relation.AppendChild(doc.AddTag("type", "regulatory_element"));
                    relation.AppendChild(doc.AddTag("subtype", regulatory_element.subType.ToString()));
                    relation.AppendChild(doc.AddMember("way", regulatory_element.refers.name, "refers"));
                    relation.AppendChild(doc.AddMember("way", regulatory_element.ref_line.name, "ref_line"));
                }
            }
            return relation;
        }
        private void Start()
        {
            foreach (var member in members)
            {
                member.Ref.TryAdd(this);
            }
        }
    }
}