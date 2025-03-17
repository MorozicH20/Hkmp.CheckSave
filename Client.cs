using System.IO;
using System.Linq;
using System.Reflection;
using Hkmp.Api.Client;
//using Hkmp.ModDiff.Extensions;
using Hkmp.CheckSave.Models;
using Hkmp.CheckSave.Services;
//using Modding;
using Newtonsoft.Json;
using HKMirror;
using Hkmp.CheckSave.Models;

namespace Hkmp.CheckSave
{
    /// <summary>
    /// Client addon for checking mod load difference
    /// </summary>
    public class Client : ClientAddon
    {
        /// <inheritdoc />
        public override void Initialize(IClientApi clientApi)
        {
            Logger.Info("Client initialized");



            PlayerSave curentSave = new PlayerSave();

            var dllDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            File.WriteAllText(Path.Combine(dllDir ?? string.Empty, "lastSave.json"), JsonConvert.SerializeObject(curentSave, Formatting.Indented));
            Logger.Info("lastSave.json created");

            // ReSharper disable once ObjectCreationAsStatement
            new ClientNetService(Logger, this, clientApi);
        }

        /// <inheritdoc />
        protected override string Name => ModInfo.Name;

        /// <inheritdoc />
        protected override string Version => ModInfo.Version;

        /// <inheritdoc />
        public override bool NeedsNetwork => true;
    }
}