﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hkmp.Api.Server;
using Hkmp.Logging;
using Hkmp.CheckSave.Models;
using Hkmp.Networking.Packet.Data;
using Newtonsoft.Json;
using Hkmp.CheckSave.Packets;
using static Hkmp.CheckSave.Models.AllowedSave;

namespace Hkmp.CheckSave.Services
{
    internal class ServerNetService
    {
        FileEdit logs = new FileEdit();


        private AllowedSave _AllowedSave;
        private Configuration _configuration;

        // This is technically IDisposable but this is a notional singleton so we should be fine.
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _playerSaveWatcher;
        private readonly ILogger _logger;

        private static readonly string DllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private const string PlayerSaveFileName = "AllowedSave.json";
        private const string ConfigFileName = "CheckSave_config.json";

        public ServerNetService(ILogger logger, Server addon, IServerApi serverApi)
        {
            _logger = logger;
            var receiver = serverApi.NetServer.GetNetworkReceiver<PlayerSavePacketId>(addon, (_) => new PlayerSavePacket());
            receiver.RegisterPacketHandler<PlayerSavePacket>(
                PlayerSavePacketId.PlayerSaveClientData, (id, data) => HandleSave(id, data, serverApi));

            _playerSaveWatcher = new FileSystemWatcher(DllDirectory ?? string.Empty);
            _playerSaveWatcher.IncludeSubdirectories = false;
            _playerSaveWatcher.Changed += OnFileChanged;
            _playerSaveWatcher.Created += OnFileChanged;
            _playerSaveWatcher.Renamed += OnFileChanged;
            _playerSaveWatcher.EnableRaisingEvents = true;
            HandleConfigChange();
            HandlePlayerSaveChange();
        }

        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // just do a best attempt at getting the file before passing it off to readers
            const int maxAttempts = 50;
            var attempts = 0;
            while (IsFileLocked(new FileInfo(e.FullPath)) && attempts <= maxAttempts)
            {
                // We don't want to mess around with half copied files, wait for the lock to end
                await Task.Delay(10);
                attempts++;
            }

            if (attempts > maxAttempts)
            {
                // We'll call this unreadable
                _logger.Warn($"Was unable to read locked file: {e.Name}");
                return;
            }

            switch (Path.GetFileName(e.Name))
            {
                case PlayerSaveFileName:
                    HandlePlayerSaveChange();
                    break;
                case ConfigFileName:
                    HandleConfigChange();
                    break;
            }
        }

        private void HandlePlayerSaveChange()
        {
            if (!File.Exists(Path.Combine(DllDirectory, PlayerSaveFileName)))
            {
                _logger.Warn("No PlayerSave Provided.");
                return;
            }

            var fileContents = ReadAllTextNoLock(Path.Combine(DllDirectory, PlayerSaveFileName));
            if (fileContents == null)
            {
                return;
            }

            _AllowedSave = JsonConvert.DeserializeObject<AllowedSave>(fileContents);
            _logger.Info("AllowedSave Updated");
        }

        private void HandleConfigChange()
        {
            if (!File.Exists(Path.Combine(DllDirectory, ConfigFileName)))
            {
                _logger.Warn("No Configuration Provided.");
                return;
            }

            var fileContents = ReadAllTextNoLock(Path.Combine(DllDirectory, ConfigFileName));
            if (fileContents == null)
            {
                return;
            }

            _configuration = JsonConvert.DeserializeObject<Configuration>(fileContents);
            _logger.Info("Configuration Updated");
        }

        private void HandleSave(ushort id, PlayerSavePacket data, IServerApi serverApi)
        {
            PlayerSave clientSave = null;
            logs.Write("\nStart read player data:");
            try
            {
                clientSave = data.PlayerInfo.playerSave;
                logs.Write("\tread player data successfully");

            }
            catch (Exception ex)
            {
                logs.Write($"\t[ERROR] {ex}");

            }


            SaveInfo SaveInfo = null;
            logs.Write("\nStart CalculateSaveDiff:");
            logs.Write($"\n\tAllowedSave:\n{JsonConvert.SerializeObject(_AllowedSave, Formatting.Indented)}");
            logs.Write($"\n\tclientSave\n{JsonConvert.SerializeObject(clientSave, Formatting.Indented)}");
            try
            {
                SaveInfo = CalculateSaveDiff(_AllowedSave, clientSave);
                logs.Write("\tCalculateSaveDiff successfully");

            }
            catch (Exception ex)
            {
                logs.Write($"\t[ERROR] {ex}");

            }

            bool isMatch = false;
            logs.Write("\nStart  CheckIsMatch:");
            try
            {
                isMatch = CheckIsMatch(SaveInfo);
                logs.Write("\tCheckIsMatch successfully");

            }
            catch (Exception ex)
            {
                logs.Write($"\t[ERROR] {ex}");

            }


            _logger.Info(!isMatch
                    ? $"{data.PlayerInfo.PlayerName}'s saves DO NOT match!"
                    : $"{data.PlayerInfo.PlayerName} joined with matching save!");

            MessageModDiscrepancies(serverApi, id, SaveInfo, isMatch);

            if (!isMatch && _configuration.KickOnMistmatch)
            {
                // Disconnecting in real time causes a weird race condition as the player is still in it's connection event.
                // delaying this action arbitrarily fixes it.
                foreach(var player in serverApi.ServerManager.Players.ToList())
                {
                    if (player.IpAddressString == "127.0.0.1")
                    {  
                        if(data.PlayerInfo.PlayerName != player.Username)
                        {
                            logs.Write($"{serverApi.ServerManager.Players.ToList()[0].Username}, {data.PlayerInfo.PlayerName}");
                            Task.Run(async () =>
                            {
                                await Task.Delay(500).ConfigureAwait(false);
                                serverApi.ServerManager.DisconnectPlayer(id, DisconnectReason.Kicked);
                            });
                        }
                    }
                }
                   
            }
        }

        private static Models.SaveInfo CalculateSaveDiff(AllowedSave allowedSaveServer, PlayerSave clientSave)
        {
            FileEdit logs = new FileEdit();

            bool equalityHealth = true;
            if (allowedSaveServer.maxHealth > 0)
                equalityHealth = allowedSaveServer.maxHealth == clientSave.maxHealth;


            bool equalityMP = true;
            if (allowedSaveServer.maxMP > 0)
                equalityMP = allowedSaveServer.maxMP == clientSave.maxMP;


            bool equalityGeo = true;
            if (allowedSaveServer.geo >= 0)
                equalityGeo = allowedSaveServer.geo == clientSave.geo;


            List<Charm> extraCharms = new List<Charm>();
            List<Charm> missingCharms = new List<Charm>();

            List<Skill> extraSkills = new List<Skill>();
            List<Skill> missingSkills = new List<Skill>();

            if (clientSave.Charms != null)
            {

                if (allowedSaveServer.BannedCharms != null && allowedSaveServer.BannedCharms.Length>0)
                    foreach (var charm in clientSave.Charms)
                    {

                        if (allowedSaveServer.BannedCharms.Contains(charm))
                        {
                            extraCharms.Add(charm);
                        }

                    }

                if (allowedSaveServer.RequiredCharms != null && allowedSaveServer.RequiredCharms.Length > 0)
                    foreach (var charm in allowedSaveServer.RequiredCharms)
                    {
                        if (!clientSave.Charms.Contains(charm))
                        {
                            missingCharms.Add(charm);
                        }
                    }
            }


            if (clientSave.Skills != null)
            {
                if (allowedSaveServer.BannedSkills != null && allowedSaveServer.BannedSkills.Length > 0)
                    foreach (var skill in clientSave.Skills)
                    {
                        if (allowedSaveServer.BannedSkills.Contains(skill))
                        {
                            extraSkills.Add(skill);
                        }
                    }

                if (allowedSaveServer.RequiredSkills != null && allowedSaveServer.RequiredSkills.Length > 0)
                    foreach (var skill in allowedSaveServer.RequiredSkills)
                    {
                        if (!clientSave.Skills.Contains(skill))
                        {
                            missingSkills.Add(skill);
                        }
                    }
            }

            return new Models.SaveInfo
            {
                EqualityHealth = equalityHealth,
                EqualityMP = equalityMP,
                EqualityGeo = equalityGeo,
                ExtraCharms = extraCharms,
                MissingCharms = missingCharms,
                ExtraSkills = extraSkills,
                MissingSkills = missingSkills

            };
        }

        private static bool CheckIsMatch(Models.SaveInfo info)
        {
            return info.EqualityHealth && info.EqualityMP && info.EqualityGeo && !info.MissingCharms.Any() && !info.ExtraCharms.Any() && !info.MissingSkills.Any() && !info.ExtraSkills.Any();
        }

        private static void MessageModDiscrepancies(IServerApi api, ushort playerId, Models.SaveInfo info, bool isMatch)
        {
            var playerFound = api.ServerManager.TryGetPlayer(playerId, out var player);

            if (!playerFound)
            {
                // Already DC'd
                return;
            }

            api.ServerManager.SendMessage(player, isMatch ? "Save Diff Check: Match!" : "Save Diff Check: Mismatch!");

            if (!info.EqualityHealth)
            {
                api.ServerManager.SendMessage(player, "your health is higher/lower than acceptable");
            }
            if (!info.EqualityMP)
            {
                api.ServerManager.SendMessage(player, "your soul stock higher/lower than acceptable");
            }
            if (!info.EqualityGeo)
            {
                api.ServerManager.SendMessage(player, "your geo stock is higher/lower than acceptable");
            }
            if (info.ExtraCharms.Any())
            {
                api.ServerManager.SendMessage(player, "Extra Charms:");
                foreach (var charm in info.ExtraCharms)
                {
                    api.ServerManager.SendMessage(player, $"- {charm.ToString()}");
                }
            }

            if (info.MissingCharms.Any())
            {
                api.ServerManager.SendMessage(player, "Missing Charms:");
                foreach (var charm in info.MissingCharms)
                {
                    api.ServerManager.SendMessage(player, $"- {charm.ToString()}");
                }
            }

            if (info.ExtraSkills.Any())
            {
                api.ServerManager.SendMessage(player, "Extra Skills:");
                foreach (var skill in info.ExtraSkills)
                {
                    api.ServerManager.SendMessage(player, $"- {skill.ToString()}");
                }
            }

            if (info.MissingSkills.Any())
            {
                api.ServerManager.SendMessage(player, "Missing Skills:");
                foreach (var skill in info.MissingSkills)
                {
                    api.ServerManager.SendMessage(player, $"- {skill.ToString()}");
                }
            }
        }

        private string ReadAllTextNoLock(string path)
        {
            try
            {
                using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(file))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                _logger.Error($"Unable to read text from file {path}. File is locked.");
                return null;
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (var _ = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }

            //file is not locked
            return false;
        }
    }
}