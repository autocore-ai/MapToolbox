#region License
/******************************************************************************
* Copyright 2019 The AutoCore Authors. All Rights Reserved.
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


using System.Linq;
using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    [ExecuteInEditMode]
    class AutowareADASMap : MonoBehaviour
    {
        #region Collection
        CollectionADASGoLane laneCollection;
        public CollectionADASGoLane CollectionLane
        {
            set => laneCollection = value;
            get
            {
                if (laneCollection == null)
                {
                    laneCollection = GetComponentInChildren<CollectionADASGoLane>();
                    if (laneCollection == null)
                    {
                        laneCollection = new GameObject(typeof(ADASMapLane).Name).AddComponent<CollectionADASGoLane>();
                        laneCollection.transform.SetParent(transform);
                    }
                }
                return laneCollection;
            }
        }
        #endregion
        public void Load(string folder)
        {
            foreach (var item in GetComponentsInChildren<CollectionADASMapGo>())
            {
                DestroyImmediate(item.gameObject);
            }
            ReadCsv(folder);
        }
        public ADASGoLane AddLane(Vector3 position) => CollectionLane.AddLane(position);
        public void Save(string folder) => WriteCsv(folder);
        private void ReadCsv(string path)
        {
            CollectionLane.ReadCsv(path);
            //VectorMapVector.ReadCsv(path);
            //VectorMapSignal.ReadCsv(path);
            //VectorMapRoadSign.ReadCsv(path);
            //VectorMapLine.ReadCsv(path);
            //VectorMapStopLine.ReadCsv(path);
            //VectorMapWhiteLine.ReadCsv(path);
            //VectorMapCurb.ReadCsv(path);
            //VectorMapArea.ReadCsv(path);
            //VectorMapRoadMark.ReadCsv(path);
            //VectorMapZebraZone.ReadCsv(path);
            //VectorMapCrossWalk.ReadCsv(path);
        }
        private void WriteCsv(string path)
        {
            CollectionLane.WriteCsv(path);
        }

        private void AddRenderer()
        {
            //AddRendererSignal();
            //AddRendererRoadSignal();
            //AddRendererStopLine();
            //AddRendererWhiteLine();
            //AddRendererCurb();
            //AddRendererRoadMark();
            //AddRendererZebraZone();
            //AddRendererCrossWalk();
        }

        private void AddRendererSignal()
        {
            var root = new GameObject(typeof(Signal).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapSignal.List)
            {
                var renderer = new GameObject().AddComponent<Signal>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapSignal = item;
            }
        }

        private void AddRendererRoadSignal()
        {
            var root = new GameObject(typeof(RoadSign).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapRoadSign.List)
            {
                var renderer = new GameObject().AddComponent<RoadSign>();
                renderer.transform.SetParent(root.transform);
                renderer.SetData(item);
            }
        }

        private void AddRendererStopLine()
        {
            var root = new GameObject(typeof(SlicesStopLine).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapStopLine.List.GroupBy(_ => _.Line.FirstLine))
            {
                var renderer = new GameObject().AddComponent<SlicesStopLine>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapStopLines = item;
                renderer.CollectionLane = CollectionLane;
                renderer.Init();
            }
        }

        private void AddRendererWhiteLine()
        {
            var root = new GameObject(typeof(SlicesWhiteLine).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapWhiteLine.List.GroupBy(_ => _.Line.FirstLine))
            {
                var renderer = new GameObject().AddComponent<SlicesWhiteLine>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapWhiteLines = item;
                renderer.Init();
            }
        }

        private void AddRendererCurb()
        {
            var root = new GameObject(typeof(SlicesCurb).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapCurb.List.GroupBy(_ => _.Line.FirstLine))
            {
                var renderer = new GameObject().AddComponent<SlicesCurb>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapCurbs = item;
                renderer.Init();
            }
        }

        private void AddRendererRoadMark()
        {
            var root = new GameObject(typeof(RoadMark).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapRoadMark.List)
            {
                var renderer = new GameObject().AddComponent<RoadMark>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapRoadMark = item;
                renderer.Init();
            }
        }

        private void AddRendererZebraZone()
        {
            var root = new GameObject(typeof(ZebraZone).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapZebraZone.List)
            {
                var renderer = new GameObject().AddComponent<ZebraZone>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapZebraZone = item;
                renderer.Init();
            }
        }

        private void AddRendererCrossWalk()
        {
            var root = new GameObject(typeof(CrossWalk).Name, typeof(CollectionADASMapGo));
            root.transform.SetParent(transform);
            foreach (var item in ADASMapCrossWalk.List)
            {
                var renderer = new GameObject().AddComponent<CrossWalk>();
                renderer.transform.SetParent(root.transform);
                renderer.VectorMapCrossWalk = item;
                renderer.Init();
            }
        }
    }
}