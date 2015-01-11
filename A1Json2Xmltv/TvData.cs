using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1Json2Xmltv
{
    class TvData
    {
        public int Id;
        public string Name;
        public readonly ConcurrentBag<ProgramInfo> Programs = new ConcurrentBag<ProgramInfo>();
    }

    class ProgramInfo
    {
        public int EventId;
        public string Start;
        public string End;
        public string Name;
        public string ShortInfo;
        public string Description;
        public readonly List<string> Genres = new List<string>();
        public string LustigerBuchstabe;
        public int Year;
    }
}
