// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.InternalConstants
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace Ionic.Zlib;

internal static class InternalConstants
{
  internal static readonly int MAX_BITS = 15;
  internal static readonly int BL_CODES = 19;
  internal static readonly int D_CODES = 30;
  internal static readonly int LITERALS = 256 /*0x0100*/;
  internal static readonly int LENGTH_CODES = 29;
  internal static readonly int L_CODES = InternalConstants.LITERALS + 1 + InternalConstants.LENGTH_CODES;
  internal static readonly int MAX_BL_BITS = 7;
  internal static readonly int REP_3_6 = 16 /*0x10*/;
  internal static readonly int REPZ_3_10 = 17;
  internal static readonly int REPZ_11_138 = 18;
}
