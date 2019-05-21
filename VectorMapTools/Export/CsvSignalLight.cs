#region License
/*
 * Copyright (c) 2018-2019 AutoCore
 */
#endregion

namespace Packages.AutowareUnityTools.VectorMapTools.Export
{
    class CsvSignalLight
    {
        internal const string fileName = "signaldata.csv";
        internal const string header = "ID,VID,PLID,Type,LinkID";
        public CsvVector Vector { get; set; }
        public int ID { get; set; }
        public int? VID => Vector.VID;
        public string CsvString => $"{ID},{VID ?? 0},0,2,0";
    }
}