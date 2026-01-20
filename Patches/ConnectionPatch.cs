using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace CoreKeeperWhitelist.Patches
{
    // [HarmonyPatch] removed to use manual patching in Plugin.Awake for ambiguity resolution
    public static class ConnectionPatch
    {
        // Using object/Harmony loose matching to avoid direct dependency on Steamworks types
        public static bool Prefix(object connection, object info)
        {
            // Reflection to get SteamId from info.Identity.SteamId.Value
            // Structure: ConnectionInfo -> NetIdentity (Identity) -> SteamId (SteamId) -> ulong (Value)
            
            try 
            {
                if (info == null) return true;

                // info.GetType() is Steamworks.Data.ConnectionInfo
                var identityProp = info.GetType().GetProperty("Identity");
                if (identityProp == null) return true;
                
                var identityVal = identityProp.GetValue(info); // NetIdentity
                if (identityVal == null) return true;

                var steamIdProp = identityVal.GetType().GetProperty("SteamId");
                if (steamIdProp == null) return true;

                var steamIdStruct = steamIdProp.GetValue(identityVal); // SteamId struct
                
                var valueField = steamIdStruct.GetType().GetField("Value"); // ulong field in SteamId struct
                if (valueField == null)
                {
                    // Fallback: try property if Value is a property in some versions? usually field in structs
                    var valueProp = steamIdStruct.GetType().GetProperty("Value");
                    if (valueProp != null)
                    {
                        ulong steamIdRes = (ulong)valueProp.GetValue(steamIdStruct);
                        return CheckWhitelist(steamIdRes, connection);
                    }
                    return true;
                }

                ulong steamId = (ulong)valueField.GetValue(steamIdStruct);
                return CheckWhitelist(steamId, connection);

            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogError($"[ConnectionPatch] Error during reflection: {ex}");
                return true; // Fail safe, allow connection or maybe deny? Defaulting to allow to not break game if update changes things
            }
        }

        private static bool CheckWhitelist(ulong steamId, object connection)
        {
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
                
                // connection.Close(false, 0, "Not Whitelisted");
                // Reflection for Close method
                try
                {
                    var verifyMethod = connection.GetType().GetMethod("Close", new[] { typeof(bool), typeof(int), typeof(string) });
                    if (verifyMethod != null)
                    {
                        verifyMethod.Invoke(connection, new object[] { false, 0, "Not Whitelisted" });
                    }
                }
                catch (System.Exception ex)
                {
                     Plugin.Log.LogError($"[ConnectionPatch] Error closing connection: {ex}");
                }

                return false;
            }
        }
    }
}
