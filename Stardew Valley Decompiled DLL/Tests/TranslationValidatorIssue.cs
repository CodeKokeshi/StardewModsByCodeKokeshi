// Decompiled with JetBrains decompiler
// Type: StardewValley.Tests.TranslationValidatorIssue
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

#nullable disable
namespace StardewValley.Tests;

/// <summary>An issue type detected by the translation validator.</summary>
public enum TranslationValidatorIssue
{
  /// <summary>The translations are missing a key found in the base data.</summary>
  MissingKey,
  /// <summary>The translations have an extra key that doesn't exist in the base data.</summary>
  UnknownKey,
  /// <summary>The translation produces a different <see cref="T:StardewValley.Tests.SyntaxAbstractor">syntactic representation</see> than the base key.</summary>
  SyntaxMismatch,
  /// <summary>A syntax block is malformed or invalid.</summary>
  MalformedSyntax,
}
