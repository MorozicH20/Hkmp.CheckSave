using System.IO;
using System.Reflection;
using Hkmp.Api.Server;
using Hkmp.CheckSave.Models;

using Hkmp.CheckSave.Services;
using Newtonsoft.Json;

namespace Hkmp.CheckSave
{
    /// <summary>
    /// Server addon for checking players save difference
    /// </summary>
    public class Server : ServerAddon
    {
        /// <inheritdoc />
        public override void Initialize(IServerApi serverApi)
        {
            Logger.Info("Server initialized");

            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configPath = Path.Combine(dllDir ?? string.Empty, "CheckSave_config.json");
            var AllowedSavePath = Path.Combine(dllDir ?? string.Empty, "AllowedSave.json");

            var LogsPath = Path.Combine(dllDir ?? string.Empty, "Logs.txt");
            if (!File.Exists(configPath))
            {
                

                File.WriteAllText(configPath, JsonConvert.SerializeObject(new Configuration(), Newtonsoft.Json.Formatting.Indented));
                Logger.Info("Created configuration file");
                
                File.WriteAllText(AllowedSavePath, JsonConvert.SerializeObject(new AllowedSave(), Newtonsoft.Json.Formatting.Indented));
                Logger.Info("Created AllowedSave file");
                
            }
            File.WriteAllText(LogsPath, "Server started:\n\n");
            Logger.Info("Created Logs file");

            // ReSharper disable once ObjectCreationAsStatement
            new ServerNetService(Logger, this, serverApi);
        }

        /// <inheritdoc />
        protected override string Name => ModInfo.Name;

        /// <inheritdoc />
        protected override string Version => ModInfo.Version;

        /// <inheritdoc />
        public override bool NeedsNetwork => true;
    }
}