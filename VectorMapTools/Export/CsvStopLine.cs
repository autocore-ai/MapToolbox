#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.UnityTools.VectorMapTools.Export
{
    class CsvStopLine
    {
        internal const string fileName = "stopline.csv";
        internal const string header = "ID,LID,TLID,SignID,LinkID";
        public CsvLine Line { get; set; }
        public CsvSignalLight SignalLight { get; set; }
        public CsvLane Lane { get; set; }
        public int ID { get; set; }
        public int? LID => Line?.LID;
        public int? TLID => SignalLight?.ID;
        public int? LinkID => Lane?.LnID;
        public string CsvString => $"{ID},{LID ?? 0},{TLID ?? 0},0,{LinkID ?? 0}";
    }
}
