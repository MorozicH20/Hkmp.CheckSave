using Hkmp.Api.Client;
using Hkmp.CheckSave.Packets;
using Hkmp.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hkmp.CheckSave.Services
{
    internal class ClientNetService
    {
        public ClientNetService(ILogger logger, Client addon, IClientApi clientApi)
        {
            var sender = clientApi.NetClient.GetNetworkSender<PlayerSavePacketId>(addon);
            clientApi.ClientManager.ConnectEvent += () =>
            {
                logger.Info("Player connected, sending save to server");

                sender.SendSingleData(PlayerSavePacketId.PlayerSaveClientData, new PlayerSavePacket
                {
                    PlayerInfo = new PlayerInformation
                    {
                        playerSave = new Models.PlayerSave(),
                        PlayerName = clientApi.ClientManager.Username
                    }
                });
            };
        }
    }
}
