// Decompiled with JetBrains decompiler
// Type: StardewValley.CollisionMask
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley;

[Flags]
public enum CollisionMask : byte
{
  None = 0,
  Buildings = 1,
  Characters = 2,
  Farmers = 4,
  Flooring = 8,
  Furniture = 16, // 0x10
  Objects = 32, // 0x20
  TerrainFeatures = 64, // 0x40
  LocationSpecific = 128, // 0x80
  All = LocationSpecific | TerrainFeatures | Objects | Furniture | Flooring | Farmers | Characters | Buildings, // 0xFF
}
