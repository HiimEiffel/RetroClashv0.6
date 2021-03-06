using System.Threading.Tasks;
using RetroClash.Extensions;
using RetroClash.Logic;

namespace RetroClash.Protocol.Messages.Server
{
    public class DisconnectedMessage : Message
    {
        public DisconnectedMessage(Device device) : base(device)
        {
            Id = 25892;
        }

        // ErrorCodes
        // 1 = Another Device is connecting

        public override async Task Encode()
        {
            await Stream.WriteIntAsync(1);
        }
    }
}