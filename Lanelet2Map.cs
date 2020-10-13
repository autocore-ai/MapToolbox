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


using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Packages.MapToolbox
{
    [ExecuteInEditMode]
    class Lanelet2Map : MonoBehaviour
    {
        private void Awake() => EditorUpdate.Instance.transform.SetAsFirstSibling();

        internal void Load(string filename)
        {
            var doc = new XmlDocument();
            doc.Load(filename);
            var osm = doc.SelectSingleNode("osm");
            var node_origin = osm.SelectSingleNode("origin");
            if (node_origin != null)
            {
                gameObject.GetOrAddComponent<Origin>().Load(node_origin);
            }
            foreach (XmlNode item in osm.SelectNodes("node"))
            {
                this.AddChildGameObject<Node>().Load(item);
            }
            foreach (XmlNode item in osm.SelectNodes("way"))
            {
                this.AddChildGameObject<Way>().Load(item);
            }
            foreach (XmlNode item in osm.SelectNodes("relation"))
            {
                this.AddChildGameObject<Relation>().Load(item);
            }
            var collectionRelation = GetComponentsInChildren<Relation>();
            foreach (var relation in collectionRelation)
            {
                foreach (var item in relation.RelationId)
                {
                    relation.Members.Add(transform.Find(item).GetComponent<Relation>());
                }
            }
        }
        public void Save(string filename)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).name = i.ToString();
            }
            var info = UnityEditor.PackageManager.PackageInfo.FindForAssembly(Assembly.GetExecutingAssembly());
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement osm = doc.CreateElement("osm");
            osm.SetAttribute("generator", info.displayName);
            osm.SetAttribute("version", info.version);
            osm.AppendChild(GetComponent<Origin>().Save(doc));
            doc.AppendChild(osm);
            foreach (var item in GetComponentsInChildren<Node>())
            {
                osm.AppendChild(item.Save(doc));
            }
            foreach (var item in GetComponentsInChildren<Way>())
            {
                osm.AppendChild(item.Save(doc));
            }
            foreach (var item in GetComponentsInChildren<Relation>())
            {
                osm.AppendChild(item.Save(doc));
            }
            doc.Save(filename);
            Undo.RegisterFullObjectHierarchyUndo(gameObject, "Save map");
        }
        internal static void CreateNew()
        {
            var go = CreateNewGo();
            go.RecordUndoCreateGo();
            Selection.activeObject = go;
        }
        private static GameObject CreateNewGo() => new GameObject(typeof(Lanelet2Map).Name, typeof(Lanelet2Map));
        internal void AddLanelet() => Selection.activeObject = Lanelet.AddNew(this);
        internal void AddTrafficLight() => Selection.activeObject = TrafficLight.AddNew(this);
        internal void AddTrafficSign() => Selection.activeObject = TrafficSign.AddNew(this);
        internal void AddParkingSpace() => Selection.activeObject = ParkingLot.AddNew(this);
        internal void AddParkingLot() => Selection.activeObject = ParkingSpot.AddNew(this);
    }

    [CustomEditor(typeof(Lanelet2Map))]
    class Lanelet2MapEditor : Editor
    {
        [MenuItem("GameObject/Autoware/Lanelet2Map", false, 10)]
        static void CreateNew() => Lanelet2Map.CreateNew();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save map"))
            {
                string saveFile = EditorUtility.SaveFilePanel(string.Empty, Application.dataPath, "lanelet2_map", "osm");
                if (!string.IsNullOrEmpty(saveFile))
                {
                    (target as Lanelet2Map).Save(saveFile);
                }
            }
            if (GUILayout.Button("Add Lanelet"))
            {
                (target as Lanelet2Map).AddLanelet();
            }
            if (GUILayout.Button("Add TrafficLight"))
            {
                (target as Lanelet2Map).AddTrafficLight();
            }
            if (GUILayout.Button("Add TrafficSign"))
            {
                (target as Lanelet2Map).AddTrafficSign();
            }
            if (GUILayout.Button("Add ParkingSpace"))
            {
                (target as Lanelet2Map).AddParkingSpace();
            }
            if (GUILayout.Button("Add ParkingLot"))
            {
                (target as Lanelet2Map).AddParkingLot();
            }
            if (GUILayout.Button("Clear Filter"))
            {
                SceneModeUtility.SearchForType(null);
            }
            if (GUILayout.Button("Filter Lanelet"))
            {
                SceneModeUtility.SearchForType(typeof(Lanelet));
            }
            if (GUILayout.Button("Filter StopLine"))
            {
                SceneModeUtility.SearchForType(typeof(StopLine));
            }
            if (GUILayout.Button("Filter TrafficLight"))
            {
                SceneModeUtility.SearchForType(typeof(TrafficLight));
            }
            if (GUILayout.Button("Filter TrafficSign"))
            {
                SceneModeUtility.SearchForType(typeof(TrafficSign));
            }
        }
    }
}