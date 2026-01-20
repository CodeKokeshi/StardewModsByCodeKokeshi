// Decompiled with JetBrains decompiler
// Type: StardewValley.TilePositionComparer
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public class TilePositionComparer : IEqualityComparer<Vector2>
{
  public bool Equals(Vector2 a, Vector2 b) => a.Equals(b);

  public int GetHashCode(Vector2 a) => (int) (ushort) a.X | (int) (ushort) a.Y << 16 /*0x10*/;
}
