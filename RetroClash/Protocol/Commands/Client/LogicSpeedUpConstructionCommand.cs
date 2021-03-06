using RetroClash.Extensions;
using RetroClash.Logic;

namespace RetroClash.Protocol.Commands.Client
{
    public class LogicSpeedUpConstructionCommand : Command
    {
        public LogicSpeedUpConstructionCommand(Device device, Reader reader) : base(device, reader)
        {
        }

        public int BuildingId { get; set; }

        public override void Decode()
        {
            BuildingId = Reader.ReadInt32();
            Reader.ReadInt32();
        }
    }
}