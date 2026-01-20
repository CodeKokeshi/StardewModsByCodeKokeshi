// Decompiled with JetBrains decompiler
// Type: StardewValley.IAnimalLocation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Network;
using System;

#nullable disable
namespace StardewValley;

[Obsolete("All locations allow animals now, so there's no need to check for this interface anymore.")]
public interface IAnimalLocation
{
  NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>> Animals { get; }

  bool CheckPetAnimal(Vector2 position, Farmer who);

  bool CheckPetAnimal(Rectangle rect, Farmer who);

  bool CheckInspectAnimal(Vector2 position, Farmer who);

  bool CheckInspectAnimal(Rectangle rect, Farmer who);
}
