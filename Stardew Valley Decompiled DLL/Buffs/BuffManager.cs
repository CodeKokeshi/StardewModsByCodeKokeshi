// Decompiled with JetBrains decompiler
// Type: StardewValley.Buffs.BuffManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Buffs;

/// <summary>Manages buffs for a player.</summary>
public class BuffManager : INetObject<NetFields>
{
  /// <summary>The player whose buffs are managed by the instance.</summary>
  protected Farmer Player;
  /// <summary>The combined effects of all current buffs and equipment bonuses, calculated from <see cref="F:StardewValley.Buffs.BuffManager.AppliedBuffs" />.</summary>
  protected readonly BuffEffects CombinedEffects = new BuffEffects();
  /// <summary>An unsynchronized dictionary of buffs that are currently applied to the player.</summary>
  public readonly IDictionary<string, Buff> AppliedBuffs = (IDictionary<string, Buff>) new Dictionary<string, Buff>();
  /// <summary>A synchronized list of buff IDs currently applied to the player.</summary>
  public readonly NetStringList AppliedBuffIds = new NetStringList();
  /// <summary>Whether the buffs changed and will be recalculated on the next update.</summary>
  public bool Dirty = true;

  public NetFields NetFields { get; } = new NetFields(nameof (BuffManager));

  /// <summary>The combined buff to the player's combat skill level.</summary>
  public int CombatLevel => (int) this.GetValues().CombatLevel.Value;

  /// <summary>The combined buff to the player's farming skill level.</summary>
  public int FarmingLevel => (int) this.GetValues().FarmingLevel.Value;

  /// <summary>The combined buff to the player's fishing skill level.</summary>
  public int FishingLevel => (int) this.GetValues().FishingLevel.Value;

  /// <summary>The combined buff to the player's mining skill level.</summary>
  public int MiningLevel => (int) this.GetValues().MiningLevel.Value;

  /// <summary>The combined buff to the player's luck skill level.</summary>
  public int LuckLevel => (int) this.GetValues().LuckLevel.Value;

  /// <summary>The combined buff to the player's foraging skill level.</summary>
  public int ForagingLevel => (int) this.GetValues().ForagingLevel.Value;

  /// <summary>The combined buff to the player's max stamina.</summary>
  public int MaxStamina => (int) this.GetValues().MaxStamina.Value;

  /// <summary>The combined buff to the player's magnetic radius.</summary>
  public int MagneticRadius => (int) this.GetValues().MagneticRadius.Value;

  /// <summary>The combined buff to the player's walk speed.</summary>
  public float Speed => this.GetValues().Speed.Value;

  /// <summary>The combined buff to the player's defense.</summary>
  public int Defense => (int) this.GetValues().Defense.Value;

  /// <summary>The combined buff to the player's attack power.</summary>
  public int Attack => (int) this.GetValues().Attack.Value;

  /// <summary>The combined buff to the player's resistance to negative effects.</summary>
  public int Immunity => (int) this.GetValues().Immunity.Value;

  /// <summary>The combined multiplier applied to the player's attack power.</summary>
  public float AttackMultiplier => this.GetValues().AttackMultiplier.Value;

  /// <summary>The combined multiplier applied to monster knockback when hit by the player's weapon.</summary>
  public float KnockbackMultiplier => this.GetValues().KnockbackMultiplier.Value;

  /// <summary>The combined multiplier applied to the player's weapon swing speed.</summary>
  public float WeaponSpeedMultiplier => this.GetValues().WeaponSpeedMultiplier.Value;

  /// <summary>The combined multiplier applied to the player's critical hit chance.</summary>
  public float CriticalChanceMultiplier => this.GetValues().CriticalChanceMultiplier.Value;

  /// <summary>The combined multiplier applied to the player's critical hit damage.</summary>
  public float CriticalPowerMultiplier => this.GetValues().CriticalPowerMultiplier.Value;

  /// <summary>The combined multiplier applied to the player's weapon accuracy.</summary>
  public float WeaponPrecisionMultiplier => this.GetValues().WeaponPrecisionMultiplier.Value;

  /// <summary>Construct an instance.</summary>
  public BuffManager()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.AppliedBuffIds, nameof (AppliedBuffIds)).AddField((INetSerializable) this.CombinedEffects.NetFields, "CombinedEffects.NetFields");
  }

  /// <summary>Get the combined buff values, recalculating them if dirty.</summary>
  /// <remarks>Most code should use the properties like <see cref="P:StardewValley.Buffs.BuffManager.Attack" /> instance.</remarks>
  public virtual BuffEffects GetValues()
  {
    if (!this.Dirty)
      return this.CombinedEffects;
    Farmer player = this.Player;
    this.CombinedEffects.Clear();
    player.stopGlowing();
    foreach (Buff buff in (IEnumerable<Buff>) this.AppliedBuffs.Values)
    {
      this.CombinedEffects.Add(buff.effects);
      if (buff.glow != Color.White && buff.glow.A > (byte) 0)
        player.startGlowing(buff.glow, false, 0.05f);
    }
    this.AppliedBuffIds.Clear();
    foreach (string key in (IEnumerable<string>) this.AppliedBuffs.Keys)
      this.AppliedBuffIds.Add(key);
    foreach (Item equippedItem in player.GetEquippedItems())
      equippedItem.AddEquipmentEffects(this.CombinedEffects);
    if (this.IsLocallyControlled())
      Game1.buffsDisplay.dirty = true;
    this.Dirty = false;
    player.stamina = Math.Min(player.stamina, (float) player.MaxStamina);
    return this.CombinedEffects;
  }

  /// <summary>Set the player managed by the instance.</summary>
  /// <param name="player">The player managed by the instance.</param>
  public void SetOwner(Farmer player) => this.Player = player;

  /// <summary>Get whether the player has a buff applied.</summary>
  /// <param name="id">The buff ID.</param>
  public bool IsApplied(string id) => this.AppliedBuffIds.Contains(id);

  /// <summary>Get whether the player has a buff with an ID containing the given string.</summary>
  /// <param name="idSubstring">The substring to match in the buff ID.</param>
  public bool HasBuffWithNameContaining(string idSubstring)
  {
    foreach (string appliedBuffId in (NetList<string, NetString>) this.AppliedBuffIds)
    {
      if (appliedBuffId.Contains(idSubstring))
        return true;
    }
    return false;
  }

  /// <summary>Get whether this instance is managed by the local player (e.g. it's their own buffs).</summary>
  public virtual bool IsLocallyControlled()
  {
    return this.Player.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID;
  }

  /// <summary>Add a buff to the player, or refresh it if it's already applied.</summary>
  /// <param name="buff">The buff to add.</param>
  public void Apply(Buff buff)
  {
    if (buff == null)
      Game1.log.Warn("Ignored invalid null buff.");
    else if (string.IsNullOrWhiteSpace(buff.id))
      Game1.log.Warn("Ignored invalid buff with no ID.");
    else if (buff.millisecondsDuration <= 0 && buff.millisecondsDuration != -2)
    {
      Game1.log.Warn($"Ignored invalid buff '{buff.id}' with {(buff.millisecondsDuration < 0 ? "negative" : "no")} duration.");
    }
    else
    {
      if (!this.IsLocallyControlled())
        return;
      this.Remove(buff.id);
      this.AppliedBuffs[buff.id] = buff;
      this.AppliedBuffIds.Add(buff.id);
      string[] actionsOnApply = buff.actionsOnApply;
      if ((actionsOnApply != null ? (actionsOnApply.Length != 0 ? 1 : 0) : 0) != 0)
      {
        foreach (string action in buff.actionsOnApply)
        {
          string error;
          Exception exception;
          if (TriggerActionManager.TryRunAction(action, out error, out exception))
            Game1.log.Verbose($"Applied action [{action}] from buff '{buff.id}'.");
          else
            Game1.log.Error($"Error applying Applied action [{action}] from buff '{buff.id}': {error}.", exception);
        }
      }
      Game1.buffsDisplay.updatedIDs.Add(buff.id);
      this.Dirty = true;
      buff.OnAdded();
    }
  }

  /// <summary>Remove a buff from the player.</summary>
  /// <param name="id">The buff ID.</param>
  public void Remove(string id)
  {
    if (!this.IsLocallyControlled())
      return;
    Buff buff;
    if (this.AppliedBuffs.TryGetValue(id, out buff))
      buff.OnRemoved();
    if (!(this.AppliedBuffs.Remove(id) | this.AppliedBuffIds.Remove(id) | Game1.buffsDisplay.updatedIDs.Remove(id)))
      return;
    this.Dirty = true;
  }

  /// <summary>Remove all buffs from the player.</summary>
  public void Clear()
  {
    if (!this.IsLocallyControlled())
      return;
    for (int index = this.AppliedBuffIds.Count - 1; index >= 0; --index)
      this.Remove(this.AppliedBuffIds[index]);
  }

  /// <summary>Update the buff timers and remove expired buffs.</summary>
  /// <param name="time">The elapsed game time.</param>
  public void Update(GameTime time)
  {
    if (!this.IsLocallyControlled())
      return;
    for (int index = this.AppliedBuffIds.Count - 1; index >= 0; --index)
    {
      string appliedBuffId = this.AppliedBuffIds[index];
      Buff buff;
      if (!this.AppliedBuffs.TryGetValue(appliedBuffId, out buff) || buff.update(time))
        this.Remove(appliedBuffId);
    }
    if (!this.Dirty)
      return;
    this.GetValues();
  }
}
