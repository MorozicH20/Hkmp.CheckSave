# Hkmp.CheckSave

### Built With

* [DotNet Framwork 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)
* [HKMP](https://github.com/Extremelyd1/HKMP)
* [HKMirror](https://github.com/TheMulhima/HKMirror/)
* [HK-Modding](https://hk-modding.github.io/api/api/index.html)

### Description
This is an HKMP Addon that provides controls to verify player save consistency between connecting clients and the server. It allows you to configure how closely a client's save data must match that of the server. Additionally, you can specify whether mismatching clients should be automatically kicked from the server. This tool is designed to ensure that all clients and servers are consistent in terms of player saves, making the setup of multiplayer sessions relying on player items and abilities smoother.

### Installation
Prerequisites
This addon requires the HK-Modding API, HKMP and HKMirror to be installed. The referenced version of HKMP will be included in each release note but is generally the latest version.

### Manual installation
Client Installation
Get the latest HKMP.SaveDiff.zip from the current release and add all its contents to a mod folder in your mods directory like:

### Configuration
All configurations are hot-reload ready. If you make modifications to the config file, the server will attempt to hot reload the changes in real-time.

### CheckSave_config.json and AllowedSave.json
Upon first starting up, a files "CheckSave_config.json", "AllowedSave.json" will be generated inside the server/mod directory. This is a simple JSON files that can be modified in real-time to update server settings.

### CheckSave_config.json

| Configuration Name  | Description                                                                                                                                                                                                                                           | Data type | Default |
|---------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|---------|
| KickOnMistmatch     | Flag for whether the server will auto-kick client who connect with a mismatched mod list. Currently the kick will happen about 500ms after the connection in order to not trap the connection in a weird state and leave behind ghosts of the player. | Boolean   | False   |

### AllowedSave.json

| Configuration Name  | Description                                         | Data type   | Default |
|---------------------|-----------------------------------------------------|-------------|---------|
| maxHealth           | a variable that stores the allowed amount of health | Integer     | 0       |
| maxMP               | a variable that stores the allowed amount of soul   | Integer     | 0       |
| geo                 | a variable that stores the allowed amount of geo    | Integer     | -1      |
| requiredCharms      | list of required charms                             | Charm[]     | null    |
| requiredSkills      | list of required skill                              | Skill[]     | null    |
| bannedCharms        | list of banned charms                               | Charm[]     | null    |
| bannedSkills        | list of banned skill                                | Skill[]     | null    |
```json
example:\
{
  "maxHealth": 9,
  "maxMP": 99,
  "geo": -1,
  "requiredCharms": [
    "LongNail",
    "ShamanStone"
  ],
  "requiredSkills": [
    "ShadeSoul",
    "DescendingDark",
    "AbyssShriek"
  ],
  "bannedCharms": [
    "WaywardCompass",
    "DefendersCrest"
  ],
  "bannedSkills": [
    "CycloneSlash",
    "DashSlash",
    "GreatSlash"
  ]
}
```
### Enums for arrays
|  public enum Charm  | public enum Skill |
| ------------------- | ----------------- |
| GatheringSwarm      | VengefulSpirit    |
| WaywardCompass      | DesolateDive      |
| GrubSong            | HowlingWraiths    |
| StalwartShell       | ShadeSoul         |
| BaldurShell         | DescendingDark    |
| FuryOfTheFallen     | AbyssShriek       |
| QuickFocus          | MothwingCloak     |
| LifebloodHeart      | MantisClaw        |
| LifebloodCore       | CrystalHeart      |
| DefendersCrest      | MonarchWings      |
| Flukenest           | IsmasTear         |
| ThornsOfAgony       | DreamNail         |
| MarkOfPride         | CycloneSlash      |
| SteadyBody          | DashSlash         |
| HeavyBlow           | GreatSlash        |
| SharpShadow         |   |
| SporeShroom         |   |
| LongNail            |   |
| ShamanStone         |   |
| SoulCatcher         |   |
| SoulEater           |   |
| GlowingWomb         |   |
| UnbreakableHeart    |   |
| UnbreakableGreed    |   |
| UnbreakableStrength |   |
| NailmastersGlory    |   |
| JonisBlessing       |   |
| ShapeOfUnn          |   |
| Hiveblood           |   |
| Dreamwielder        |   |
| Dashmaster          |   |
| QuickSlash          |   |
| SpellTwister        |   |
| DeepFocus           |   |
| GrubberflysElegy    |   |
| Kingsoul            |   |
| Sprintmaster        |   |
| Dreamshield         |   |
| Weaversong          |   |
| Grimmchild          |   |
| CarefreeMelody      |   |

