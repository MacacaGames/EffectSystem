See [Document](https://macacagames.github.io/EffectSystem/) for more detail.

# Overview
The table is used to collage various buffs, debuffs or skill effects based on EffectInfo as the basic unit, and it's convenient to use between different projects. Engineers only need to implement EffectType and register the timing of activation and deactivation.

---------
# Features
- Add or adjust skills through Excel
- Combine different skills through EffectSubInfo
---

# Installation
## Option 1: Install via OpenUPM (recommended)

```sh
openupm add com.macacagames.effectsystem
```

## Option 2: Unity Package file
Add to the editor's manifest.json:
```json
{
    "dependencies": {
        "com.macacagames.utility": "https://github.com/MacacaGames/MacacaUtility.git",
        "com.macacagames.effectsystem": "https://github.com/MacacaGames/EffectSystem.git"
    }
}
```

## Option 3: Git SubModule
```bash
git submodule add https://github.com/MacacaGames/EffectSystem.git Assets/MacacaEffectSystem
```
Note: EffectSystem is dependent on MacacaUtility, so MacacaUtility must also be added to the git submodule.
```bash
git submodule add https://github.com/MacacaGames/MacacaUtility.git Assets/MacacaUtility
```
---
# Setup
## Excel Data
    - [Effect Data Sample](https://docs.google.com/spreadsheets/d/1zYKiOlThAqTMVuUPHcxeQGX7rBRLp5E-49ci-GCZBa8/edit?usp=drive_link)
    - Add or remove the content of each field's Enum according to different needs
## Add SkillResource
    - AssetMenu path : _"GameResource/SkillResource"_
    - Add skill icon sprite
    - Paste the Json data of Enums backe into Bake All Effect Enum, and press Invoke
## Add EffectResource
    - AssetMenu path : _"GameResource/EffectResource"_
    - Paste EffectView Json
    - Add a path to store new special effects, then press the Get All View Prefab button
---
# Usage

## Implement EffectType
    - Inheritance EffectBase and implement the required effects

## Inheritance and implement IEffectSystemData
    - Initialize Effect table data
    - Query EffectInfo
    - Add all implemented EffectTypes to the dictionary

## Inheritance and implement IEffectableObject
    - The entity used to add skills

## EffectManager
    - Attach Effect to IEffectableObject or remove it
    - Query the Effect instance on IEffectableObject
    - Get skill description with I2


# Code Generate

The MessagePack.Csharp should do a code generate first to make the EffectInfo available to use on AOT Platform

First, use the mpc tool to generate the Resolver, here is some example: 
For more detail, see the MessagePack Document
```bash
dotnet new tool-manifest
dotnet tool install MessagePack.Generator
dotnet tool run mpc -i {PATH_TO_YOUR_EFFECTPACKAGE_MODEL_FOLEDR} -o ./Assets/EffectSystemResources/EffectSystem.Generated.cs -r EffectSystemResolver -n MacacaGames.EffectSystem

## Example
## dotnet tool run mpc -i ./MacacaPackages/EffectSystem/Model -o ./Assets/EffectSystemResources/EffectSystem.Generated.cs -r EffectSystemResolver -n MacacaGames.EffectSystem
```

And add into your StaticCompositeResolver
```csharp
StaticCompositeResolver.Instance.Register(
    MacacaGames.EffectSystem.Resolvers.EffectSystemResolver.Instance,
);
```