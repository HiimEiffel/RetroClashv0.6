using System;
using System.Threading.Tasks;
using RetroClash.Extensions;
using RetroClash.Logic;

namespace RetroClash.Protocol.Commands.Client
{
    public class LogicSpeedUpHeroHealthCommand : Command
    {
        public LogicSpeedUpHeroHealthCommand(Device device, Reader reader) : base(device, reader)
        {
        }

        public int HeroId { get; set; }

        public override void Decode()
        {
            HeroId = Reader.ReadInt32();

            Reader.ReadInt32();
        }

        public override async Task Process()
        {
            var hero = Device.Player.HeroManager.Get(HeroId);

            if (hero != null)
                hero.RegenerationEndTime = DateTime.Now;
        }
    }
}