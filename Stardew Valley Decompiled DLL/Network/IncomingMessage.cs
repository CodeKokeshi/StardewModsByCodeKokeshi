// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.IncomingMessage
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public class IncomingMessage : IDisposable
{
  private byte messageType;
  private long farmerID;
  private byte[] data;
  private MemoryStream stream;
  private BinaryReader reader;

  public byte MessageType => this.messageType;

  public long FarmerID => this.farmerID;

  public Farmer SourceFarmer => Game1.GetPlayer(this.farmerID) ?? Game1.MasterPlayer;

  public byte[] Data => this.data;

  public BinaryReader Reader => this.reader;

  public void Read(BinaryReader reader)
  {
    this.Dispose();
    this.messageType = reader.ReadByte();
    this.farmerID = reader.ReadInt64();
    this.data = reader.ReadSkippableBytes();
    this.stream = new MemoryStream(this.data);
    this.reader = new BinaryReader((Stream) this.stream);
  }

  public void Dispose()
  {
    this.reader?.Dispose();
    this.stream?.Dispose();
    this.stream = (MemoryStream) null;
    this.reader = (BinaryReader) null;
  }
}
