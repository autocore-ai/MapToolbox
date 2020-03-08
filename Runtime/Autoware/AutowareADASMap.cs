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


using UnityEngine;

namespace AutoCore.MapToolbox.Autoware
{
    [ExecuteInEditMode]
    class AutowareADASMap : MonoBehaviour
    {
        #region Collection
        CollectionADASLane laneCollection;
        public CollectionADASLane CollectionLane
        {
            set => laneCollection = value;
            get
            {
                if (laneCollection == null)
                {
                    laneCollection = GetComponentInChildren<CollectionADASLane>();
                    if (laneCollection == null)
                    {
                        laneCollection = new GameObject(typeof(ADASMapLane).Name).AddComponent<CollectionADASLane>();
                        laneCollection.transform.SetParent(transform);
                    }
                }
                return laneCollection;
            }
        }
        CollectionADASPole poleCollection;
        public CollectionADASPole CollectionPole
        {
            set => poleCollection = value;
            get
            {
                if (poleCollection == null)
                {
                    poleCollection = GetComponentInChildren<CollectionADASPole>();
                    if (poleCollection == null)
                    {
                        poleCollection = new GameObject(typeof(ADASMapPole).Name).AddComponent<CollectionADASPole>();
                        poleCollection.transform.SetParent(transform);
                    }
                }
                return poleCollection;
            }
        }
        CollectionADASSignal signalCollection;
        public CollectionADASSignal CollectionSignal
        {
            set => signalCollection = value;
            get
            {
                if (signalCollection == null)
                {
                    signalCollection = GetComponentInChildren<CollectionADASSignal>();
                    if (signalCollection == null)
                    {
                        signalCollection = new GameObject(typeof(ADASMapSignal).Name).AddComponent<CollectionADASSignal>();
                        signalCollection.transform.SetParent(transform);
                    }
                }
                return signalCollection;
            }
        }
        CollectionADASRoadSign roadSignCollection;
        public CollectionADASRoadSign CollectionRoadSign
        {
            set => roadSignCollection = value;
            get
            {
                if (roadSignCollection == null)
                {
                    roadSignCollection = GetComponentInChildren<CollectionADASRoadSign>();
                    if (roadSignCollection == null)
                    {
                        roadSignCollection = new GameObject(typeof(ADASMapRoadSign).Name).AddComponent<CollectionADASRoadSign>();
                        roadSignCollection.transform.SetParent(transform);
                    }
                }
                return roadSignCollection;
            }
        }
        CollectionADASStopLine stopLineCollection;
        public CollectionADASStopLine CollectionStopLine
        {
            set => stopLineCollection = value;
            get
            {
                if (stopLineCollection == null)
                {
                    stopLineCollection = GetComponentInChildren<CollectionADASStopLine>();
                    if (stopLineCollection == null)
                    {
                        stopLineCollection = new GameObject(typeof(ADASMapStopLine).Name).AddComponent<CollectionADASStopLine>();
                        stopLineCollection.transform.SetParent(transform);
                    }
                }
                return stopLineCollection;
            }
        }
        CollectionADASWhiteLine whiteLineCollection;
        public CollectionADASWhiteLine CollectionWhiteLine
        {
            set => whiteLineCollection = value;
            get
            {
                if (whiteLineCollection == null)
                {
                    whiteLineCollection = GetComponentInChildren<CollectionADASWhiteLine>();
                    if (whiteLineCollection == null)
                    {
                        whiteLineCollection = new GameObject(typeof(ADASMapWhiteLine).Name).AddComponent<CollectionADASWhiteLine>();
                        whiteLineCollection.transform.SetParent(transform);
                    }
                }
                return whiteLineCollection;
            }
        }
        CollectionADASRoadEdge roadEdgeCollection;
        public CollectionADASRoadEdge CollectionRoadEdge
        {
            set => roadEdgeCollection = value;
            get
            {
                if (roadEdgeCollection == null)
                {
                    roadEdgeCollection = GetComponentInChildren<CollectionADASRoadEdge>();
                    if (roadEdgeCollection == null)
                    {
                        roadEdgeCollection = new GameObject(typeof(ADASMapRoadEdge).Name).AddComponent<CollectionADASRoadEdge>();
                        roadEdgeCollection.transform.SetParent(transform);
                    }
                }
                return roadEdgeCollection;
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
        public void AddLane(Vector3 position) => CollectionLane.AddLane(position);
        public void AddSignal(Vector3 position) => CollectionSignal.AddSignal(position);
        public void AddStopLine(Vector3 position) => CollectionStopLine.AddStopLine(position);
        public void AddWhiteLine(Vector3 position) => CollectionWhiteLine.AddWhiteLine(position);
        public void AddRoadEdge(Vector3 position) => CollectionRoadEdge.AddRoadEdge(position);
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
            CollectionLane.Csv2Go();
            CollectionPole.Csv2Go();
            CollectionSignal.Csv2Go();
            CollectionRoadSign.Csv2Go();
            CollectionStopLine.Csv2Go();
            CollectionWhiteLine.Csv2Go();
            CollectionRoadEdge.Csv2Go();
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
            CollectionLane.Go2Csv();
            CollectionPole.Go2Csv();
            CollectionSignal.Go2Csv();
            CollectionRoadSign.Go2Csv();
            CollectionStopLine.Go2Csv();
            CollectionWhiteLine.Go2Csv();
            CollectionRoadEdge.Go2Csv();
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
        }
    }
}