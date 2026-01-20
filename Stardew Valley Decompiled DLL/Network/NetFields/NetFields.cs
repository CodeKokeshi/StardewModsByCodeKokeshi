// Decompiled with JetBrains decompiler
// Type: Netcode.NetFields
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

#nullable disable
namespace Netcode;

public class NetFields : AbstractNetSerializable
{
  /// <summary>Whether to run detailed validation checks to detect possible bugs with net fields (e.g. fields which aren't added to the owner's <see cref="T:Netcode.NetFields" /> collection).</summary>
  /// <remarks>These validation checks are expensive and should normally be disabled.</remarks>
  public static bool ShouldValidateNetFields;
  /// <summary>The net fields within the collection to synchronize between players.</summary>
  private readonly List<INetSerializable> fields = new List<INetSerializable>();

  /// <summary>A name for this net field collection, used for troubleshooting network sync.</summary>
  public new string Name { get; }

  /// <summary>The object instance which owns this collection.</summary>
  /// <remarks>This is the instance which has the <see cref="T:Netcode.NetFields" /> property; see also <see cref="P:Netcode.AbstractNetSerializable.Parent" /> for the net field it's synced through (if any). For example, <see cref="P:StardewValley.Character.NetFields" />'s owner is a <see cref="T:StardewValley.Character" /> and its parent is another <see cref="T:Netcode.NetFields" />.</remarks>
  public INetObject<Netcode.NetFields> Owner { get; private set; }

  /// <summary>Construct an instance.</summary>
  /// <param name="name">A name for this net field collection, used for troubleshooting network sync.</param>
  public NetFields(string name) => this.Name = name;

  /// <summary>Set the object instance which owns this collection, used to enable validation and simplify troubleshooting.</summary>
  /// <param name="owner">The instance which owns this net field collection.</param>
  public Netcode.NetFields SetOwner(INetObject<Netcode.NetFields> owner)
  {
    this.Owner = owner;
    return this;
  }

  /// <summary>Get a suggested name for an instance's net field collection, for cases where it's useful to show the name of the subtype.</summary>
  /// <typeparam name="TBaseType">The base type which defines the net field collection.</typeparam>
  /// <param name="instance">The instance which inherits the net field collection.</param>
  public static string GetNameForInstance<TBaseType>(TBaseType instance)
  {
    Type type1 = typeof (TBaseType);
    Type type2 = instance.GetType();
    return !(type1 == type2) ? $"{type1.Name} ({type2.Name})" : type1.Name;
  }

  /// <summary>Get the fields that are in the collection.</summary>
  public IEnumerable<INetSerializable> GetFields() => (IEnumerable<INetSerializable>) this.fields;

  public void CancelInterpolation()
  {
    foreach (INetSerializable field in this.fields)
    {
      if (field is InterpolationCancellable nterpolationCancellable)
        nterpolationCancellable.CancelInterpolation();
    }
  }

  /// <summary>Add a net field to this collection.</summary>
  /// <param name="field">The field to sync as part of this collection.</param>
  /// <param name="name">A readable name for the field within the collection, used for troubleshooting network sync. This should usually be omitted, so it's auto-generated from the expression passed to <paramref name="field" />.</param>
  /// <exception cref="T:System.InvalidOperationException">The field is already part of another collection, or this collection has already been fully initialized.</exception>
  /// <remarks><see cref="M:Netcode.NetFields.SetOwner(Netcode.INetObject{Netcode.NetFields})" /> should be called before any fields are added to enable readable error logs.</remarks>
  public Netcode.NetFields AddField(INetSerializable field, [CallerArgumentExpression("field")] string name = null)
  {
    name = name ?? field.GetType().FullName;
    if (this.Owner == null)
      NetHelper.LogWarning($"Field '{name}' was added to the '{this.Name}' net fields before {"SetOwner"} was called.");
    if (field.Parent != null)
      throw new InvalidOperationException($"Can't add field '{name}' to the '{this.Name}' net fields because it's already part of the {field.Parent.Name} tree.");
    if (this.Parent != null)
      throw new InvalidOperationException($"Can't add field '{name}' to the '{this.Name}' net fields, because they've already been added to a tree.");
    if (Netcode.NetFields.ShouldValidateNetFields)
    {
      foreach (INetSerializable field1 in this.fields)
      {
        if (field == field1)
        {
          NetHelper.LogWarning($"Field '{name}' was added to the '{this.Name}' net fields multiple times.");
          break;
        }
      }
    }
    field.Name = $"{this.Name}: {name}";
    this.fields.Add(field);
    return this;
  }

  protected override void SetParent(INetSerializable parent)
  {
    base.SetParent(parent);
    this.ValidateNetFields();
  }

  /// <summary>Detect and log warnings for common issues like net fields not added to the collection.</summary>
  protected void ValidateNetFields()
  {
    if (this.Owner == null)
      NetHelper.LogWarning($"{nameof (NetFields)} collection '{this.Name}' was initialized without calling {"SetOwner"}, so it can't be validated.");
    else if (this != this.Owner.NetFields)
    {
      NetHelper.LogWarning($"{nameof (NetFields)} collection '{this.Name}' has its own owner set to an {this.Owner?.GetType().FullName} instance whose {nameof (NetFields)} field doesn't reference this collection.");
    }
    else
    {
      if (!Netcode.NetFields.ShouldValidateNetFields)
        return;
      NetFieldValidator.ValidateNetFields(this.Owner, new Action<string>(NetHelper.LogWarning));
    }
  }

  public override void Read(BinaryReader reader, NetVersion version)
  {
    BitArray bitArray = reader.ReadBitArray();
    if (this.fields.Count != bitArray.Length)
      throw new InvalidOperationException();
    for (int index = 0; index < this.fields.Count; ++index)
    {
      if (bitArray[index])
      {
        INetSerializable field = this.fields[index];
        try
        {
          field.Read(reader, version);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Failed reading {this.Name} field '{field.Name}'", ex);
        }
      }
    }
  }

  public override void Write(BinaryWriter writer)
  {
    BitArray bits = new BitArray(this.fields.Count);
    for (int index = 0; index < this.fields.Count; ++index)
      bits[index] = this.fields[index].Dirty;
    writer.WriteBitArray(bits);
    for (int index = 0; index < this.fields.Count; ++index)
    {
      if (bits[index])
      {
        INetSerializable field = this.fields[index];
        writer.Push(Convert.ToString(index));
        try
        {
          field.Write(writer);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Failed writing {this.Name} field '{field.Name}'", ex);
        }
        writer.Pop();
      }
    }
  }

  public override void ReadFull(BinaryReader reader, NetVersion version)
  {
    foreach (INetSerializable field in this.fields)
    {
      try
      {
        field.ReadFull(reader, version);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Failed reading {this.Name} field '{field.Name}'", ex);
      }
    }
  }

  public override void WriteFull(BinaryWriter writer)
  {
    for (int index = 0; index < this.fields.Count; ++index)
    {
      INetSerializable field = this.fields[index];
      writer.Push(Convert.ToString(index));
      try
      {
        field.WriteFull(writer);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Failed writing {this.Name} field '{field.Name}'", ex);
      }
      writer.Pop();
    }
  }

  public virtual void CopyFrom(Netcode.NetFields source)
  {
    try
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) memoryStream))
        {
          using (BinaryReader reader = new BinaryReader((Stream) memoryStream))
          {
            source.WriteFull(writer);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            if (this.Root == null)
              this.ReadFull(reader, new NetClock().netVersion);
            else
              this.ReadFull(reader, this.Root.Clock.netVersion);
            this.MarkClean();
          }
        }
      }
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Failed copying {this.Name} fields from '{source.Name}'", ex);
    }
  }

  protected override void ForEachChild(Action<INetSerializable> childAction)
  {
    foreach (INetSerializable field in this.fields)
      childAction(field);
  }
}
