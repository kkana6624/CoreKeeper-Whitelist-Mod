using System;
using System.Collections.Generic;
using System.IO;

namespace CoreKeeperWhitelist.Configs
{
    public static class WhitelistConfig
    {
        private static HashSet<ulong> _whitelistedIds = new HashSet<ulong>();
        private static string _configPath;

        public static void Initialize(string path)
        {
            _configPath = path;
            Load();
        }

        public static void Load()
        {
            if (!File.Exists(_configPath))
            {
                CreateDefault();
            }

            _whitelistedIds.Clear();
            try
            {
                var lines = File.ReadAllLines(_configPath);
                foreach (var line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    {
                        continue;
                    }

                    if (ulong.TryParse(trimmed, out ulong steamId))
                    {
                        _whitelistedIds.Add(steamId);
                    }
                }
            }
            catch
            {
                // Simple error logging if something goes wrong, though we don't have logger ref here easily without passing it.
                // For now, we'll swallow or print to console if possible, but safe to just proceed with partial list.
            }
        }

        private static void CreateDefault()
        {
            var defaultContent = new List<string>
            {
                "# Core Keeper Whitelist Configuration",
                "# Add allowed SteamIDs (ulong) below, one per line.",
                "# Lines starting with '#' are comments.",
                ""
            };
            try 
            {
                File.WriteAllLines(_configPath, defaultContent); 
            }
            catch { }
        }

        public static bool IsWhitelisted(ulong steamId)
        {
            return _whitelistedIds.Contains(steamId);
        }
    }
}
