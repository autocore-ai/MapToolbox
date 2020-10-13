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


namespace Packages.MapToolbox
{
    static class Utils
    {
        public static void TryAdd<T>(this ICollection<T> collection, T target)
        {
            if (!collection.Contains(target))
            {
                collection.Add(target);
            }
        }
        public static void TryRemove<T>(this ICollection<T> collection, T target)
        {
            if (collection.Contains(target))
            {
                collection.Remove(target);
            }
        }
        public static void RemoveNull<T>(this List<T> list) => list.RemoveAll(_ => _ == null);
        public static void RemoveLast<T>(this ICollection<T> collection)
        {
            if (collection.Count > 0)
            {
                collection.Remove(collection.Last());
            }
        }
        public static void DestroyAll<T>(this ICollection<T> collection) where T : Object
        {
            for (int i = 0; i < collection.Count; i++)
            {
                Undo.DestroyObjectImmediate(collection.ElementAt(i));
            }
        }
        public static void RecordUndoCreateGo(this GameObject go) => Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        public static T GetChild<T>(this Component component, string name = null) where T : Component
        {
            T ret = component.GetComponentInChildren<T>();
            if (ret == null)
            {
                ret = component.AddChildGameObject<T>(name);
            }
            return ret;
        }
        public static T AddChildGameObject<T>(this Component component, string name = null) where T : Component
        {
            var target = new GameObject(name ?? typeof(T).Name).AddComponent<T>();
            target.transform.SetParent(component.transform);
            target.transform.position = component.transform.position;
            return target;
        }
        public static void SetLocalX(this Transform transform, double x) => transform.SetLocalX((float)x);
        public static void SetLocalY(this Transform transform, double y) => transform.SetLocalY((float)y);
        public static void SetLocalZ(this Transform transform, double z) => transform.SetLocalZ((float)z);
        public static void SetLocalX(this Transform transform, float x) => transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        public static void SetLocalY(this Transform transform, float y) => transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        public static void SetLocalZ(this Transform transform, float z) => transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        public static Vector3 MousePointInSceneView
        {
            get
            {
                Vector3 mousePosition = Event.current.mousePosition;
                mousePosition.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePosition.y * EditorGUIUtility.pixelsPerPoint;
                mousePosition.x *= EditorGUIUtility.pixelsPerPoint;
                mousePosition.z = 20;
                mousePosition = SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(mousePosition);
                return mousePosition;
            }
        }
        public static bool MouseInSceneView => SceneView.lastActiveSceneView.camera.pixelRect.Contains(Event.current.mousePosition * EditorGUIUtility.pixelsPerPoint);
        public static XmlElement AddTag(this XmlDocument doc, string key, string value)
        {
            XmlElement tag = doc.CreateElement("tag");
            tag.SetAttribute("k", key);
            tag.SetAttribute("v", value);
            return tag;
        }
        public static XmlElement AddTag(this XmlDocument doc, Tag tag) => doc.AddTag(tag.k, tag.v);
        public static Tag SetOrAddTag(this ICollection<Tag> tags, string key, string value)
        {
            var target = tags.Where(_ => _.k.Equals(key));
            if (target.Count() > 0)
            {
                return target.First();
            }
            else
            {
                var tag = new Tag(key, value);
                tags.Add(tag);
                return tag;
            }
        }
        public static XmlElement AddMember(this XmlDocument doc, string type, string @ref, string role)
        {
            XmlElement member = doc.CreateElement("member");
            member.SetAttribute("type", type);
            member.SetAttribute("ref", @ref);
            member.SetAttribute("role", role);
            return member;
        }
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component => gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        public static T GetOrAddComponent<T>(this Component component) where T : Component => component.gameObject.GetOrAddComponent<T>();
        public static float ToFloat(this XmlAttribute attribute) => float.Parse(attribute.Value);
        public static float ToFloat(this XmlNode node) => float.Parse(node.InnerText);
        public static double ToDouble(this XmlAttribute attribute) => double.Parse(attribute.Value);
        public static double ToDouble(this XmlNode node) => double.Parse(node.InnerText);
    }
}