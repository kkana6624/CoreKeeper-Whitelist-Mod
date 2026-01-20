using BepInEx;
using HarmonyLib;
using System.IO;

namespace CoreKeeperWhitelist
{
    [BepInPlugin("corekeeper.whitelist", "Core Keeper Whitelist", "1.0.0")]
    [BepInProcess("CoreKeeperServer.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static BepInEx.Logging.ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo("Plugin Core Keeper Whitelist is loading...");

            // Config Init
            string configPath = Path.Combine(Paths.PluginPath, "whitelist.txt");
            Configs.WhitelistConfig.Initialize(configPath);

            // Harmony Patch setup
            // Harmony Patch setup
            Harmony harmony = new Harmony("corekeeper.whitelist");
            
            // Manual patching to avoid ambiguous match and platform dependencies
            var targetType = AccessTools.TypeByName("Pug.Platform.SteamNetworking");
            var connectionType = AccessTools.TypeByName("Steamworks.Data.Connection");
            var connectionInfoType = AccessTools.TypeByName("Steamworks.Data.ConnectionInfo");

            if (targetType != null && connectionType != null && connectionInfoType != null)
            {
                var targetMethod = AccessTools.Method(targetType, "OnConnecting", new[] { connectionType, connectionInfoType });
                
                if (targetMethod != null)
                {
                    var prefixMethod = AccessTools.Method(typeof(Patches.ConnectionPatch), "Prefix");
                    harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));
                    Logger.LogInfo("Successfully patched Pug.Platform.SteamNetworking.OnConnecting");
                }
                else
                {
                    Logger.LogError("Could not find method OnConnecting with specified signature.");
                }
            }
            else
            {
                 Logger.LogError($"Could not find required types for patching. Target: {targetType != null}, Connection: {connectionType != null}, Info: {connectionInfoType != null}");
            }

            Logger.LogInfo("Plugin Core Keeper Whitelist is loaded!");
        }
    }
}
