// Decompiled with JetBrains decompiler
// Type: StardewValley.FarmerPair
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace StardewValley;

public struct FarmerPair
{
  public long Farmer1;
  public long Farmer2;

  public static FarmerPair MakePair(long f1, long f2)
  {
    return new FarmerPair()
    {
      Farmer1 = Math.Min(f1, f2),
      Farmer2 = Math.Max(f1, f2)
    };
  }

  public bool Contains(long f) => this.Farmer1 == f || this.Farmer2 == f;

  public long GetOther(long f) => this.Farmer1 == f ? this.Farmer2 : this.Farmer1;

  public bool Equals(FarmerPair other)
  {
    return this.Farmer1 == other.Farmer1 && this.Farmer2 == other.Farmer2;
  }

  public override bool Equals(object obj) => obj is FarmerPair other && this.Equals(other);

  public override int GetHashCode()
  {
    return this.Farmer1.GetHashCode() ^ this.Farmer2.GetHashCode() << 16 /*0x10*/;
  }
}
