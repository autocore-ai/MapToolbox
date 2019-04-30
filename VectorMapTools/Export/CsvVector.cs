#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

using UnityEngine;

namespace Packages.UnityTools.VectorMapTools.Export
{
    class CsvVector
    {
        internal const string fileName = "vector.csv";
        internal const string header = "VID,PID,Hang,Vang";
        public Quaternion rotation { get; set; }
        public CsvPoint Point { get; set; }
        public int VID { get; set; }
        public int? PID => Point?.PID;
        public float Hang => rotation.eulerAngles.y + 90;
        public float Vang => rotation.eulerAngles.x + 90;
        public string CsvString => $"{VID},{PID ?? 0},{Hang:F2},{Vang:F2}";
    }
}
