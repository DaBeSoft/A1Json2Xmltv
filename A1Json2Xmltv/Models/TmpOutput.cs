using DabeSoft.A1.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace A1Json2Xmltv.Models
{
    public class TmpOutput
    {
        [JsonProperty(PropertyName = "stations")]
        public List<Station> Stations { get; set; }

        [JsonProperty(PropertyName = "programs")]
        public List<ProgramInfo> Programs { get; set; }
    }
}
