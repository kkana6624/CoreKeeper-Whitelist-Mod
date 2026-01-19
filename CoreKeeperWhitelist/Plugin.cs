using BepInEx;
using HarmonyLib;
using System.IO;

namespace CoreKeeperWhitelist
{
    [BepInPlugin("com.yourname.ckwhitelist", "Core Keeper Whitelist", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static WhitelistConfig Config { get; private set; }
        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;
            Logger.LogInfo($"Plugin com.yourname.ckwhitelist is loading...");

            string configPath = Path.Combine(Paths.PluginPath, "whitelist.txt");
            
            Config = new WhitelistConfig(configPath);
            Config.Load();

            _harmony = new Harmony("com.yourname.ckwhitelist");
            _harmony.PatchAll(typeof(Patches.ConnectionPatch));

            Logger.LogInfo($"Plugin com.yourname.ckwhitelist is loaded!");
        }
    }
}