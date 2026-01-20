// Decompiled with JetBrains decompiler
// Type: StardewValley.Util.SynchronizedShopStock
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using StardewValley.GameData.Shops;
using StardewValley.Network;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.Util;

public class SynchronizedShopStock : INetObject<NetFields>
{
  private readonly NetStringDictionary<int, NetInt> stockDictionary = new NetStringDictionary<int, NetInt>();
  protected static HashSet<string> _usedKeys = new HashSet<string>();
  protected static List<ISalable> _stockSalables = new List<ISalable>();

  public NetFields NetFields { get; } = new NetFields(nameof (SynchronizedShopStock));

  public SynchronizedShopStock() => this.initNetFields();

  private void initNetFields()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.stockDictionary, "stockDictionary");
  }

  public virtual void Clear() => this.stockDictionary.Clear();

  public void OnItemPurchased(
    string shop_id,
    ISalable item,
    Dictionary<ISalable, ItemStockInformation> stock,
    int amount)
  {
    NetStringDictionary<int, NetInt> stockDictionary = this.stockDictionary;
    ItemStockInformation stockInformation;
    if (!stock.TryGetValue(item, out stockInformation) || stockInformation.Stock == int.MaxValue)
      return;
    string qualifiedSyncedKey = this.GetQualifiedSyncedKey(shop_id, stockInformation);
    stockInformation.Stock -= amount;
    stockDictionary[qualifiedSyncedKey] = stockInformation.Stock;
  }

  public string GetQualifiedSyncedKey(string shop_id, ItemStockInformation item)
  {
    if (item.LimitedStockMode == LimitedStockMode.Global)
      return $"{shop_id}/Global/{item.SyncedKey}";
    return $"{shop_id}/{Game1.player.UniqueMultiplayerID}/{item.SyncedKey}";
  }

  public void UpdateLocalStockWithSyncedQuanitities(
    string shop_id,
    Dictionary<ISalable, ItemStockInformation> local_stock)
  {
    SynchronizedShopStock._usedKeys.Clear();
    SynchronizedShopStock._stockSalables.Clear();
    List<ISalable> salableList = new List<ISalable>();
    SynchronizedShopStock._stockSalables.AddRange((IEnumerable<ISalable>) local_stock.Keys);
    foreach (ISalable stockSalable in SynchronizedShopStock._stockSalables)
    {
      ItemStockInformation stockInformation = local_stock[stockSalable];
      if (stockInformation.Stock != int.MaxValue && stockInformation.LimitedStockMode != LimitedStockMode.None)
      {
        if (stockInformation.SyncedKey == null)
        {
          string name = stockSalable.Name;
          string str = name;
          int num = 1;
          while (SynchronizedShopStock._usedKeys.Contains(str))
          {
            str = name + num.ToString();
            ++num;
          }
          SynchronizedShopStock._usedKeys.Add(str);
          stockInformation.SyncedKey = str;
          local_stock[stockSalable] = stockInformation;
        }
        int num1;
        if (this.stockDictionary.TryGetValue(this.GetQualifiedSyncedKey(shop_id, stockInformation), out num1))
        {
          stockInformation.Stock = num1;
          local_stock[stockSalable] = stockInformation;
          if (num1 <= 0)
            salableList.Add(stockSalable);
        }
      }
    }
    SynchronizedShopStock._usedKeys.Clear();
    SynchronizedShopStock._stockSalables.Clear();
    foreach (Item key in salableList)
      local_stock.Remove((ISalable) key);
  }
}
