using BepInEx;
using HarmonyLib;
using System.IO;

namespace CoreKeeperWhitelist
{
    [BepInPlugin("com.kkana6624.corekeeper.whitelist", "Core Keeper Whitelist", "1.0.0")]
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
            Harmony.CreateAndPatchAll(typeof(Patches.ConnectionPatch));

            Logger.LogInfo("Plugin Core Keeper Whitelist is loaded!");
        }
    }
}
