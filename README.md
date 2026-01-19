# Core Keeper Dedicated Server Whitelist Mod (Mono / BepInEx v5)

This mod implements a strict whitelist system for Core Keeper Dedicated Servers running on **Mono** (Standard Unity Runtime).
It intercepts connections at the steam networking level and rejects any SteamID not present in the whitelist.

## Requirements

- **Server**: Core Keeper Dedicated Server (Windows/Linux)
- **Mod Loader**: [BepInEx 5.4.22 (x64)](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.22)
- **Runtime**: Mono / .NET Standard 2.1

## Installation

### 1. Prerequisite: Conflict Resolution
The Core Keeper server ships with its own `0Harmony.dll` and other libraries in `CoreKeeperServer_Data/Managed/` that conflict with BepInEx.
**You must rename or remove the following files from `CoreKeeperServer_Data/Managed/` before installing BepInEx:**
- `0Harmony.dll`
- `Mono.Cecil.dll` (if present)

### 2. Install BepInEx
- Extract BepInEx v5 (x64) into the server root directory.
- Run the server once to generate configuration files.

### 3. Install Mod
- Build the project (`dotnet build -c Release`).
- Copy `bin/Release/netstandard2.1/CoreKeeperWhitelist.dll` to `BepInEx/plugins/`.

### 4. Configure Whitelist
- Create `whitelist.txt` in `BepInEx/plugins/`.
- Add one SteamID per line.

```text
# whitelist.txt example
76561198000000000
76561198000000001
```

## Logs
Check `BepInEx/LogOutput.log` for connection attempts:
- `[Info] [Whitelist] Accepted connection from <SteamID>`
- `[Warning] [Whitelist] Rejected connection from <SteamID> (Not in whitelist)`
