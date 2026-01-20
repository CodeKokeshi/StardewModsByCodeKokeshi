// Decompiled with JetBrains decompiler
// Type: StardewValley.ISittable
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public interface ISittable
{
  bool IsSittingHere(Farmer who);

  bool HasSittingFarmers();

  void RemoveSittingFarmer(Farmer farmer);

  int GetSittingFarmerCount();

  List<Vector2> GetSeatPositions(bool ignore_offsets = false);

  Vector2? GetSittingPosition(Farmer who, bool ignore_offsets = false);

  Vector2? AddSittingFarmer(Farmer who);

  int GetSittingDirection();

  Rectangle GetSeatBounds();

  bool IsSeatHere(GameLocation location);
}
