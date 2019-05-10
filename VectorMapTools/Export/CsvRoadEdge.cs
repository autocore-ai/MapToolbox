#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.UnityTools.VectorMapTools.Export
{
    class CsvRoadEdge
    {
        internal const string fileName = "roadedge.csv";
        internal const string header = "ID,LID,LinkID";
        internal CsvLine Line { get; set; }
        public int ID { get; set; }
        public int? LID => Line?.LID;
        public string CsvString => $"{ID},{LID ?? 0},0";
    }
}
