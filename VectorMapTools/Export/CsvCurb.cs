#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.AutowareUnityTools.VectorMapTools.Export
{
    class CsvCurb
    {
        internal const string fileName = "curb.csv";
        internal const string header = "ID,LID,Height,Width,Dir,LinkID";
        internal CsvLine Line { get; set; }
        internal int ID { get; set; }
        internal int? LID => Line?.LID;
        internal float Height { get; set; } = 0.15f;
        internal float Width { get; set; } = 0.15f;
        internal float Dir { get; set; } = 1;
        internal string CsvString => $"{ID},{LID ?? 0},{Height},{Width},{Dir},0";
    }
}