#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.AutowareUnityTools.VectorMapTools.Export
{
    class CsvWhiteLine
    {
        internal const string fileName = "whiteline.csv";
        internal const string header = "ID,LID,Width,Color,type,LinkID";
        public CsvLine Line { get; set; }
        public int ID { get; set; }
        public int? LID => Line?.LID;
        public float Width { get; set; }
        public string CsvString => $"{ID},{LID ?? 0},{Width:F3},W,0,1";
    }
}