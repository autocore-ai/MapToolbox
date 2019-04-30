#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools.Export
{
    class CsvDtLane
    {
        internal const string fileName = "dtlane.csv";
        internal const string header = "DID,Dist,PID,Dir,Apara,r,slope,cant,LW,RW";
        private CsvDtLane lastDtLane;
        private CsvDtLane nextDtLane;
        public CsvDtLane LastDtLane
        {
            get => lastDtLane;
            set
            {
                lastDtLane = value;
                lastDtLane.nextDtLane = this;
            }
        }
        public CsvPoint Point { get; set; }
        public int DID { get; set; }
        public int? PID => Point?.PID;
        public float Dist => AddSegmentDistance(0);
        public float? Dir => nextDtLane?.DirectionFrom(this);
        private float AddSegmentDistance(float currentDistance) => lastDtLane == null ? currentDistance : (Vector3.Distance(lastDtLane.Point.Position, Point.Position) + lastDtLane.AddSegmentDistance(currentDistance));
        private float DirectionFrom(CsvDtLane target) => Vector3.SignedAngle(Point.Position - target.Point.Position, Vector3.forward, Vector3.up) / 180 * Mathf.PI;
        public string CsvString => $"{DID},{Dist:F2},{PID ?? 0},{Dir ?? lastDtLane.Dir:F2},0,90000000000,0,0,2,2";
    }
}