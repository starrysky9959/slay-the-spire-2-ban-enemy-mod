# Ban Enemy Mod

`Slay the Spire 2` mod project for banning selected encounter groups from appearing in runs.

《Slay the Spire 2》Mod 项目，用于在开局前禁用指定战斗关卡（encounter groups）。

## Current setup

- Project skeleton is based on the current community `ModTemplate-StS2` structure.
- `Sts2Path` is preconfigured for this machine:
  `/mnt/d/SteamLibrary/steamapps/common/Slay the Spire 2`
- `Build` should compile the DLL and copy the mod manifest into the game's `mods/BanEnemyMod` folder.
- `Publish` additionally needs `GodotPath` configured in [Directory.Build.props](/home/starrysky/workspace/slay-the-spire-2-ban-enemy-mod/Directory.Build.props).

## Prerequisites

- `.NET SDK` with support for `net9.0`
- Access to the installed game files
- Optional: `MegaDot/Godot 4.5.1 mono` for final `.pck` export

## Useful commands

```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet restore
dotnet build
```

If `dotnet publish` is needed later, set `GodotPath` first.

## Install

1. Copy the `BanEnemyMod` folder into `Slay the Spire 2/mods/`.
2. Launch the game.
3. On character select, click `Ban Enemy` or press `B`.

## 安装

1. 将 `BanEnemyMod` 文件夹复制到 `Slay the Spire 2/mods/`。
2. 启动游戏。
3. 在角色选择界面点击 `Ban Enemy / 禁用敌人`，或直接按 `B`。

## Release Contents

A release zip should only contain:

- `BanEnemyMod.dll`
- `BanEnemyMod.json`
- `BanEnemyMod.pck`

## Release Contents（发布内容）

发布包只包含以下 3 个文件：

- `BanEnemyMod.dll`
- `BanEnemyMod.json`
- `BanEnemyMod.pck`
