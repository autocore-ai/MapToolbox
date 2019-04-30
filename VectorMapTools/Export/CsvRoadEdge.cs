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
        public int ID { get; }
        public int LID { get; }
        public string CsvString => $"{ID},{LID},0";
    }
}
