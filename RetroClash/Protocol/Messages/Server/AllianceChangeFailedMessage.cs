using System.Threading.Tasks;
using RetroClash.Extensions;
using RetroClash.Logic;

namespace RetroClash.Protocol.Messages.Server
{
    public class AllianceChangeFailedMessage : Message
    {
        public AllianceChangeFailedMessage(Device device) : base(device)
        {
            Id = 24333;
        }

        public override async Task Encode()
        {
            await Stream.WriteIntAsync(0);
        }
    }
}
