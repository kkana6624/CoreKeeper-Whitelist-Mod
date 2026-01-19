using HarmonyLib;
using Pug.Platform;
using Steamworks;
using Steamworks.Data;
using System;

namespace CoreKeeperWhitelist.Patches
{
    [HarmonyPatch(typeof(SteamNetworking), nameof(SteamNetworking.OnConnecting))]
    public class ConnectionPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(SteamNetworking __instance, Connection connection, ConnectionInfo info)
        {
            try
            {
                ulong steamId = info.Identity.SteamId.Value;

                if (Plugin.Config.IsWhitelisted(steamId))
                {
                    Plugin.Instance.Logger.LogInfo($"[Whitelist] Accepted connection from {steamId}");
                    return true; 
                }
                else
                {
                    Plugin.Instance.Logger.LogWarning($"[Whitelist] Rejected connection from {steamId} (Not in whitelist)");
                    
                    connection.Close(false, 0, "Not Whitelisted");
                    
                    return false; 
                }
            }
            catch (Exception ex)
            {
                Plugin.Instance.Logger.LogError($"[Whitelist] Error in OnConnecting patch: {ex}");
                return false; 
            }
        }
    }
}