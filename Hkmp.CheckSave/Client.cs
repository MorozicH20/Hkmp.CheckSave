using Hkmp.Api.Client;
using Hkmp.CheckSave.Services;


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