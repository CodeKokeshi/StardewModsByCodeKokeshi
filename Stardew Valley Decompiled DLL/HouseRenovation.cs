// Decompiled with JetBrains decompiler
// Type: StardewValley.HouseRenovation
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Extensions;
using StardewValley.GameData.HomeRenovations;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley;

public class HouseRenovation : ISalable, IHaveItemTypeId
{
  protected string _displayName;
  protected string _name;
  protected string _description;
  public HouseRenovation.AnimationType animationType;
  public List<List<Rectangle>> renovationBounds = new List<List<Rectangle>>();
  public string placementText = "";
  public GameLocation location;
  public bool requireClearance = true;
  public Action<HouseRenovation, int> onRenovation;
  public Func<HouseRenovation, int, bool> validate;
  /// <inheritdoc cref="F:StardewValley.GameData.HomeRenovations.HomeRenovation.Price" />
  public int Price;
  /// <inheritdoc cref="F:StardewValley.GameData.HomeRenovations.HomeRenovation.RoomId" />
  public string RoomId;

  public bool ShouldDrawIcon() => false;

  /// <inheritdoc />
  public string TypeDefinitionId => "(Salable)";

  /// <inheritdoc />
  public string QualifiedItemId => this.TypeDefinitionId + nameof (HouseRenovation);

  /// <inheritdoc />
  public string DisplayName => this._displayName;

  public void drawInMenu(
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

  /// <inheritdoc />
  public string Name => this._name;

  public bool IsRecipe
  {
    get => false;
    set
    {
    }
  }

  public string getDescription() => this._description;

  public int maximumStackSize() => 1;

  public int addToStack(Item stack) => 0;

  public int Stack
  {
    get => 1;
    set
    {
    }
  }

  public int Quality
  {
    get => 0;
    set
    {
    }
  }

  /// <inheritdoc />
  public int sellToStorePrice(long specificPlayerID = -1) => -1;

  /// <inheritdoc />
  public int salePrice(bool ignoreProfitMargins = false) => this.Price <= 0 ? 0 : this.Price;

  /// <inheritdoc />
  public bool appliesProfitMargins() => false;

  /// <inheritdoc />
  public bool actionWhenPurchased(string shopId) => false;

  public bool canStackWith(ISalable other) => false;

  public bool CanBuyItem(Farmer farmer) => true;

  public bool IsInfiniteStock() => true;

  public ISalable GetSalableInstance() => (ISalable) this;

  /// <inheritdoc />
  public void FixStackSize()
  {
  }

  /// <inheritdoc />
  public void FixQuality()
  {
  }

  /// <inheritdoc />
  public string GetItemTypeId() => this.TypeDefinitionId;

  public static void ShowRenovationMenu()
  {
    Game1.activeClickableMenu = (IClickableMenu) new ShopMenu("HouseRenovations", HouseRenovation.GetAvailableRenovations(), on_purchase: new ShopMenu.OnPurchaseDelegate(HouseRenovation.OnPurchaseRenovation))
    {
      purchaseSound = (string) null
    };
  }

  public static List<ISalable> GetAvailableRenovations()
  {
    FarmHouse farmhouse = Game1.RequireLocation<FarmHouse>(Game1.player.homeLocation.Value);
    List<ISalable> availableRenovations = new List<ISalable>();
    Dictionary<string, HomeRenovation> dictionary = DataLoader.HomeRenovations(Game1.content);
    foreach (string key in dictionary.Keys)
    {
      HomeRenovation homeRenovation = dictionary[key];
      bool flag1 = true;
      foreach (RenovationValue requirement in homeRenovation.Requirements)
      {
        if (requirement.Type == "Value")
        {
          string s = requirement.Value;
          bool flag2 = true;
          if (s.Length > 0 && s[0] == '!')
          {
            s = s.Substring(1);
            flag2 = false;
          }
          int num = int.Parse(s);
          try
          {
            NetInt netInt = (NetInt) farmhouse.GetType().GetField(requirement.Key).GetValue((object) farmhouse);
            if (netInt == null)
            {
              flag1 = false;
              break;
            }
            if (netInt.Value == num != flag2)
            {
              flag1 = false;
              break;
            }
          }
          catch (Exception ex)
          {
            flag1 = false;
            break;
          }
        }
        else if (requirement.Type == "Mail" && Game1.player.hasOrWillReceiveMail(requirement.Key) != (requirement.Value == "1"))
        {
          flag1 = false;
          break;
        }
      }
      if (flag1)
      {
        HouseRenovation houseRenovation = new HouseRenovation()
        {
          location = (GameLocation) farmhouse,
          _name = key
        };
        string[] strArray = Game1.content.LoadString(homeRenovation.TextStrings).Split('/');
        try
        {
          houseRenovation._displayName = strArray[0];
          houseRenovation._description = strArray[1];
          houseRenovation.placementText = strArray[2];
        }
        catch (Exception ex)
        {
          houseRenovation._displayName = "?";
          houseRenovation._description = "?";
          houseRenovation.placementText = "?";
        }
        if (homeRenovation.CheckForObstructions)
          houseRenovation.validate += new Func<HouseRenovation, int, bool>(HouseRenovation.EnsureNoObstructions);
        houseRenovation.animationType = !(homeRenovation.AnimationType == "destroy") ? HouseRenovation.AnimationType.Build : HouseRenovation.AnimationType.Destroy;
        houseRenovation.Price = homeRenovation.Price;
        houseRenovation.RoomId = !string.IsNullOrEmpty(homeRenovation.RoomId) ? homeRenovation.RoomId : key;
        if (!string.IsNullOrEmpty(homeRenovation.SpecialRect))
        {
          if (homeRenovation.SpecialRect == "crib")
          {
            Rectangle? cribBounds = farmhouse.GetCribBounds();
            if (farmhouse.CanModifyCrib() && cribBounds.HasValue)
              houseRenovation.AddRenovationBound(cribBounds.Value);
            else
              continue;
          }
        }
        else
        {
          foreach (RectGroup rectGroup in homeRenovation.RectGroups)
          {
            List<Rectangle> bounds = new List<Rectangle>();
            foreach (Rect rect in rectGroup.Rects)
              bounds.Add(new Rectangle()
              {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height
              });
            houseRenovation.AddRenovationBound(bounds);
          }
        }
        foreach (RenovationValue renovateAction in homeRenovation.RenovateActions)
        {
          RenovationValue action_data = renovateAction;
          if (action_data.Type == "Value")
          {
            try
            {
              NetInt field = (NetInt) farmhouse.GetType().GetField(action_data.Key).GetValue((object) farmhouse);
              if ((NetFieldBase<int, NetInt>) field == (NetInt) null)
              {
                flag1 = false;
                break;
              }
              houseRenovation.onRenovation += new Action<HouseRenovation, int>(ActionOnRenovation);

              void ActionOnRenovation(HouseRenovation selectedRenovation, int index)
              {
                if (action_data.Value == "selected")
                  field.Value = index;
                else
                  field.Value = int.Parse(action_data.Value);
              }
            }
            catch (Exception ex)
            {
              flag1 = false;
              break;
            }
          }
          else if (action_data.Type == "Mail")
            houseRenovation.onRenovation += new Action<HouseRenovation, int>(MailOnRenovation);

          void MailOnRenovation(HouseRenovation selectedRenovation, int index)
          {
            if (action_data.Value == "0")
              Game1.player.mailReceived.Remove(action_data.Key);
            else
              Game1.player.mailReceived.Add(action_data.Key);
          }
        }
        if (flag1)
        {
          houseRenovation.onRenovation += (Action<HouseRenovation, int>) ((a, b) => farmhouse.UpdateForRenovation());
          availableRenovations.Add((ISalable) houseRenovation);
        }
      }
    }
    return availableRenovations;
  }

  public static bool EnsureNoObstructions(HouseRenovation renovation, int selected_index)
  {
    if (renovation.location == null)
      return false;
    foreach (Rectangle rect in renovation.renovationBounds[selected_index])
    {
      foreach (Vector2 vector in rect.GetVectors())
      {
        if (renovation.location.isTileOccupiedByFarmer(vector) != null)
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:RenovationBlocked"));
          return false;
        }
        if (renovation.location.IsTileOccupiedBy(vector))
        {
          Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:RenovationBlocked"));
          return false;
        }
      }
      Rectangle rectangle = new Rectangle(rect.X * 64 /*0x40*/, rect.Y * 64 /*0x40*/, rect.Width * 64 /*0x40*/, rect.Height * 64 /*0x40*/);
      if (renovation.location is DecoratableLocation location)
      {
        foreach (Object @object in location.furniture)
        {
          if (@object.GetBoundingBox().Intersects(rectangle))
          {
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:RenovationBlocked"));
            return false;
          }
        }
      }
    }
    return true;
  }

  public static void BuildCrib(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    location.cribStyle.Value = 1;
  }

  public static void RemoveCrib(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    location.cribStyle.Value = 0;
  }

  public static void OpenBedroom(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    Game1.player.mailReceived.Add("renovation_bedroom_open");
    location.UpdateForRenovation();
  }

  public static void CloseBedroom(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    Game1.player.mailReceived.Remove("renovation_bedroom_open");
    location.UpdateForRenovation();
  }

  public static void OpenSouthernRoom(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    Game1.player.mailReceived.Add("renovation_southern_open");
    location.UpdateForRenovation();
  }

  public static void CloseSouthernRoom(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    Game1.player.mailReceived.Remove("renovation_southern_open");
    location.UpdateForRenovation();
  }

  public static void OpenCornernRoom(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    Game1.player.mailReceived.Add("renovation_corner_open");
    location.UpdateForRenovation();
  }

  public static void CloseCornerRoom(HouseRenovation renovation, int selected_index)
  {
    if (!(renovation.location is FarmHouse location))
      return;
    Game1.player.mailReceived.Remove("renovation_corner_open");
    location.UpdateForRenovation();
  }

  /// <summary>Handle a renovation being purchased.</summary>
  /// <inheritdoc cref="T:StardewValley.Menus.ShopMenu.OnPurchaseDelegate" />
  public static bool OnPurchaseRenovation(
    ISalable salable,
    Farmer who,
    int countTaken,
    ItemStockInformation stock)
  {
    if (!(salable is HouseRenovation renovation))
      return false;
    who._money += salable.salePrice();
    Game1.activeClickableMenu = (IClickableMenu) new RenovateMenu(renovation);
    return true;
  }

  public virtual void AddRenovationBound(Rectangle bound)
  {
    this.renovationBounds.Add(new List<Rectangle>()
    {
      bound
    });
  }

  public virtual void AddRenovationBound(List<Rectangle> bounds)
  {
    this.renovationBounds.Add(bounds);
  }

  public enum AnimationType
  {
    Build,
    Destroy,
  }
}
