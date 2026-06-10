# Runic Voice

Runic Voice adds Valheim-native voice abilities with original runic-themed names and mechanics.

## Current Ability

### Bellow of the Mountain

A forward cone bellow that knocks enemies back and deals minor blunt damage.

## Controls

Press `Z` by default to cast the active Runic Voice.

The key can be changed in the BepInEx config file after the mod has launched once.

## Config Options

### General

- `EnableMod`
- `EnableDebugLogs`

### Input

- `ShoutKey`

### Bellow of the Mountain

- `EnableBellowOfTheMountain`
- `BellowStaminaCost`
- `BellowCooldown`
- `BellowRange`
- `BellowConeAngle`
- `BellowBluntDamage`
- `BellowKnockbackForce`
- `BellowAffectsPlayers`

## Installation With A Mod Manager

Install `RunicVoice` with your Valheim mod manager, then make sure BepInExPack for Valheim and Jotunn are installed.

## Manual Installation

Install BepInExPack for Valheim and Jotunn, then place `RunicVoice.dll` in a folder under `BepInEx/plugins`.

## Compatibility Notes

Runic Voice does not modify base game files. Version `0.1.0` is focused on local and single-player use. Multiplayer server-authoritative handling is planned for a later release.

## Future Plans

- Server-authoritative multiplayer validation.
- Original VFX and SFX.
- Additional Valheim-native runic voice abilities.
- Ability unlock progression after the core system is stable.
