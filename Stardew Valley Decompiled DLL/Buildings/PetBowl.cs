// Decompiled with JetBrains decompiler
// Type: StardewValley.Buildings.PetBowl
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.GameData.Buildings;
using StardewValley.Tools;
using System;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley.Buildings;

public class PetBowl(Vector2 tileLocation) : Building("Pet Bowl", tileLocation)
{
  /// <summary>Whether the pet bowl is full.</summary>
  [XmlElement("watered")]
  public readonly NetBool watered = new NetBool();
  private int nameTimer;
  private string nameTimerMessage;
  /// <summary>The pet to which this bowl belongs, if any.</summary>
  /// <remarks>When a pet is assigned, this matches <see cref="F:StardewValley.Characters.Pet.petId" />.</remarks>
  [XmlElement("petGuid")]
  public readonly NetGuid petId = new NetGuid();

  public PetBowl()
    : this(Vector2.Zero)
  {
  }

  /// <summary>Assign a pet to this pet bowl.</summary>
  /// <param name="pet">The pet to assign.</param>
  public virtual void AssignPet(Pet pet)
  {
    this.petId.Value = pet.petId.Value;
    pet.homeLocationName.Value = this.parentLocationName.Value;
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this.watered, "watered").AddField((INetSerializable) this.petId, "petId");
  }

  public virtual Point GetPetSpot() => new Point(this.tileX.Value, this.tileY.Value + 1);

  public override bool doAction(Vector2 tileLocation, Farmer who)
  {
    if (!this.isTilePassable(tileLocation))
    {
      Guid guid = this.petId.Value;
      Pet pet = Utility.findPet(this.petId.Value);
      if (pet != null)
      {
        this.nameTimer = 3500;
        this.nameTimerMessage = Game1.content.LoadString("Strings\\1_6_Strings:PetBowlName", (object) pet.displayName);
      }
    }
    return base.doAction(tileLocation, who);
  }

  public override void Update(GameTime time)
  {
    if (this.nameTimer > 0)
      this.nameTimer -= (int) time.ElapsedGameTime.TotalMilliseconds;
    base.Update(time);
  }

  public override void performToolAction(Tool t, int tileX, int tileY)
  {
    if (t is WateringCan)
    {
      string property_value = (string) null;
      if (this.doesTileHaveProperty(tileX, tileY, nameof (PetBowl), "Buildings", ref property_value))
        this.watered.Value = true;
    }
    base.performToolAction(t, tileX, tileY);
  }

  /// <summary>Get whether any pet has been assigned to this pet bowl.</summary>
  public bool HasPet() => this.petId.Value != Guid.Empty;

  public override void draw(SpriteBatch b)
  {
    base.draw(b);
    if (this.isMoving || this.isUnderConstruction())
      return;
    if (this.watered.Value)
    {
      BuildingData data = this.GetData();
      float num = (float) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/);
      if (data != null)
        num -= data.SortTileOffset * 64f;
      float layerDepth = (num + 1.5f) / 10000f;
      Vector2 vector2_1 = new Vector2((float) (this.tileX.Value * 64 /*0x40*/), (float) (this.tileY.Value * 64 /*0x40*/ + this.tilesHigh.Value * 64 /*0x40*/));
      Vector2 vector2_2 = Vector2.Zero;
      if (data != null)
        vector2_2 = data.DrawOffset * 4f;
      Rectangle sourceRect = this.getSourceRect();
      sourceRect.X += sourceRect.Width;
      Vector2 origin = new Vector2(0.0f, (float) sourceRect.Height);
      b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, vector2_1 + vector2_2), new Rectangle?(sourceRect), this.color * this.alpha, 0.0f, origin, 4f, SpriteEffects.None, layerDepth);
    }
    if (this.nameTimer <= 0)
      return;
    BuildingData data1 = this.GetData();
    float num1 = (float) ((this.tileY.Value + this.tilesHigh.Value) * 64 /*0x40*/);
    if (data1 != null)
      num1 -= data1.SortTileOffset * 64f;
    float num2 = (num1 + 1.5f) / 10000f;
    SpriteText.drawSmallTextBubble(b, this.nameTimerMessage, Game1.GlobalToLocal(new Vector2((float) (((double) this.tileX.Value + 1.5) * 64.0), (float) (this.tileY.Value * 64 /*0x40*/ - 32 /*0x20*/))), layerDepth: num2 + 1E-06f);
  }
}
