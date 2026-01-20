// Decompiled with JetBrains decompiler
// Type: StardewValley.Network.NetAudio
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Audio;
using System;
using System.IO;

#nullable disable
namespace StardewValley.Network;

public class NetAudio : INetObject<NetFields>
{
  private readonly NetEventBinary audioEvent = new NetEventBinary();
  /// <summary>The backing field for <see cref="P:StardewValley.Network.NetAudio.ActiveCues" />.</summary>
  private readonly NetStringDictionary<bool, NetBool> activeCues = new NetStringDictionary<bool, NetBool>();
  /// <summary>The location whose audio this instance manages.</summary>
  private GameLocation location;

  public NetFields NetFields { get; } = new NetFields(nameof (NetAudio));

  /// <summary>The sound IDs to play continuously until they're removed from the list.</summary>
  public NetDictionary<string, bool, NetBool, SerializableDictionary<string, bool>, NetStringDictionary<bool, NetBool>>.KeysCollection ActiveCues
  {
    get => this.activeCues.Keys;
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="location">The location whose audio this instance manages.</param>
  public NetAudio(GameLocation location)
  {
    this.location = location;
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this.audioEvent, nameof (audioEvent)).AddField((INetSerializable) this.activeCues, nameof (activeCues));
    this.audioEvent.AddReaderHandler(new Action<BinaryReader>(this.handleAudioEvent));
  }

  private void handleAudioEvent(BinaryReader reader)
  {
    string audioName;
    Vector2? position;
    int? pitch;
    SoundContext context;
    this.Read(reader, out audioName, out position, out pitch, out context);
    Game1.sounds.PlayLocal(audioName, this.location, position, pitch, context, out ICue _);
  }

  public void Update() => this.audioEvent.Poll();

  /// <summary>Send an audio cue to all players, including the current one.</summary>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="position">The tile position from which the sound is playing, or <c>null</c> if it's playing throughout the location.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> for the default pitch.</param>
  /// <param name="context">The source which triggered a game sound.</param>
  public void Fire(string audioName, Vector2? position, int? pitch, SoundContext context)
  {
    this.audioEvent.Fire((NetEventBinary.ArgWriter) (writer =>
    {
      writer.Write(audioName);
      writer.WriteVector2(position ?? new Vector2((float) int.MinValue));
      writer.Write(pitch ?? int.MinValue);
      writer.Write((int) context);
    }));
    this.audioEvent.Poll();
  }

  /// <summary>Read an audio cue from the network that was sent via <see cref="M:StardewValley.Network.NetAudio.Fire(System.String,System.Nullable{Microsoft.Xna.Framework.Vector2},System.Nullable{System.Int32},StardewValley.Audio.SoundContext)" />.</summary>
  /// <param name="reader">The network input reader.</param>
  /// <param name="audioName">The sound ID to play.</param>
  /// <param name="position">The tile position from which the sound is playing, or <c>null</c> if it's playing throughout the location.</param>
  /// <param name="pitch">The pitch modifier to apply, or <c>null</c> for the default pitch.</param>
  /// <param name="context">The source which triggered a game sound.</param>
  public void Read(
    BinaryReader reader,
    out string audioName,
    out Vector2? position,
    out int? pitch,
    out SoundContext context)
  {
    audioName = reader.ReadString();
    position = new Vector2?(reader.ReadVector2());
    pitch = new int?(reader.ReadInt32());
    context = (SoundContext) reader.ReadInt32();
    if ((int) position.Value.X == int.MinValue && (int) position.Value.Y == int.MinValue)
      position = new Vector2?();
    if (pitch.GetValueOrDefault() != int.MinValue)
      return;
    pitch = new int?();
  }

  /// <summary>Play a sound continuously until it's stopped via <see cref="M:StardewValley.Network.NetAudio.StopPlaying(System.String)" />.</summary>
  /// <param name="cueName">The sound ID to play.</param>
  public void StartPlaying(string cueName) => this.activeCues[cueName] = false;

  /// <summary>Stop a sound that is playing continuously after <see cref="M:StardewValley.Network.NetAudio.StartPlaying(System.String)" />.</summary>
  /// <param name="cueName">The sound ID to stop.</param>
  public void StopPlaying(string cueName) => this.activeCues.Remove(cueName);
}
