using RetroClash.Extensions;
using RetroClash.Logic;

namespace RetroClash.Protocol.Commands.Client
{
    public class LogicDisbandLeagueCommand : Command
    {
        public LogicDisbandLeagueCommand(Device device, Reader reader) : base(device, reader)
        {
            Id = 534;
        }
    }
}