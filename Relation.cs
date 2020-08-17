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
using System.Xml;
using UnityEngine;
namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    class Relation : Member
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
                            default:
                                break;
                        }
                        break;
                    default:
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
                                GetComponent<Lanelet>().left = way.GetComponent<LineThin>();
                                break;
                            case "right":
                                GetComponent<Lanelet>().right = way.GetComponent<LineThin>();
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
        internal XmlElement Save(XmlDocument xmlDocument)
        {
            XmlElement relation = xmlDocument.CreateElement("relation");
            relation.SetAttribute("id", name);
            members.RemoveNull();
            foreach (var item in members)
            {
                XmlElement member = xmlDocument.CreateElement("member");
                if (item is Way)
                {
                    member.SetAttribute("type", "way");
                    if (item.GetComponent<StopLine>())
                    {
                        member.SetAttribute("role", "ref_line");
                    }
                    else if (item.GetComponent<TrafficSign>() || item.GetComponent<TrafficLight>())
                    {
                        member.SetAttribute("role", "refers");
                    }
                }
                else if (item is Relation)
                {
                    member.SetAttribute("type", "relation");
                    member.SetAttribute("role", "regulatory_element");
                }
                else if (item is Node)
                {
                    member.SetAttribute("type", "node");
                }
                member.SetAttribute("ref", item.name);
                relation.AppendChild(member);
            }
            var lanelet = GetComponent<Lanelet>();
            if (lanelet)
            {
                XmlElement type = xmlDocument.CreateElement("tag");
                type.SetAttribute("k", "type");
                type.SetAttribute("v", "lanelet");
                relation.AppendChild(type);
                XmlElement subType = xmlDocument.CreateElement("tag");
                subType.SetAttribute("k", "subtype");
                subType.SetAttribute("v", "road");
                relation.AppendChild(subType);
                XmlElement left = xmlDocument.CreateElement("member");
                left.SetAttribute("type", "way");
                left.SetAttribute("ref", lanelet.left.name);
                left.SetAttribute("role", "left");
                relation.AppendChild(left);
                XmlElement right = xmlDocument.CreateElement("member");
                right.SetAttribute("type", "way");
                right.SetAttribute("ref", lanelet.right.name);
                right.SetAttribute("role", "right");
                relation.AppendChild(right);
            }
            else
            {
                var regulatory_element = GetComponent<RegulatoryElement>();
                if (regulatory_element)
                {
                    XmlElement type = xmlDocument.CreateElement("tag");
                    type.SetAttribute("k", "type");
                    type.SetAttribute("v", "regulatory_element");
                    relation.AppendChild(type);
                    XmlElement subtype = xmlDocument.CreateElement("tag");
                    subtype.SetAttribute("k", "subtype");
                    subtype.SetAttribute("v", regulatory_element.subType.ToString());
                    relation.AppendChild(subtype);
                }
            }
            return relation;
        }
    }
}