// Decompiled with JetBrains decompiler
// Type: ContentManifest.Internal.CHValue
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;

#nullable disable
namespace ContentManifest.Internal;

internal class CHValue : CHParsable
{
  public CHValueUnion RawValue;
  public CHValueEnum ValueType = CHValueEnum.Unknown;

  public CHValue() => this.RawValue.ValueNull = (object) null;

  public void Parse(CHJsonParserContext context)
  {
    if (context.ReadHead >= context.JsonText.Length)
      throw new InvalidOperationException();
    char prefixChar = context.JsonText[context.ReadHead];
    CHParsable chParsable;
    switch (prefixChar)
    {
      case '"':
        chParsable = (CHParsable) (this.RawValue.ValueString = new CHString());
        this.ValueType = CHValueEnum.String;
        break;
      case '[':
        chParsable = (CHParsable) (this.RawValue.ValueArray = new CHArray());
        this.ValueType = CHValueEnum.Array;
        break;
      case 'f':
      case 't':
        chParsable = (CHParsable) (this.RawValue.ValueBoolean = new CHBoolean());
        this.ValueType = CHValueEnum.Boolean;
        break;
      case 'n':
        if (context.ReadHead + 3 >= context.JsonText.Length)
          throw new InvalidOperationException();
        if (context.JsonText[context.ReadHead + 1] != 'u' || context.JsonText[context.ReadHead + 2] != 'l' || context.JsonText[context.ReadHead + 3] != 'l')
          throw new InvalidOperationException();
        chParsable = (CHParsable) null;
        this.ValueType = CHValueEnum.Null;
        break;
      case '{':
        chParsable = (CHParsable) (this.RawValue.ValueObject = new CHObject());
        this.ValueType = CHValueEnum.Object;
        break;
      default:
        if (!CHNumber.IsValidPrefix(prefixChar))
          throw new InvalidOperationException();
        chParsable = (CHParsable) (this.RawValue.ValueNumber = new CHNumber());
        this.ValueType = CHValueEnum.Number;
        break;
    }
    chParsable?.Parse(context);
  }

  public object GetManagedObject()
  {
    switch (this.ValueType)
    {
      case CHValueEnum.Object:
        return (object) this.RawValue.ValueObject.Members;
      case CHValueEnum.Array:
        return (object) this.RawValue.ValueArray.Elements;
      case CHValueEnum.String:
        return (object) this.RawValue.ValueString.RawString;
      case CHValueEnum.Number:
        return (object) this.RawValue.ValueNumber.RawDouble;
      case CHValueEnum.Boolean:
        return (object) this.RawValue.ValueBoolean.RawBoolean;
      case CHValueEnum.Null:
        return (object) null;
      default:
        throw new InvalidOperationException();
    }
  }
}
