// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.SpecialItem
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects;

public class SpecialItem : Item
{
  public const int skullKey = 4;
  public const int clubCard = 2;
  public const int specialCharm = 3;
  public const int backpack = 99;
  public const int magnifyingGlass = 5;
  public const int darkTalisman = 6;
  public const int magicInk = 7;
  [XmlElement("which")]
  public readonly NetInt which = new NetInt();
  /// <summary>The backing field for <see cref="P:StardewValley.Objects.SpecialItem.displayName" />.</summary>
  [XmlIgnore]
  private string _displayName;

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(O)";

  /// <summary>The cached value for <see cref="P:StardewValley.Objects.SpecialItem.DisplayName" />.</summary>
  [XmlIgnore]
  private string displayName
  {
    get
    {
      if (string.IsNullOrEmpty(this._displayName))
      {
        switch (this.which.Value)
        {
          case 2:
            this._displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13089");
            break;
          case 3:
            this._displayName = Game1.content.LoadString("Strings\\Objects:SpecialCharm");
            break;
          case 4:
            this._displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13088");
            break;
          case 5:
            this._displayName = Game1.content.LoadString("Strings\\Objects:MagnifyingGlass");
            break;
          case 6:
            this._displayName = Game1.content.LoadString("Strings\\Objects:DarkTalisman");
            break;
          case 7:
            this._displayName = Game1.content.LoadString("Strings\\Objects:MagicInk");
            break;
          case 99:
            this._displayName = Game1.player.maxItems.Value != 36 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708") : Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8709");
            break;
        }
      }
      return this._displayName;
    }
    set
    {
      if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(this._displayName))
      {
        switch (this.which.Value)
        {
          case 2:
            this._displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13089");
            break;
          case 3:
            this._displayName = Game1.content.LoadString("Strings\\Objects:SpecialCharm");
            break;
          case 4:
            this._displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13088");
            break;
          case 5:
            this._displayName = Game1.content.LoadString("Strings\\Objects:MagnifyingGlass");
            break;
          case 6:
            this._displayName = Game1.content.LoadString("Strings\\Objects:DarkTalisman");
            break;
          case 7:
            this._displayName = Game1.content.LoadString("Strings\\Objects:MagicInk");
            break;
          case 99:
            if (Game1.player.maxItems.Value == 36)
            {
              this._displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8709");
              break;
            }
            this._displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:GameLocation.cs.8708");
            break;
        }
      }
      else
        this._displayName = value;
    }
  }

  public SpecialItem()
  {
    this.which.Value = this.which.Value;
    if (!(this.netName.Value == "Error Item") && this.Name.Length >= 1)
      return;
    switch (this.which.Value)
    {
      case 2:
        this.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13089");
        break;
      case 3:
        this.displayName = Game1.content.LoadString("Strings\\Objects:SpecialCharm");
        break;
      case 4:
        this.displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13088");
        break;
      case 5:
        this.displayName = Game1.content.LoadString("Strings\\Objects:MagnifyingGlass");
        break;
      case 6:
        this.displayName = Game1.content.LoadString("Strings\\Objects:DarkTalisman");
        break;
      case 7:
        this.displayName = Game1.content.LoadString("Strings\\Objects:MagicInk");
        break;
    }
  }

  public SpecialItem(int which, string name = "")
    : this()
  {
    this.which.Value = which;
    this.Name = name;
    if (name.Length >= 1)
      return;
    switch (which)
    {
      case 2:
        this.Name = "Club Card";
        break;
      case 3:
        this.Name = Game1.content.LoadString("Strings\\Objects:SpecialCharm");
        break;
      case 4:
        this.Name = "Skull Key";
        break;
      case 5:
        this.Name = Game1.content.LoadString("Strings\\Objects:MagnifyingGlass");
        break;
      case 6:
        this.Name = Game1.content.LoadString("Strings\\Objects:DarkTalisman");
        break;
      case 7:
        this.Name = Game1.content.LoadString("Strings\\Objects:MagicInk");
        break;
    }
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.which, "which");
  }

  public void actionWhenReceived(Farmer who)
  {
    switch (this.which.Value)
    {
      case 3:
        who.hasSpecialCharm = true;
        break;
      case 4:
        who.hasSkullKey = true;
        who.addQuest("19");
        break;
      case 5:
        who.hasMagnifyingGlass = true;
        break;
      case 6:
        who.hasDarkTalisman = true;
        break;
      case 7:
        who.hasMagicInk = true;
        break;
    }
  }

  public TemporaryAnimatedSprite getTemporarySpriteForHoldingUp(Vector2 position)
  {
    if (this.which.Value == 99)
      return new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(Game1.player.maxItems.Value == 36 ? 268 : 257, 1436, Game1.player.maxItems.Value == 36 ? 11 : 9, 13), position + new Vector2(16f, 0.0f), false, 0.0f, Color.White)
      {
        scale = 4f,
        layerDepth = 1f
      };
    return new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(129 + 16 /*0x10*/ * this.which.Value, 320, 16 /*0x10*/, 16 /*0x10*/), position, false, 0.0f, Color.White)
    {
      layerDepth = 1f
    };
  }

  public override string checkForSpecialItemHoldUpMeessage()
  {
    switch (this.which.Value)
    {
      case 2:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13090", (object) this.displayName);
      case 3:
        return Game1.content.LoadString("Strings\\Objects:SpecialCharmDescription", (object) this.displayName);
      case 4:
        return Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13092", (object) this.displayName);
      case 5:
        return Game1.content.LoadString("Strings\\Objects:MagnifyingGlassDescription", (object) this.displayName);
      case 6:
        return Game1.content.LoadString("Strings\\Objects:DarkTalismanDescription", (object) this.displayName);
      case 7:
        return Game1.content.LoadString("Strings\\Objects:MagicInkDescription", (object) this.displayName);
      default:
        return this.which.Value == 99 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SpecialItem.cs.13094", (object) this.displayName, (object) Game1.player.maxItems) : base.checkForSpecialItemHoldUpMeessage();
    }
  }

  public override void drawInMenu(
    SpriteBatch spriteBatch,
    Vector2 location,
    float scaleSize,
    float transparency,
    float layerDepth,
    StackDrawType drawStackNumber,
    Color color,
    bool drawShadow)
  {
  }

  public override int maximumStackSize() => 1;

  public override string getDescription() => (string) null;

  public override bool isPlaceable() => false;

  /// <inheritdoc />
  [XmlIgnore]
  public override string DisplayName => this.displayName;

  /// <inheritdoc />
  [XmlIgnore]
  public override string Name
  {
    get
    {
      if (this.netName.Value.Length < 1 || this.netName.Value == "Error Item")
      {
        switch (this.which.Value)
        {
          case 2:
            return "Club Card";
          case 3:
            return Game1.content.LoadString("Strings\\Objects:SpecialCharm");
          case 4:
            return "Skull Key";
          case 5:
            return Game1.content.LoadString("Strings\\Objects:MagnifyingGlass");
          case 6:
            return Game1.content.LoadString("Strings\\Objects:DarkTalisman");
          case 7:
            return Game1.content.LoadString("Strings\\Objects:MagicInk");
        }
      }
      return this.netName.Value;
    }
    set => this.netName.Value = value;
  }

  /// <inheritdoc />
  protected override Item GetOneNew() => throw new NotImplementedException();
}
