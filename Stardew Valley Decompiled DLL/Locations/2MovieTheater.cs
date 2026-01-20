// Decompiled with JetBrains decompiler
// Type: StardewValley.MovieViewerLockEvent
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace StardewValley;

public class MovieViewerLockEvent : NetEventArg
{
  public List<long> uids;
  public int movieStartTime;

  public MovieViewerLockEvent()
  {
    this.uids = new List<long>();
    this.movieStartTime = 0;
  }

  public MovieViewerLockEvent(List<Farmer> present_farmers, int movie_start_time)
  {
    this.movieStartTime = movie_start_time;
    this.uids = new List<long>();
    foreach (Farmer presentFarmer in present_farmers)
      this.uids.Add(presentFarmer.UniqueMultiplayerID);
  }

  public void Read(BinaryReader reader)
  {
    this.uids.Clear();
    this.movieStartTime = reader.ReadInt32();
    int num = reader.ReadInt32();
    for (int index = 0; index < num; ++index)
      this.uids.Add(reader.ReadInt64());
  }

  public void Write(BinaryWriter writer)
  {
    writer.Write(this.movieStartTime);
    writer.Write(this.uids.Count);
    for (int index = 0; index < this.uids.Count; ++index)
      writer.Write(this.uids[index]);
  }
}
