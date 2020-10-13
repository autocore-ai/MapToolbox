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

using System.Xml;
using UnityEngine;

namespace Packages.MapToolbox
{
    public class Origin : MonoBehaviour
    {
        public double latitude;
        public double longtitude;
        public double altitude;
        public int zone;
        public bool northp;
        public double x;
        public double y;
        public bool localPosition = false;
        internal void Load(XmlNode node)
        {
            latitude = node.SelectSingleNode("lat").ToDouble();
            longtitude = node.SelectSingleNode("lon").ToDouble();
            altitude = node.SelectSingleNode("alt").ToDouble();
            GeographicWarpper.UTMUPS_Forward(latitude, longtitude, out int zone, out bool northp, out double x, out double y);
            this.zone = zone;
            this.northp = northp;
            this.x = x;
            this.y = y;
        }
        internal XmlElement Save(XmlDocument doc)
        {
            XmlElement node = doc.CreateElement("origin");
            var lat = doc.CreateElement("lat");
            var lon = doc.CreateElement("lon");
            var alt = doc.CreateElement("alt");
            lat.InnerText = latitude.ToString();
            lon.InnerText = longtitude.ToString();
            alt.InnerText = altitude.ToString();
            node.AppendChild(lat);
            node.AppendChild(lon);
            node.AppendChild(alt);
            return node;
        }
    }
}