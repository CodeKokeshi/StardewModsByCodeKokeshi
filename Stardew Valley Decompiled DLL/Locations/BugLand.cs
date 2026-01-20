// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.BugLand
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System;
using System.Xml.Serialization;
using xTile.Layers;

#nullable disable
namespace StardewValley.Locations;

public class BugLand : GameLocation
{
  [XmlElement("hasSpawnedBugsToday")]
  public bool hasSpawnedBugsToday;

  public BugLand()
  {
  }

  public BugLand(string map, string name)
    : base(map, name)
  {
  }

  public override void TransferDataFromSavedLocation(GameLocation l)
  {
    if (l is BugLand bugLand)
      this.hasSpawnedBugsToday = bugLand.hasSpawnedBugsToday;
    base.TransferDataFromSavedLocation(l);
  }

  public override void hostSetup()
  {
    base.hostSetup();
    if (!Game1.IsMasterGame || this.hasSpawnedBugsToday)
      return;
    this.InitializeBugLand();
  }

  public override void DayUpdate(int dayOfMonth)
  {
    base.DayUpdate(dayOfMonth);
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => npc is Grub || npc is Fly));
    this.hasSpawnedBugsToday = false;
  }

  public virtual void InitializeBugLand()
  {
    if (this.hasSpawnedBugsToday)
      return;
    this.hasSpawnedBugsToday = true;
    Layer layer = this.map.RequireLayer("Paths");
    for (int x = 0; x < this.map.Layers[0].LayerWidth; ++x)
    {
      for (int y = 0; y < this.map.Layers[0].LayerHeight; ++y)
      {
        if (Game1.random.NextDouble() < 0.33)
        {
          int tileIndexAt = layer.GetTileIndexAt(x, y);
          if (tileIndexAt != -1)
          {
            Vector2 vector2 = new Vector2((float) x, (float) y);
            switch (tileIndexAt - 13)
            {
              case 0:
              case 1:
              case 2:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<StardewValley.Object>(GameLocation.getWeedForSeason(Game1.random, Season.Spring)));
                  continue;
                }
                continue;
              case 3:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<StardewValley.Object>(Game1.random.Choose<string>("(O)343", "(O)450")));
                  continue;
                }
                continue;
              case 4:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<StardewValley.Object>(Game1.random.Choose<string>("(O)343", "(O)450")));
                  continue;
                }
                continue;
              case 5:
                if (!this.objects.ContainsKey(vector2))
                {
                  this.objects.Add(vector2, ItemRegistry.Create<StardewValley.Object>(Game1.random.Choose<string>("(O)294", "(O)295")));
                  continue;
                }
                continue;
              default:
                if (tileIndexAt == 28 && this.CanSpawnCharacterHere(vector2) && this.characters.Count < 50)
                {
                  this.characters.Add((NPC) new Grub(new Vector2(vector2.X * 64f, vector2.Y * 64f), true));
                  continue;
                }
                continue;
            }
          }
        }
      }
    }
  }
}
