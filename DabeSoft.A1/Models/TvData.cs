using System;
using System.Collections.Generic;

namespace DabeSoft.A1.Models
{
    public class ProgramInfo
    {
        public int EventId;
        public int StationId;
        public DateTime Start;
        public DateTime End;
        public string Name;
        public string ShortInfo;
        public string StationName;
        public string Description;
        public readonly List<string> Genres = new List<string>();
        public string LustigerBuchstabe;
        public int Year;
        public string Category;
    }
}
