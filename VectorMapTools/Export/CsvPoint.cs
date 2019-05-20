#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.AutowareUnityTools.VectorMapTools.Export
{
    class CsvPoint
    {
        internal const string fileName = "point.csv";
        internal const string header = "PID,B,L,H,Bx,Ly,ReF,MCODE1,MCODE2,MCODE3";
        public Vector3 Position { get; set; }
        public int PID { get; set; }
        public float H => Position.y;
        public float Bx => -Position.x;
        public float Ly => Position.z;
        public string CsvString => $"{PID},0,0,{H:F2},{Bx:F2},{Ly:F2},0,0,0,0";
    }
}