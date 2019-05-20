#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.AutowareUnityTools.VectorMapTools.Export
{
    class CsvNode
    {
        internal const string fileName = "node.csv";
        internal const string header = "NID,PID";
        public CsvPoint Point { get; set; }
        public int NID { get; set; }
        public int? PID => Point?.PID;
        public string CsvString => $"{NID},{PID ?? 0}";
    }
}