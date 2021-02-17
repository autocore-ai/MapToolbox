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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    class Node : Member
    {
        public UnityAction<Node> OnMoved { get; set; }
        public UnityAction<Node> OnDestroyed { get; set; }
        public UnityAction<Node, Node> OnMerged { get; set; }
        public List<Tag> extermTags = new List<Tag>();
        internal Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                OnMoved?.Invoke(this);
            }
        }
        internal static Node AddNew(Lanelet2Map map, Vector3 position)
        {
            var ret = map.AddChildGameObject<Node>((map.transform.childCount+1).ToString());
            ret.Position = position;
            ret.gameObject.RecordUndoCreateGo();
            return ret;
        }
        private void Start() => gameObject.SetIcon(IconManager.ShapeIcon.CircleGray);
        protected void OnDestroy() => OnDestroyed?.Invoke(this);

        [DllImport("GeographicWarpper")]
        extern static void UTMUPS_Forward(double lat, double lon, out int zone, out bool northp, out double x, out double y);
        
        [DllImport("GeographicWarpper")]
        extern static void UTMUPS_Reverse(int zone, bool northp, double x, double y, out double lat, out double lon);
        
        [DllImport("GeographicWarpper")]
        extern static void MGRS_Reverse(string mgrs, out int zone, out bool northp, out double x, out double y, out int prec, bool centerp);

        private void Mgrs2Gps(string mgrs_code, double x, double y, out double lat, out double lon){
            int zone;
            bool northp;
            int prec;
            string mgrs= String.Format("{0}{1:00000}{2:00000}", Utils.MGRS_code, transform.localPosition.x, transform.localPosition.z);
            MGRS_Reverse(mgrs, out zone, out northp, out x, out y, out prec, true);
            UTMUPS_Reverse(zone, northp, x, y, out lat, out lon);
        }

        internal void Load(XmlNode xmlNode)
        {
            name = xmlNode.Attributes["id"].Value;
            var lat = double.Parse(xmlNode.Attributes["lat"].Value);
            var lon = double.Parse(xmlNode.Attributes["lon"].Value);
            UTMUPS_Forward(lat, lon, out int zone, out bool northp, out double x, out double y);
            x %= 1e5;
            y %= 1e5;
            transform.SetLocalX(x);
            transform.SetLocalZ(y);
            extermTags.Clear();
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
                        extermTags.Add(new Tag(tag));
                        break;
                }
            }
        }
        internal XmlElement Save(XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("node");
            node.SetAttribute("id", name);

            double lat, lon;
            Mgrs2Gps(Utils.MGRS_code, transform.localPosition.x, transform.localPosition.z, out lat, out lon);
            node.SetAttribute("lat", lat.ToString());
            node.SetAttribute("lon", lon.ToString());
            node.SetAttribute("version", "1");
            
            node.AppendChild(doc.AddTag("ele", transform.localPosition.y.ToString()));
            node.AppendChild(doc.AddTag("local_x", transform.localPosition.x.ToString()));
            node.AppendChild(doc.AddTag("local_y", transform.localPosition.z.ToString()));
            foreach (var item in extermTags)
            {
                node.AppendChild(doc.AddTag(item.k, item.v));
            }
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
            OnMerged?.Invoke(this, node);
            Selection.activeObject = node;
            Undo.DestroyObjectImmediate(gameObject);
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
            if (targets.Length > maxMultiEditorCount)
            {
                return;
            }
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
                SelectList.Add(Targets.Where(_ => !SelectList.Contains(_)).First());
            }
            else if (targets.Length == SelectList.Count - 1)
            {
                SelectList.Remove(SelectList.Where(_ => !Targets.Contains(_)).First());
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
                if (GUILayout.Button("Debug Distance"))
                {
                    Debug.Log($"Distance:{Vector3.Distance(Targets[0].Position, Targets[1].Position)}");
                    Debug.DrawLine(Targets[0].Position, Targets[1].Position, Color.white, 1);
                }
            }
        }
    }
}