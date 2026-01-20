// Decompiled with JetBrains decompiler
// Type: StardewValley.InteriorDoorDictionary
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
namespace StardewValley;

public class InteriorDoorDictionary : NetPointDictionary<bool, 
#nullable disable
InteriorDoor>
{
  private GameLocation location;

  public InteriorDoorDictionary.DoorCollection Doors
  {
    get => new InteriorDoorDictionary.DoorCollection(this);
  }

  public InteriorDoorDictionary(GameLocation location) => this.location = location;

  protected override void setFieldValue(InteriorDoor door, Point position, bool open)
  {
    door.Location = this.location;
    door.Position = position;
    base.setFieldValue(door, position, open);
  }

  public void ResetSharedState()
  {
    if (this.location.isOutdoors.Value)
      return;
    foreach (Point key in InteriorDoorDictionary.GetDoorTilesFromMapProperty(this.location))
      this[key] = false;
  }

  public void ResetLocalState()
  {
    if (this.location.isOutdoors.Value)
      return;
    foreach (Point key in InteriorDoorDictionary.GetDoorTilesFromMapProperty(this.location))
    {
      if (this.ContainsKey(key))
      {
        InteriorDoor interiorDoor = this.FieldDict[key];
        interiorDoor.Location = this.location;
        interiorDoor.Position = key;
        interiorDoor.ResetLocalState();
      }
    }
  }

  /// <summary>Get the tile positions containing doors based on the <c>Doors</c> map property.</summary>
  /// <param name="location">The location whose map property to read.</param>
  public static IEnumerable<Point> GetDoorTilesFromMapProperty(GameLocation location)
  {
    string[] fields = location.GetMapPropertySplitBySpaces("Doors");
    for (int i = 0; i < fields.Length; i += 4)
    {
      Point point;
      string error;
      if (ArgUtility.TryGetPoint(fields, i, out point, out error, "Point tile"))
        yield return point;
      else
        location.LogMapPropertyError("Doors", fields, error);
    }
  }

  public void MakeMapModifications()
  {
    foreach (InteriorDoor door in this.Doors)
      door.ApplyMapModifications();
  }

  public void CleanUpLocalState()
  {
    foreach (InteriorDoor door in this.Doors)
      door.CleanUpLocalState();
  }

  public void Update(GameTime time)
  {
    foreach (InteriorDoor door in this.Doors)
      door.Update(time);
  }

  public void Draw(SpriteBatch b)
  {
    foreach (InteriorDoor door in this.Doors)
      door.Draw(b);
  }

  public struct DoorCollection(InteriorDoorDictionary dict) : IEnumerable<InteriorDoor>, IEnumerable
  {
    private InteriorDoorDictionary _dict = dict;

    public InteriorDoorDictionary.DoorCollection.Enumerator GetEnumerator()
    {
      return new InteriorDoorDictionary.DoorCollection.Enumerator(this._dict);
    }

    IEnumerator<InteriorDoor> IEnumerable<InteriorDoor>.GetEnumerator()
    {
      return (IEnumerator<InteriorDoor>) new InteriorDoorDictionary.DoorCollection.Enumerator(this._dict);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new InteriorDoorDictionary.DoorCollection.Enumerator(this._dict);
    }

    public struct Enumerator : IEnumerator<InteriorDoor>, IEnumerator, IDisposable
    {
      private readonly InteriorDoorDictionary _dict;
      private Dictionary<Point, InteriorDoor>.Enumerator _enumerator;
      private InteriorDoor _current;
      private bool _done;

      public Enumerator(InteriorDoorDictionary dict)
      {
        this._dict = dict;
        this._enumerator = this._dict.FieldDict.GetEnumerator();
        this._current = (InteriorDoor) null;
        this._done = false;
      }

      public bool MoveNext()
      {
        if (this._enumerator.MoveNext())
        {
          KeyValuePair<Point, InteriorDoor> current = this._enumerator.Current;
          this._current = current.Value;
          this._current.Location = this._dict.location;
          this._current.Position = current.Key;
          return true;
        }
        this._done = true;
        this._current = (InteriorDoor) null;
        return false;
      }

      public InteriorDoor Current => this._current;

      public void Dispose()
      {
      }

      object IEnumerator.Current
      {
        get
        {
          if (this._done)
            throw new InvalidOperationException();
          return (object) this._current;
        }
      }

      void IEnumerator.Reset()
      {
        this._enumerator = this._dict.FieldDict.GetEnumerator();
        this._current = (InteriorDoor) null;
        this._done = false;
      }
    }
  }
}
