// Decompiled with JetBrains decompiler
// Type: StardewValley.Quests.NetDescriptionElementRef
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;

#nullable disable
namespace StardewValley.Quests;

public class NetDescriptionElementRef : 
  NetExtendableRef<DescriptionElement, NetDescriptionElementRef>
{
  public NetDescriptionElementRef() => this.Serializer = DescriptionElement.serializer;

  public NetDescriptionElementRef(DescriptionElement value)
    : base(value)
  {
    this.Serializer = DescriptionElement.serializer;
  }
}
