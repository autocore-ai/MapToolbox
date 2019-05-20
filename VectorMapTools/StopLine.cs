#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools
{
    public class StopLine : AutoHeightLine
    {
        public List<Lane> Lanes { get; set; } = new List<Lane>();
        [Range(0, 1)]
        public float displayWidth = 0.2f;
        [Range(0, 5)]
        public float distance_autoConnectLane = 3f;
        public static List<StopLine> List { get; set; } = new List<StopLine>();
        public SignalLight[] signalLights;
        protected override void Awake()
        {
            base.Awake();
            List.Add(this);
            LineRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"))
            {
                color = Color.white
            };
            LineRenderer.startWidth = LineRenderer.endWidth = displayWidth;
        }
        private void Update()
        {
            if (signalLights != null)
            {
                if (signalLights.Length != Count + 1)
                {
                    Count = signalLights.Length + 1;
                }
            }
        }
        public override void SetPosition(int index, Vector3 position)
        {
            base.SetPosition(index, position);
            Lanes.Clear();
            for (int i = 1; i < Count; i++)
            {
                Lanes.Add(GetLane((Points[i - 1] + Points[i]) / 2));
            }
        }
        private Lane GetLane(Vector3 vector3)
        {
            var finalPoints = Lane.FinalPoints.GetNearby(vector3, distance_autoConnectLane);
            if (finalPoints != null && finalPoints.Length > 0)
            {
                return finalPoints.OrderBy(_ => Vector3.Distance(vector3, _.endPosition)).First();
            }
            return null;
        }
        private void OnDestroy() => List.Remove(this);
        private void OnDrawGizmosSelected()
        {
            if (signalLights != null)
            {
                for (int i = 0; i < signalLights.Length; i++)
                {
                    if (signalLights[i])
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine((Points[i] + Points[i + 1]) / 2, signalLights[i].transform.position);
                    }
                }
            }
        }
    }
}