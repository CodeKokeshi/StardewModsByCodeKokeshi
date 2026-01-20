// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetEvents.MailType
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Network.NetEvents;

/// <summary>The mail lists for a player.</summary>
public enum MailType : byte
{
  /// <summary>Mail in the mailbox now.</summary>
  Now,
  /// <summary>Mail queued to add to the mailbox tomorrow.</summary>
  Tomorrow,
  /// <summary>Mail that has already been received.</summary>
  Received,
  /// <summary>All mail types.</summary>
  All,
}
