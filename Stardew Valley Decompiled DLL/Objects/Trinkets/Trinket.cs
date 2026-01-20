// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Trinkets.Trinket
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.GameData;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Objects.Trinkets;

/// <summary>An item which can be equipped in the player's trinket slot which has special effects while equipped.</summary>
public class Trinket : StardewValley.Object
{
  protected string _description;
  protected TrinketData _data;
  protected TrinketEffect _trinketEffect;
  protected string _trinketEffectClassName;
  /// <summary>The parsed form of <see cref="F:StardewValley.Objects.Trinkets.Trinket.displayNameOverrideTemplate" /> used to build the display name for <see cref="M:StardewValley.Objects.Trinkets.Trinket.loadDisplayName" />.</summary>
  protected string displayNameOverride;
  /// <summary>The net-synced <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenized string</see> used to build the display name for <see cref="M:StardewValley.Objects.Trinkets.Trinket.loadDisplayName" />.</summary>
  public readonly NetString displayNameOverrideTemplate = new NetString();
  /// <summary>The net-synced <see cref="T:StardewValley.TokenizableStrings.TokenParser">tokenized strings</see> used to fill placeholders in <see cref="M:StardewValley.Objects.Trinkets.Trinket.getDescription" />.</summary>
  public readonly NetStringList descriptionSubstitutionTemplates = new NetStringList();
  public readonly NetStringDictionary<string, NetString> trinketMetadata = new NetStringDictionary<string, NetString>();
  [XmlElement("generationSeed")]
  public readonly NetInt generationSeed = new NetInt();

  /// <inheritdoc />
  public override string TypeDefinitionId { get; } = "(TR)";

  public Trinket()
  {
  }

  public Trinket(string itemId, int generationSeed)
    : this()
  {
    this.ItemId = itemId;
    this.name = itemId;
    this.generationSeed.Value = generationSeed;
    this.ParentSheetIndex = ItemRegistry.GetDataOrErrorItem(itemId).SpriteIndex;
    Dictionary<string, string> modData = this.GetTrinketData()?.ModData;
    // ISSUE: explicit non-virtual call
    if (modData != null && __nonvirtual (modData.Count) > 0)
    {
      foreach (KeyValuePair<string, string> keyValuePair in modData)
        this.modData.Add(keyValuePair.Key, keyValuePair.Value);
    }
    this.GetEffect()?.GenerateRandomStats(this);
  }

  public static bool CanSpawnTrinket(Farmer f) => f.stats.Get("trinketSlots") > 0U;

  public static void SpawnTrinket(GameLocation location, Vector2 spawnPoint)
  {
    Trinket randomTrinket = Trinket.GetRandomTrinket();
    if (randomTrinket == null)
      return;
    Game1.createItemDebris((Item) randomTrinket, spawnPoint, Game1.random.Next(4), location);
  }

  /// <summary>Re-roll the trinket stats if applicable.</summary>
  /// <param name="newSeed">The new trinket seed to set.</param>
  /// <remarks>Returns whether the trinket stats were re-rolled (regardless of whether they changed).</remarks>
  public bool RerollStats(int newSeed)
  {
    this.generationSeed.Value = newSeed;
    TrinketEffect effect = this.GetEffect();
    return effect != null && effect.GenerateRandomStats(this);
  }

  /// <inheritdoc />
  public override bool canBeShipped() => false;

  /// <inheritdoc />
  public override int sellToStorePrice(long specificPlayerID = -1) => 1000;

  public static void TrySpawnTrinket(
    GameLocation location,
    Monster monster,
    Vector2 spawnPosition,
    double chanceModifier = 1.0)
  {
    if (!Trinket.CanSpawnTrinket(Game1.player))
      return;
    double val2 = 0.004;
    if (monster != null)
    {
      val2 += (double) monster.MaxHealth * 1E-05;
      if (monster.isGlider.Value && monster.MaxHealth >= 150)
        val2 += 0.002;
      if (monster is Leaper)
        val2 -= 0.005;
    }
    double num = (Math.Min(0.025, val2) + Game1.player.DailyLuck / 25.0 + (double) Game1.player.LuckLevel * 0.0013299999991431832) * chanceModifier;
    if (Game1.random.NextDouble() >= num)
      return;
    Trinket.SpawnTrinket(location, spawnPosition);
  }

  public static Trinket GetRandomTrinket()
  {
    Dictionary<string, TrinketData> dictionary = DataLoader.Trinkets(Game1.content);
    Trinket randomTrinket = (Trinket) null;
    while (randomTrinket == null)
    {
      int num1 = Game1.random.Next(dictionary.Count);
      int num2 = 0;
      foreach (KeyValuePair<string, TrinketData> keyValuePair in dictionary)
      {
        if (num1 == num2 && keyValuePair.Value.DropsNaturally)
        {
          randomTrinket = ItemRegistry.Create<Trinket>("(TR)" + keyValuePair.Key);
          break;
        }
        ++num2;
      }
    }
    return randomTrinket;
  }

  /// <inheritdoc />
  public override bool canBeGivenAsGift() => true;

  public override void reloadSprite()
  {
    base.reloadSprite();
    this.GetEffect()?.GenerateRandomStats(this);
  }

  /// <inheritdoc />
  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.trinketMetadata, "trinketMetadata").AddField((INetSerializable) this.generationSeed, "generationSeed").AddField((INetSerializable) this.displayNameOverrideTemplate, "displayNameOverrideTemplate").AddField((INetSerializable) this.descriptionSubstitutionTemplates, "descriptionSubstitutionTemplates");
    this.displayNameOverrideTemplate.fieldChangeVisibleEvent += (FieldChange<NetString, string>) ((field, oldValue, newValue) => this.displayNameOverride = TokenParser.ParseText(newValue));
    this.descriptionSubstitutionTemplates.OnElementChanged += (NetList<string, NetString>.ElementChangedEvent) ((_param1, _param2, _param3, _param4) => this._description = (string) null);
    this.descriptionSubstitutionTemplates.OnArrayReplaced += (NetList<string, NetString>.ArrayReplacedEvent) ((_param1, _param2, _param3) => this._description = (string) null);
  }

  public TrinketData GetTrinketData()
  {
    if (this._data == null)
      this._data = DataLoader.Trinkets(Game1.content).GetValueOrDefault<string, TrinketData>(this.ItemId);
    return this._data;
  }

  public virtual TrinketEffect GetEffect()
  {
    if (this._trinketEffect == null)
    {
      TrinketData trinketData = this.GetTrinketData();
      if (trinketData != null && this._trinketEffectClassName != trinketData.TrinketEffectClass)
      {
        this._trinketEffectClassName = trinketData.TrinketEffectClass;
        if (trinketData.TrinketEffectClass != null)
        {
          System.Type type = System.Type.GetType(trinketData.TrinketEffectClass);
          if (type != (System.Type) null)
            this._trinketEffect = (TrinketEffect) Activator.CreateInstance(type, (object) this);
          else
            Game1.log.Warn($"Failed loading effects for trinket {this.QualifiedItemId}: invalid class type '{trinketData.TrinketEffectClass}'.");
        }
      }
    }
    return this._trinketEffect;
  }

  /// <inheritdoc />
  protected override string loadDisplayName()
  {
    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(this.ItemId);
    return this.displayNameOverride ?? dataOrErrorItem.DisplayName;
  }

  public override int maximumStackSize() => 1;

  public override string getDescription()
  {
    if (this._description == null)
    {
      string str = TokenParser.ParseText(ItemRegistry.GetDataOrErrorItem(this.ItemId).Description);
      if (this.descriptionSubstitutionTemplates.Count > 0)
      {
        object[] objArray = new object[this.descriptionSubstitutionTemplates.Count];
        for (int index = 0; index < this.descriptionSubstitutionTemplates.Count; ++index)
          objArray[index] = (object) TokenParser.ParseText(this.descriptionSubstitutionTemplates[index]);
        str = string.Format(str, objArray);
      }
      this._description = Game1.parseText(str, Game1.smallFont, this.getDescriptionWidth());
    }
    return this._description;
  }

  /// <inheritdoc />
  public override string getCategoryName()
  {
    return Game1.content.LoadString("Strings\\1_6_Strings:Trinket");
  }

  /// <inheritdoc />
  public override Color getCategoryColor() => new Color(96 /*0x60*/, 81, (int) byte.MaxValue);

  public override bool isPlaceable() => false;

  public override bool performUseAction(GameLocation location)
  {
    this.GetEffect().OnUse(Game1.player);
    return false;
  }

  public override bool performToolAction(Tool t) => false;

  /// <inheritdoc />
  protected override Item GetOneNew() => (Item) new Trinket(this.ItemId, this.generationSeed.Value);

  /// <inheritdoc />
  protected override void GetOneCopyFrom(Item source)
  {
    base.GetOneCopyFrom(source);
    if (!(source is Trinket trinket))
      return;
    this.displayNameOverrideTemplate.Value = trinket.displayNameOverrideTemplate.Value;
    this.descriptionSubstitutionTemplates.Set((IList<string>) trinket.descriptionSubstitutionTemplates);
    this.trinketMetadata.Set((IEnumerable<KeyValuePair<string, string>>) trinket.trinketMetadata.Pairs);
    this.generationSeed.Value = trinket.generationSeed.Value;
  }

  public override bool IsHeldOverHead() => false;

  /// <summary>Handle the trinket being equipped.</summary>
  /// <param name="farmer">The player equipping the trinket.</param>
  public virtual void Apply(Farmer farmer) => this.GetEffect()?.Apply(farmer);

  /// <summary>Handle the trinket being unequipped.</summary>
  /// <param name="farmer">The player unequipping the trinket.</param>
  public virtual void Unapply(Farmer farmer) => this.GetEffect()?.Unapply(farmer);

  /// <summary>Update the trinket.</summary>
  /// <param name="farmer">The player with the trinket equipped.</param>
  /// <param name="time">The elapsed game time.</param>
  /// <param name="location">The player's current location.</param>
  public virtual void Update(Farmer farmer, GameTime time, GameLocation location)
  {
    this.GetEffect()?.Update(farmer, time, location);
  }

  /// <summary>Handle the player having taken a step.</summary>
  /// <param name="farmer">The player with the trinket equipped.</param>
  public virtual void OnFootstep(Farmer farmer) => this.GetEffect()?.OnFootstep(farmer);

  /// <summary>Handle the player having received damage.</summary>
  /// <param name="farmer">The player with the trinket equipped.</param>
  /// <param name="damageAmount">The amount of damage that was taken.</param>
  public virtual void OnReceiveDamage(Farmer farmer, int damageAmount)
  {
    this.GetEffect()?.OnReceiveDamage(farmer, damageAmount);
  }

  /// <summary>Handle the player dealing damage to a monster.</summary>
  /// <param name="farmer">The player with the trinket equipped.</param>
  /// <param name="monster">The monster which was damaged.</param>
  /// <param name="damageAmount">The amount of damage that was dealt.</param>
  /// <param name="isBomb">Whether the damage is from a bomb.</param>
  /// <param name="isCriticalHit">Whether the attack which caused the damage was a critical hit.</param>
  public virtual void OnDamageMonster(
    Farmer farmer,
    Monster monster,
    int damageAmount,
    bool isBomb,
    bool isCriticalHit)
  {
    this.GetEffect()?.OnDamageMonster(farmer, monster, damageAmount, isBomb, isCriticalHit);
  }
}
