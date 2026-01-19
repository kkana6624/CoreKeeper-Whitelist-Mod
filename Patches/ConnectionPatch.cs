using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace CoreKeeperWhitelist.Patches
{
    [HarmonyPatch(typeof(Pug.Platform.SteamNetworking), "OnConnecting", new[] { typeof(Connection), typeof(ConnectionInfo) })]
    public static class ConnectionPatch
    {
        public static bool Prefix(Connection connection, ConnectionInfo info)
        {
            ulong steamId = info.Identity.SteamId.Value;

            if (Configs.WhitelistConfig.IsWhitelisted(steamId))
            {
                // Allowed
                Plugin.Log.LogInfo($"[Whitelist] Accepted connection from {steamId}");
                return true;
            }
            else
            {
                // Denied
                Plugin.Log.LogWarning($"[Whitelist] Rejected connection from {steamId} (Not in whitelist)");
                connection.Close(false, 0, "Not Whitelisted");
                return false;
            }
        }
    }
}
