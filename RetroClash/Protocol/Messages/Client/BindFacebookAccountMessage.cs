using System.Threading.Tasks;
using RetroClash.Database;
using RetroClash.Extensions;
using RetroClash.Logic;
using RetroClash.Protocol.Messages.Server;

namespace RetroClash.Protocol.Messages.Client
{
    public class BindFacebookAccountMessage : Message
    {
        public BindFacebookAccountMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public string FacebookId { get; set; }

        public override void Decode()
        {
            Reader.ReadByte(); // Force
            FacebookId = Reader.ReadString(); // Facebook Id
            Reader.ReadString(); // AuthenticationToken
        }

        public override async Task Process()
        {
            var player = await MySQL.GetPlayerByFacebookId(FacebookId);

            if (player == null)
            {
                Device.Player.FacebookId = FacebookId;
                await Resources.Gateway.Send(new FacebookAccountBoundMessage(Device));
            }
            else
            {
                if (Device.Player.AccountId != player.AccountId)
                {
                    await Resources.Gateway.Send(new FacebookAccountAlreadyBoundMessage(Device) {Player = player});
                }
                else
                {
                    Device.Player.FacebookId = FacebookId;
                    await Resources.Gateway.Send(new FacebookAccountBoundMessage(Device));
                }
            }
        }
    }
}