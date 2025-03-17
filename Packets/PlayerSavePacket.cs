using System.Collections.Generic;
using Hkmp.CheckSave.Models;
using Hkmp.Networking.Packet;
using Newtonsoft.Json;

namespace Hkmp.CheckSave.Packets
{
    public enum PlayerSavePacketId
    {
        /// <summary>
        /// Client data packet for sending the modlist to the server for comparison
        /// </summary>
        PlayerSaveClientData,
    }

    internal class PlayerSavePacket : IPacketData
    {
        public bool IsReliable => true;

        public bool DropReliableDataIfNewerExists => true;

        public PlayerInformation PlayerInfo { get; set; }

        public void WriteData(IPacket packet)
        {
            packet.Write(JsonConvert.SerializeObject(PlayerInfo));
        }

        public void ReadData(IPacket packet)
        {
            PlayerInfo = JsonConvert.DeserializeObject<PlayerInformation>(packet.ReadString());
        }
    }

    internal class PlayerInformation
    {
        public string PlayerName { get; set; }
        public PlayerSave playerSave{ get; set; }
    }
}

