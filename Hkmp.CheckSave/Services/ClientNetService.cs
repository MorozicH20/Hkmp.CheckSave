using Hkmp.Api.Client;
using Hkmp.CheckSave.Models;
using Hkmp.CheckSave.Packets;
using Hkmp.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hkmp.CheckSave.Services
{
    internal class ClientNetService
    {
        public ClientNetService(ILogger logger, Client addon, IClientApi clientApi)
        {
            FileEdit logs = new FileEdit();
            var clientSave = new PlayerSave();
            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var sender = clientApi.NetClient.GetNetworkSender<PlayerSavePacketId>(addon);
            clientApi.ClientManager.ConnectEvent += () =>
            {
                logger.Info("Player connected, sending save to server");
                logs.Write("\nStart send player data");
                clientSave.LoadData();

                try
                {
                    sender.SendSingleData(PlayerSavePacketId.PlayerSaveClientData, new PlayerSavePacket
                    {
                        PlayerInfo = new PlayerInformation
                        {
                            playerSave = clientSave,
                            PlayerName = clientApi.ClientManager.Username
                        }
                    });
                    logs.Write("\tthe data has been sent successfully");

                }
                catch (Exception ex)
                {
                    logs.Write($"\t[ERROR] {ex}");

                }

            };
        }
    }
}
