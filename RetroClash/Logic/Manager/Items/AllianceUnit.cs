using Newtonsoft.Json;

namespace RetroClash.Logic.Manager.Items
{
    public class AllianceUnit
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cnt")]
        public int Cnt { get; set; }

        [JsonProperty("lvl")]
        public int Level { get; set; }
    }
}