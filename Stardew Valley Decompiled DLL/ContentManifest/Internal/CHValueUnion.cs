// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHValueUnion
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System.Runtime.InteropServices;

#nullable disable
namespace ContentManifest.Internal;

[StructLayout(LayoutKind.Explicit)]
internal struct CHValueUnion
{
  [FieldOffset(0)]
  public CHObject ValueObject;
  [FieldOffset(0)]
  public CHArray ValueArray;
  [FieldOffset(0)]
  public CHString ValueString;
  [FieldOffset(0)]
  public CHNumber ValueNumber;
  [FieldOffset(0)]
  public CHBoolean ValueBoolean;
  [FieldOffset(0)]
  public object ValueNull;
}
