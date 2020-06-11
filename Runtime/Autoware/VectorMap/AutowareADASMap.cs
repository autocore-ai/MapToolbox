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


using UnityEngine;
namespace AutoCore.MapToolbox.Autoware
{
    [ExecuteInEditMode]
    class AutowareADASMap : MonoBehaviour
    {
        #region Collection
        CollectionLane collectionLane;
        public CollectionLane CollectionLane
        {
            set => collectionLane = value;
            get
            {
                if (collectionLane == null)
                {
                    collectionLane = GetComponentInChildren<CollectionLane>();
                    if (collectionLane == null)
                    {
                        collectionLane = new GameObject(typeof(CollectionLane).Name).AddComponent<CollectionLane>();
                        collectionLane.transform.SetParent(transform);
                    }
                }
                return collectionLane;
            }
        }
        CollectionPole collectionPole;
        public CollectionPole CollectionPole
        {
            set => collectionPole = value;
            get
            {
                if (collectionPole == null)
                {
                    collectionPole = GetComponentInChildren<CollectionPole>();
                    if (collectionPole == null)
                    {
                        collectionPole = new GameObject(typeof(CollectionPole).Name).AddComponent<CollectionPole>();
                        collectionPole.transform.SetParent(transform);
                    }
                }
                return collectionPole;
            }
        }
        CollectionSignal collectionSignal;
        public CollectionSignal CollectionSignal
        {
            set => collectionSignal = value;
            get
            {
                if (collectionSignal == null)
                {
                    collectionSignal = GetComponentInChildren<CollectionSignal>();
                    if (collectionSignal == null)
                    {
                        collectionSignal = new GameObject(typeof(CollectionSignal).Name).AddComponent<CollectionSignal>();
                        collectionSignal.transform.SetParent(transform);
                    }
                }
                return collectionSignal;
            }
        }
        CollectionRoadSign collectionRoadSign;
        public CollectionRoadSign CollectionRoadSign
        {
            set => collectionRoadSign = value;
            get
            {
                if (collectionRoadSign == null)
                {
                    collectionRoadSign = GetComponentInChildren<CollectionRoadSign>();
                    if (collectionRoadSign == null)
                    {
                        collectionRoadSign = new GameObject(typeof(CollectionRoadSign).Name).AddComponent<CollectionRoadSign>();
                        collectionRoadSign.transform.SetParent(transform);
                    }
                }
                return collectionRoadSign;
            }
        }
        CollectionStopLine stopLineCollection;
        public CollectionStopLine CollectionStopLine
        {
            set => stopLineCollection = value;
            get
            {
                if (stopLineCollection == null)
                {
                    stopLineCollection = GetComponentInChildren<CollectionStopLine>();
                    if (stopLineCollection == null)
                    {
                        stopLineCollection = new GameObject(typeof(CollectionStopLine).Name).AddComponent<CollectionStopLine>();
                        stopLineCollection.transform.SetParent(transform);
                    }
                }
                return stopLineCollection;
            }
        }
        CollectionWhiteLine whiteLineCollection;
        public CollectionWhiteLine CollectionWhiteLine
        {
            set => whiteLineCollection = value;
            get
            {
                if (whiteLineCollection == null)
                {
                    whiteLineCollection = GetComponentInChildren<CollectionWhiteLine>();
                    if (whiteLineCollection == null)
                    {
                        whiteLineCollection = new GameObject(typeof(CollectionWhiteLine).Name).AddComponent<CollectionWhiteLine>();
                        whiteLineCollection.transform.SetParent(transform);
                    }
                }
                return whiteLineCollection;
            }
        }
        CollectionRoadEdge roadEdgeCollection;
        public CollectionRoadEdge CollectionRoadEdge
        {
            set => roadEdgeCollection = value;
            get
            {
                if (roadEdgeCollection == null)
                {
                    roadEdgeCollection = GetComponentInChildren<CollectionRoadEdge>();
                    if (roadEdgeCollection == null)
                    {
                        roadEdgeCollection = new GameObject(typeof(CollectionRoadEdge).Name).AddComponent<CollectionRoadEdge>();
                        roadEdgeCollection.transform.SetParent(transform);
                    }
                }
                return roadEdgeCollection;
            }
        }
        CollectionCrossWalk collectionCrossWalk;
        public CollectionCrossWalk CollectionCrossWalk
        {
            set => collectionCrossWalk = value;
            get
            {
                if (collectionCrossWalk == null)
                {
                    collectionCrossWalk = GetComponentInChildren<CollectionCrossWalk>();
                    if (collectionCrossWalk == null)
                    {
                        collectionCrossWalk = new GameObject(typeof(CollectionCrossWalk).Name).AddComponent<CollectionCrossWalk>();
                        collectionCrossWalk.transform.SetParent(transform);
                    }
                }
                return collectionCrossWalk;
            }
        }
        CollectionRoadMark collectionRoadMark;
        public CollectionRoadMark CollectionRoadMark
        {
            set => collectionRoadMark = value;
            get
            {
                if (collectionRoadMark == null)
                {
                    collectionRoadMark = GetComponentInChildren<CollectionRoadMark>();
                    if (collectionRoadMark == null)
                    {
                        collectionRoadMark = new GameObject(typeof(CollectionRoadMark).Name).AddComponent<CollectionRoadMark>();
                        collectionRoadMark.transform.SetParent(transform);
                    }
                }
                return collectionRoadMark;
            }
        }
        CollectionWayArea collectionWayArea;
        public CollectionWayArea CollectionWayArea
        {
            set => collectionWayArea = value;
            get
            {
                if (collectionWayArea == null)
                {
                    collectionWayArea = GetComponentInChildren<CollectionWayArea>();
                    if (collectionWayArea == null)
                    {
                        collectionWayArea = new GameObject(typeof(CollectionWayArea).Name).AddComponent<CollectionWayArea>();
                        collectionWayArea.transform.SetParent(transform);
                    }
                }
                return collectionWayArea;
            }
        }
        CollectionCustomArea collectionCustomArea;
        public CollectionCustomArea CollectionCustomArea
        {
            set => collectionCustomArea = value;
            get
            {
                if (collectionCustomArea == null)
                {
                    collectionCustomArea = GetComponentInChildren<CollectionCustomArea>();
                    if (collectionCustomArea == null)
                    {
                        collectionCustomArea = new GameObject(typeof(CollectionCustomArea).Name).AddComponent<CollectionCustomArea>();
                        collectionCustomArea.transform.SetParent(transform);
                    }
                }
                return collectionCustomArea;
            }
        }
        #endregion
        public void Load(string folder)
        {
            foreach (var item in GetComponentsInChildren<Collection>())
            {
                DestroyImmediate(item.gameObject);
            }
            ReadCsv(folder);
        }
        public void AddLane(Vector3 position) => CollectionLane.AddLane(position);
        public void AddSignal(Vector3 position) => CollectionSignal.AddSignal(position);
        public void AddStopLine(Vector3 position) => CollectionStopLine.AddStopLine(position);
        public void AddWhiteLine(Vector3 position) => CollectionWhiteLine.AddWhiteLine(position);
        public void AddRoadEdge(Vector3 position) => CollectionRoadEdge.AddRoadEdge(position);
        public void AddCrossWalk(Vector3 position) => CollectionCrossWalk.AddCrossWalk(position);
        public void AddRoadMark(Vector3 position) => CollectionRoadMark.AddRoadMark(position);
        public void AddWayArea(Vector3 position) => CollectionWayArea.AddWayArea(position);
        public void AddCustomArea(Vector3 position) => CollectionCustomArea.AddCustomArea(position);
        public void Save(string folder) => WriteCsv(folder);
        private void ReadCsv(string path)
        {
            ADASMapPoint.ReadCsv(path);
            ADASMapNode.ReadCsv(path);
            ADASMapDTLane.ReadCsv(path);
            ADASMapLane.ReadCsv(path);
            ADASMapVector.ReadCsv(path);
            ADASMapPole.ReadCsv(path);
            ADASMapSignal.ReadCsv(path);
            ADASMapRoadSign.ReadCsv(path);
            ADASMapLine.ReadCsv(path);
            ADASMapStopLine.ReadCsv(path);
            ADASMapWhiteLine.ReadCsv(path);
            ADASMapRoadEdge.ReadCsv(path);
            ADASMapArea.ReadCsv(path);
            ADASMapCrossWalk.ReadCsv(path);
            ADASMapRoadMark.ReadCsv(path);
            ADASMapWayArea.ReadCsv(path);
            ADASMapCustomArea.ReadCsv(path);
            CollectionLane.Csv2Go();
            CollectionPole.Csv2Go();
            CollectionSignal.Csv2Go();
            CollectionRoadSign.Csv2Go();
            CollectionStopLine.Csv2Go();
            CollectionWhiteLine.Csv2Go();
            CollectionRoadEdge.Csv2Go();
            CollectionCrossWalk.Csv2Go();
            CollectionRoadMark.Csv2Go();
            CollectionWayArea.Csv2Go();
            CollectionCustomArea.Csv2Go();
        }
        private void WriteCsv(string path)
        {
            ADASMapPoint.Reset();
            ADASMapNode.Reset();
            ADASMapDTLane.Reset();
            ADASMapLane.Reset();
            ADASMapLine.Reset();
            ADASMapVector.Reset();
            ADASMapSignal.Reset();
            ADASMapStopLine.Reset();
            ADASMapWhiteLine.Reset();
            ADASMapRoadEdge.Reset();
            ADASMapArea.Reset();
            ADASMapCrossWalk.Reset();
            ADASMapRoadMark.Reset();
            ADASMapWayArea.Reset();
            ADASMapCustomArea.Reset();
            CollectionLane.Go2Csv();
            //CollectionPole.Go2Csv();
            CollectionSignal.Go2Csv();
            //CollectionRoadSign.Go2Csv();
            CollectionStopLine.Go2Csv();
            CollectionWhiteLine.Go2Csv();
            CollectionRoadEdge.Go2Csv();
            CollectionCrossWalk.Go2Csv();
            CollectionRoadMark.Go2Csv();
            CollectionWayArea.Go2Csv();
            CollectionCustomArea.Go2Csv();
            ADASMapPoint.WriteCsv(path);
            ADASMapNode.WriteCsv(path);
            ADASMapDTLane.WriteCsv(path);
            ADASMapLane.WriteCsv(path);
            ADASMapLine.WriteCsv(path);
            ADASMapVector.WriteCsv(path);
            ADASMapSignal.WriteCsv(path);
            ADASMapStopLine.WriteCsv(path);
            ADASMapWhiteLine.WriteCsv(path);
            ADASMapRoadEdge.WriteCsv(path);
            ADASMapArea.WriteCsv(path);
            ADASMapCrossWalk.WriteCsv(path);
            ADASMapRoadMark.WriteCsv(path);
            ADASMapWayArea.WriteCsv(path);
            ADASMapCustomArea.WriteCsv(path);
        }
    }
}