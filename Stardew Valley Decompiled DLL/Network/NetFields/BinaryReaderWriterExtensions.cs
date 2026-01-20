// Decompiled with JetBrains decompiler
// Type: Netcode.BinaryReaderWriterExtensions
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.IO;

#nullable disable
namespace Netcode;

public static class BinaryReaderWriterExtensions
{
  public static void ReadSkippable(this BinaryReader reader, Action readAction)
  {
    uint num = reader.ReadUInt32();
    long position = reader.BaseStream.Position;
    readAction();
    if (reader.BaseStream.Position > position + (long) num)
      throw new InvalidOperationException();
    reader.BaseStream.Position = position + (long) num;
  }

  public static byte[] ReadSkippableBytes(this BinaryReader reader)
  {
    uint count = reader.ReadUInt32();
    return reader.ReadBytes((int) count);
  }

  public static void Skip(this BinaryReader reader) => reader.ReadSkippable((Action) (() => { }));

  public static void WriteSkippable(this BinaryWriter writer, Action writeAction)
  {
    long position1 = writer.BaseStream.Position;
    writer.Write(0U);
    long position2 = writer.BaseStream.Position;
    writeAction();
    long position3 = writer.BaseStream.Position;
    long num = position3 - position2;
    writer.BaseStream.Position = position1;
    writer.Write((uint) num);
    writer.BaseStream.Position = position3;
  }

  public static BitArray ReadBitArray(this BinaryReader reader)
  {
    int num = (int) reader.Read7BitEncoded();
    return new BitArray(reader.ReadBytes((num + 7) / 8))
    {
      Length = num
    };
  }

  public static void WriteBitArray(this BinaryWriter writer, BitArray bits)
  {
    byte[] buffer = new byte[(bits.Length + 7) / 8];
    bits.CopyTo((Array) buffer, 0);
    writer.Write7BitEncoded((uint) bits.Length);
    writer.Write(buffer);
  }

  public static void Write7BitEncoded(this BinaryWriter writer, uint value)
  {
    do
    {
      byte num = (byte) (value & (uint) sbyte.MaxValue);
      value >>= 7;
      if (value != 0U)
        num |= (byte) 128 /*0x80*/;
      writer.Write(num);
    }
    while (value != 0U);
  }

  public static uint Read7BitEncoded(this BinaryReader reader)
  {
    uint num1 = 0;
    byte num2 = reader.ReadByte();
    int num3 = 0;
    for (; ((int) num2 & 128 /*0x80*/) != 0; num2 = reader.ReadByte())
    {
      num1 |= (uint) (((int) num2 & (int) sbyte.MaxValue) << num3);
      num3 += 7;
    }
    return num1 | (uint) (((int) num2 & (int) sbyte.MaxValue) << num3);
  }

  public static Guid ReadGuid(this BinaryReader reader) => new Guid(reader.ReadBytes(16 /*0x10*/));

  public static void WriteGuid(this BinaryWriter writer, Guid guid)
  {
    writer.Write(guid.ToByteArray());
  }

  public static Vector2 ReadVector2(this BinaryReader reader)
  {
    return new Vector2(reader.ReadSingle(), reader.ReadSingle());
  }

  public static void WriteVector2(this BinaryWriter writer, Vector2 vec)
  {
    writer.Write(vec.X);
    writer.Write(vec.Y);
  }

  public static Point ReadPoint(this BinaryReader reader)
  {
    return new Point(reader.ReadInt32(), reader.ReadInt32());
  }

  public static void WritePoint(this BinaryWriter writer, Point p)
  {
    writer.Write(p.X);
    writer.Write(p.Y);
  }

  public static Rectangle ReadRectangle(this BinaryReader reader)
  {
    Point point1 = reader.ReadPoint();
    Point point2 = reader.ReadPoint();
    return new Rectangle(point1.X, point1.Y, point2.X, point2.Y);
  }

  public static void WriteRectangle(this BinaryWriter writer, Rectangle rect)
  {
    writer.WritePoint(rect.Location);
    writer.WritePoint(new Point(rect.Width, rect.Height));
  }

  public static Color ReadColor(this BinaryReader reader)
  {
    return new Color() { PackedValue = reader.ReadUInt32() };
  }

  public static void WriteColor(this BinaryWriter writer, Color color)
  {
    writer.Write(color.PackedValue);
  }

  public static T ReadEnum<T>(this BinaryReader reader) where T : struct, IConvertible
  {
    return (T) Enum.ToObject(typeof (T), reader.ReadInt16());
  }

  public static void WriteEnum<T>(this BinaryWriter writer, T enumValue) where T : struct, IConvertible
  {
    writer.Write(Convert.ToInt16((object) enumValue));
  }

  public static void WriteEnum(this BinaryWriter writer, object enumValue)
  {
    writer.Write(Convert.ToInt16(enumValue));
  }

  public static void Push(this BinaryWriter writer, string name)
  {
    if (!(writer is ILoggingWriter loggingWriter))
      return;
    loggingWriter.Push(name);
  }

  public static void Pop(this BinaryWriter writer)
  {
    if (!(writer is ILoggingWriter loggingWriter))
      return;
    loggingWriter.Pop();
  }
}
