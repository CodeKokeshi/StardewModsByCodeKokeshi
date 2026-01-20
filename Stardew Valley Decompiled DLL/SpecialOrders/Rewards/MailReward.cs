// Decompiled with JetBrains decompiler
// Type: StardewValley.SpecialOrders.Rewards.MailReward
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;

#nullable disable
namespace StardewValley.SpecialOrders.Rewards;

public class MailReward : OrderReward
{
  public NetBool noLetter = new NetBool(true);
  public NetStringList grantedMails = new NetStringList();
  public NetBool host = new NetBool(false);

  public override void InitializeNetFields()
  {
    base.InitializeNetFields();
    this.NetFields.AddField((INetSerializable) this.noLetter, "noLetter").AddField((INetSerializable) this.grantedMails, "grantedMails").AddField((INetSerializable) this.host, "host");
  }

  public override void Load(SpecialOrder order, Dictionary<string, string> data)
  {
    this.grantedMails.AddRange((IEnumerable<string>) ArgUtility.SplitBySpace(order.Parse(data["MailReceived"])));
    string data1;
    if (data.TryGetValue("NoLetter", out data1))
      this.noLetter.Value = Convert.ToBoolean(order.Parse(data1));
    if (!data.TryGetValue("Host", out data1))
      return;
    this.host.Value = Convert.ToBoolean(order.Parse(data1));
  }

  public override void Grant()
  {
    foreach (string grantedMail in (NetList<string, NetString>) this.grantedMails)
    {
      if (this.host.Value)
      {
        if (Game1.IsMasterGame)
        {
          if (Game1.newDaySync.hasInstance())
          {
            Game1.addMail(grantedMail, this.noLetter.Value, true);
          }
          else
          {
            string mailName = grantedMail;
            if (mailName == "ClintReward" && Game1.player.mailReceived.Contains("ClintReward"))
            {
              Game1.player.mailReceived.Remove("ClintReward2");
              mailName = "ClintReward2";
            }
            Game1.addMailForTomorrow(mailName, this.noLetter.Value, true);
          }
        }
      }
      else if (Game1.newDaySync.hasInstance())
      {
        Game1.addMail(grantedMail, this.noLetter.Value, true);
      }
      else
      {
        string mailName = grantedMail;
        if (mailName == "ClintReward" && Game1.player.mailReceived.Contains("ClintReward"))
        {
          Game1.player.mailReceived.Remove("ClintReward2");
          mailName = "ClintReward2";
        }
        Game1.addMailForTomorrow(mailName, this.noLetter.Value, true);
      }
    }
  }
}
