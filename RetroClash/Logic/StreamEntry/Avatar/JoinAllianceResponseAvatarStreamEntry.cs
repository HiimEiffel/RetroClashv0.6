using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RetroClash.Extensions;

namespace RetroClash.Logic.StreamEntry.Avatar
{
    public class JoinAllianceResponseAvatarStreamEntry : AvatarStreamEntry
    {
        public JoinAllianceResponseAvatarStreamEntry()
        {
            StreamEntryType = 3;
        }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("alliance_id")]
        public long AllianceId { get; set; }

        [JsonProperty("alliance_name")]
        public string AllianceName { get; set; }

        [JsonProperty("alliance_badge")]
        public int AllianceBadge { get; set; }

        [JsonProperty("sender_home_id")]
        public long SenderHomeId { get; set; }

        public override async Task Encode(MemoryStream stream)
        {
            await base.Encode(stream);

            await stream.WriteLongAsync(AllianceId); // AllianceId
            await stream.WriteStringAsync(AllianceName); // AllianceName
            await stream.WriteIntAsync(AllianceBadge); // AllianceBadge
            await stream.WriteStringAsync(Message);

            if (SenderHomeId > 0)
            {
                stream.WriteByte(1);
                await stream.WriteLongAsync(SenderHomeId); // SenderHomeId
            }
            else
            {
                stream.WriteByte(0);
            }
        }
    }
}