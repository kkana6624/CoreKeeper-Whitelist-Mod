using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoreKeeperWhitelist
{
    public class WhitelistConfig
    {
        private readonly string _path;
        private readonly HashSet<ulong> _validSteamIds = new HashSet<ulong>();

        public WhitelistConfig(string path)
        {
            _path = path;
        }

        public void Load()
        {
            _validSteamIds.Clear();
            if (!File.Exists(_path))
            {
                File.WriteAllText(_path, "# Add SteamIDs here, one per line\n");
                Plugin.Instance.Logger.LogWarning($"Whitelist file not found. Created new one at: {_path}");
                return;
            }

            foreach (var line in File.ReadAllLines(_path))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#")) continue;

                if (ulong.TryParse(trimmed, out var steamId))
                {
                    _validSteamIds.Add(steamId);
                }
                else
                {
                    Plugin.Instance.Logger.LogWarning($"Invalid SteamID format: {trimmed}");
                }
            }
            Plugin.Instance.Logger.LogInfo($"Loaded {_validSteamIds.Count} whitelisted users.");
        }

        public bool IsWhitelisted(ulong steamId)
        {
            return _validSteamIds.Contains(steamId);
        }
    }
}