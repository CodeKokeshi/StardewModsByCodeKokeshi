// Decompiled with JetBrains decompiler
// Type: Netcode.AbstractNetEvent1`1
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;
using System.IO;

#nullable disable
namespace Netcode;

public abstract class AbstractNetEvent1<T> : AbstractNetSerializable
{
  public bool InterpolationWait = true;
  private List<AbstractNetEvent1<T>.EventRecording> outgoingEvents = new List<AbstractNetEvent1<T>.EventRecording>();
  private List<AbstractNetEvent1<T>.EventRecording> incomingEvents = new List<AbstractNetEvent1<T>.EventRecording>();

  public event AbstractNetEvent1<T>.Event onEvent;

  public bool HasPendingEvent(Predicate<T> match)
  {
    return this.incomingEvents.Exists((Predicate<AbstractNetEvent1<T>.EventRecording>) (e => match(e.arg)));
  }

  public void Clear()
  {
    this.outgoingEvents.Clear();
    this.incomingEvents.Clear();
  }

  public void Fire(T arg)
  {
    AbstractNetEvent1<T>.EventRecording eventRecording = new AbstractNetEvent1<T>.EventRecording(arg, this.GetLocalTick());
    this.outgoingEvents.Add(eventRecording);
    this.incomingEvents.Add(eventRecording);
    this.MarkDirty();
    this.Poll();
  }

  public void Poll()
  {
    List<AbstractNetEvent1<T>.EventRecording> eventRecordingList = (List<AbstractNetEvent1<T>.EventRecording>) null;
    foreach (AbstractNetEvent1<T>.EventRecording incomingEvent in this.incomingEvents)
    {
      if (this.Root != null)
      {
        if (this.GetLocalTick() < incomingEvent.timestamp)
          break;
      }
      if (eventRecordingList == null)
        eventRecordingList = new List<AbstractNetEvent1<T>.EventRecording>();
      eventRecordingList.Add(incomingEvent);
    }
    // ISSUE: explicit non-virtual call
    if (eventRecordingList == null || __nonvirtual (eventRecordingList.Count) <= 0)
      return;
    this.incomingEvents.RemoveAll(new Predicate<AbstractNetEvent1<T>.EventRecording>(eventRecordingList.Contains));
    if (this.onEvent == null)
      return;
    foreach (AbstractNetEvent1<T>.EventRecording eventRecording in eventRecordingList)
      this.onEvent(eventRecording.arg);
  }

  protected abstract T readEventArg(BinaryReader reader, NetVersion version);

  protected abstract void writeEventArg(BinaryWriter writer, T arg);

  public override void Read(BinaryReader reader, NetVersion version)
  {
    uint num1 = reader.Read7BitEncoded();
    uint localTick = this.GetLocalTick();
    if (this.InterpolationWait)
      localTick += (uint) this.Root.Clock.InterpolationTicks;
    for (uint index = 0; index < num1; ++index)
    {
      uint num2 = reader.ReadUInt32();
      this.incomingEvents.Add(new AbstractNetEvent1<T>.EventRecording(this.readEventArg(reader, version), localTick + num2));
    }
    this.ChangeVersion.Merge(version);
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    this.ChangeVersion.Merge(version);
  }

  public override void Write(BinaryWriter writer)
  {
    writer.Write7BitEncoded((uint) this.outgoingEvents.Count);
    if (this.outgoingEvents.Count > 0)
    {
      uint timestamp = this.outgoingEvents[0].timestamp;
      foreach (AbstractNetEvent1<T>.EventRecording outgoingEvent in this.outgoingEvents)
      {
        writer.Write(outgoingEvent.timestamp - timestamp);
        this.writeEventArg(writer, outgoingEvent.arg);
      }
    }
    this.outgoingEvents.Clear();
  }

  protected override void CleanImpl()
  {
    base.CleanImpl();
    this.outgoingEvents.Clear();
  }

  public override void WriteFull(BinaryWriter writer)
  {
  }

  public class EventRecording
  {
    public T arg;
    public uint timestamp;

    public EventRecording(T arg, uint timestamp)
    {
      this.arg = arg;
      this.timestamp = timestamp;
    }
  }

  public delegate void Event(T arg);
}
