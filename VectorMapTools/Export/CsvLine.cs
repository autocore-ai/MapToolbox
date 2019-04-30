#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.UnityTools.VectorMapTools.Export
{
    class CsvLine
    {
        internal const string fileName = "line.csv";
        internal const string header = "LID,BPID,FPID,BLID,FLID";
        private CsvLine lineLast;
        private CsvLine lineNext;
        public CsvPoint PointBegin { get; set; }
        public CsvPoint PointFinal { get; set; }
        public CsvLine LineLast
        {
            get => lineLast;
            set
            {
                lineLast = value;
                lineLast.lineNext = this;
            }
        }
        public CsvLine LineNext
        {
            get => lineNext;
            set
            {
                lineNext = value;
                lineLast.lineLast = this;
            }
        }
        public int LID { get; set; }
        public int? BPID => PointBegin?.PID;
        public int? FPID => PointFinal?.PID;
        public int? BLID => LineLast?.LID;
        public int? FLID => LineNext?.LID;
        public string CsvString => $"{LID},{BPID ?? 0},{FPID ?? 0},{BLID ?? 0},{FLID ?? 0}";
    }
}
