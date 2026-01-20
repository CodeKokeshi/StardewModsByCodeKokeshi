// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.OutgoingMessage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.ObjectModel;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public struct OutgoingMessage(byte messageType, long farmerID, params object[] data)
{
  private byte messageType = messageType;
  private long farmerID = farmerID;
  private object[] data = data;

  public byte MessageType => this.messageType;

  public long FarmerID => this.farmerID;

  public Farmer SourceFarmer => Game1.GetPlayer(this.farmerID) ?? Game1.MasterPlayer;

  public ReadOnlyCollection<object> Data => Array.AsReadOnly<object>(this.data);

  public OutgoingMessage(byte messageType, Farmer sourceFarmer, params object[] data)
    : this(messageType, sourceFarmer.UniqueMultiplayerID, data)
  {
  }

  public OutgoingMessage(IncomingMessage message)
    : this(message.MessageType, message.FarmerID, new object[1]
    {
      (object) message.Data
    })
  {
  }

  public void Write(BinaryWriter writer)
  {
    writer.Write(this.messageType);
    writer.Write(this.farmerID);
    object[] data = this.data;
    writer.WriteSkippable((Action) (() =>
    {
      foreach (object enumValue in data)
      {
        switch (enumValue)
        {
          case Vector2 vector2_2:
            writer.Write(vector2_2.X);
            writer.Write(vector2_2.Y);
            break;
          case Guid guid2:
            writer.Write(guid2.ToByteArray());
            break;
          case byte[] buffer2:
            writer.Write(buffer2);
            break;
          case bool flag2:
            writer.Write((byte) flag2);
            break;
          case byte num6:
            writer.Write(num6);
            break;
          case int num7:
            writer.Write(num7);
            break;
          case short num8:
            writer.Write(num8);
            break;
          case float num9:
            writer.Write(num9);
            break;
          case long num10:
            writer.Write(num10);
            break;
          case string str2:
            writer.Write(str2);
            break;
          case string[] strArray2:
            writer.Write((byte) strArray2.Length);
            for (int index = 0; index < strArray2.Length; ++index)
              writer.Write(strArray2[index]);
            break;
          case IConvertible _:
            if (!enumValue.GetType().IsValueType)
              throw new InvalidDataException();
            writer.WriteEnum(enumValue);
            break;
          default:
            throw new InvalidDataException();
        }
      }
    }));
  }
}
