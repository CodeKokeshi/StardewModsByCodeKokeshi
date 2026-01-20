// Decompiled with JetBrains decompiler
// Type: StardewValley.TerrainFeatures.Leaf
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;

#nullable disable
namespace StardewValley.TerrainFeatures;

public class Leaf
{
  public Vector2 position;
  public float rotation;
  public float rotationRate;
  public float yVelocity;
  public int type;

  public Leaf(Vector2 position, float rotationRate, int type, float yVelocity)
  {
    this.position = position;
    this.rotationRate = rotationRate;
    this.type = type;
    this.yVelocity = yVelocity;
  }
}
