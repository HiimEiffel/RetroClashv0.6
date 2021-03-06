using System.Net;
using System.Threading.Tasks;
using RetroClash.Database;
using RetroClash.Extensions;
using RetroClash.Logic;
using RetroClash.Protocol.Messages.Server;

namespace RetroClash.Protocol.Messages.Client
{
    public class LoginMessage : Message
    {
        public LoginMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        public long AccountId { get; set; }
        public string Token { get; set; }
        public string Language { get; set; }
        public string DeviceName { get; set; }
        public string MasterHash { get; set; }

        public override void Decode()
        {
            AccountId = Reader.ReadInt64(); // ACCOUNT ID
            Token = Reader.ReadString(); // PASS TOKEN

            Reader.ReadInt32(); // Major Version
            Reader.ReadInt32(); // Minor Version
            Reader.ReadInt32(); // Build 

            MasterHash = Reader.ReadString(); // Masterhash

            Reader.ReadString(); // UDID

            Reader.ReadString(); // OpenUDID

            Reader.ReadString(); // MacAddress
            DeviceName = Reader.ReadString(); // Device

            Reader.ReadInt32(); // Unknown

            Language = Reader.ReadString(); // Language

            Reader.ReadString(); // ADID

            Reader.ReadString(); // OS Version

            Reader.ReadByte(); // IsAndroid
            Reader.ReadInt32(); // IMEI

            Reader.ReadString(); // AndroidId
        }

        public override async Task Process()
        {
            if (Device.State != Enums.State.Home || Language.Length >= 2)
                if (Configuration.Maintenance)
                {
                    await Resources.Gateway.Send(new LoginFailedMessage(Device) {ErrorCode = 10});
                }
                else
                {
                    if (MasterHash == Resources.Fingerprint.Sha)
                        if (Resources.PlayerCache.Count < Configuration.MaxClients)
                        {
                            if (AccountId == 0)
                            {
                                Device.Player = await MySQL.CreatePlayer();

                                if (Device.Player != null)
                                {
                                    Device.Player.Language = Language.ToUpper();
                                    Device.Player.DeviceName = DeviceName;
                                    Device.Player.IpAddress =
                                        ((IPEndPoint) Device.Socket.RemoteEndPoint).Address.ToString();
                                    Device.Player.Device = Device;

                                    await Resources.Gateway.Send(new LoginOkMessage(Device));

                                    await Resources.PlayerCache.AddPlayer(AccountId, Device.Player);

                                    await Resources.Gateway.Send(new OwnHomeDataMessage(Device));
                                }
                                else
                                {
                                    await Resources.Gateway.Send(new LoginFailedMessage(Device)
                                    {
                                        ErrorCode = 10,
                                        Reason =
                                            "An error occured during the creation of your account. Please contact the administrators of this server."
                                    });
                                }
                            }
                            else
                            {
                                if (Resources.PlayerCache.ContainsKey(AccountId))
                                {
                                    await Resources.Gateway.Send(new DisconnectedMessage(Resources.PlayerCache[AccountId].Device));
                                    await Resources.PlayerCache.RemovePlayer(AccountId, Resources.PlayerCache[AccountId].Device.SessionId);
                                }

                                Device.Player = await MySQL.GetPlayer(AccountId);

                                if (Device.Player != null && Device.Player.PassToken == Token)
                                {
                                    Device.Player.Device = Device;

                                    await Resources.Gateway.Send(new LoginOkMessage(Device));

                                    if (await Resources.PlayerCache.AddPlayer(AccountId, Device.Player))
                                    {

                                        if (Device.Player.AllianceId > 0)
                                        {
                                            var alliance =
                                                await Resources.AllianceCache.GetAlliance(Device.Player.AllianceId);

                                            if (!alliance.IsMember(AccountId))
                                            {
                                                Device.Player.AllianceId = 0;

                                                await Resources.Gateway.Send(new OwnHomeDataMessage(Device));
                                            }
                                            else
                                            {
                                                await Resources.Gateway.Send(new OwnHomeDataMessage(Device));

                                                await Resources.Gateway.Send(new AllianceStreamMessage(Device)
                                                {
                                                    AllianceStream = alliance.Stream
                                                });
                                            }
                                        }
                                        else
                                        {
                                            await Resources.Gateway.Send(new OwnHomeDataMessage(Device));
                                        }
                                    }
                                    else
                                    {
                                        await Resources.Gateway.Send(new LoginFailedMessage(Device)
                                        {
                                            ErrorCode = 10,
                                            Reason =
                                                "The server couldn't add you to the cache."
                                        });
                                    }
                                }
                                else
                                {
                                    await Resources.Gateway.Send(new LoginFailedMessage(Device)
                                    {
                                        ErrorCode = 10,
                                        Reason =
                                            "We couldn't find your account in our systems or your token is invalid."
                                    });

                                    Device.Disconnect();
                                }
                            }
                        }
                        else
                        {
                            await Resources.Gateway.Send(new LoginFailedMessage(Device)
                            {
                                ErrorCode = 10,
                                Reason = "The server is currently full."
                            });

                            Device.Disconnect();
                        }
                    else
                        await Resources.Gateway.Send(new LoginFailedMessage(Device)
                        {
                            ErrorCode = 7,
                            Fingerprint = Resources.Fingerprint.Json
                        });
                }
            else
                Device.Disconnect();
        }
    }
}