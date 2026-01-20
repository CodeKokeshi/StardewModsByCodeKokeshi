// Decompiled with JetBrains decompiler
// Type: StardewValley.Tests.TranslationValidator
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using System;
using System.Collections.Generic;

#nullable enable
namespace StardewValley.Tests;

/// <summary>Provides methods to compare and validate translations, used in the game's internal unit tests.</summary>
public class TranslationValidator
{
  /// <summary>Converts raw text into language-independent syntax representations, which can be compared between languages.</summary>
  private readonly 
  #nullable disable
  SyntaxAbstractor Abstractor = new SyntaxAbstractor();

  /// <summary>Compare the base and translated variants of an asset and return a list of keys which are missing, unknown, or have a different syntax.</summary>
  /// <typeparam name="TValue">The value type in the asset data.</typeparam>
  /// <param name="baseData">The original untranslated data.</param>
  /// <param name="translatedData">The translated data.</param>
  /// <param name="getText">Get the text to compare for an entry.</param>
  /// <param name="baseAssetName">The asset name without the locale suffix, like <c>Data/Achievements</c>.</param>
  public IEnumerable<TranslationValidatorResult> Compare<TValue>(
    Dictionary<string, TValue> baseData,
    Dictionary<string, TValue> translatedData,
    Func<TValue, string> getText,
    string baseAssetName)
  {
    return this.Compare<TValue>(baseData, translatedData, getText, (Func<string, string, string>) ((key, text) => this.Abstractor.ExtractSyntaxFor(baseAssetName, key, text)));
  }

  /// <summary>Compare the base and translated variants of an asset and return a list of keys which are missing, unknown, or have a different syntax.</summary>
  /// <typeparam name="TValue">The value type in the asset data.</typeparam>
  /// <param name="baseData">The original untranslated data.</param>
  /// <param name="translatedData">The translated data.</param>
  /// <param name="getText">Get the text to compare for an entry.</param>
  /// <param name="getSyntax">Get the syntax for a data entry, given its key and value.</param>
  public IEnumerable<TranslationValidatorResult> Compare<TValue>(
    Dictionary<string, TValue> baseData,
    Dictionary<string, TValue> translatedData,
    Func<TValue, string> getText,
    Func<string, string, string> getSyntax)
  {
    foreach (KeyValuePair<string, TValue> keyValuePair in baseData)
    {
      string key = keyValuePair.Key;
      string baseText = getText(keyValuePair.Value);
      TValue obj;
      if (!translatedData.TryGetValue(key, out obj))
      {
        yield return new TranslationValidatorResult(TranslationValidatorIssue.MissingKey, key, getSyntax(key, baseText), baseText, (string) null, (string) null, "Key not found in the translated asset.");
      }
      else
      {
        string translationText = getText(obj);
        TranslationValidatorResult translationValidatorResult = this.CompareEntry(key, baseText, translationText, (Func<string, string>) (value => getSyntax(key, value)));
        if (translationValidatorResult != null)
          yield return translationValidatorResult;
      }
    }
    foreach (KeyValuePair<string, TValue> keyValuePair in translatedData)
    {
      string key = keyValuePair.Key;
      if (!baseData.ContainsKey(key))
      {
        string translationText = getText(keyValuePair.Value);
        string translationSyntax = getSyntax(key, translationText);
        yield return new TranslationValidatorResult(TranslationValidatorIssue.UnknownKey, key, (string) null, (string) null, translationSyntax, translationText, "Unknown key in translation which isn't in the base asset.");
      }
    }
  }

  /// <summary>Compare the base and translated variants of a single entry in an asset and return a result if the entries have a different syntax.</summary>
  /// <param name="key">The key for this entry in the asset.</param>
  /// <param name="baseText">The original untranslated text.</param>
  /// <param name="translationText">The translated text.</param>
  /// <param name="getSyntax">Get the syntax for an entry, given its value.</param>
  /// <returns>Returns the validator result if an issue was found, else <c>null</c>.</returns>
  public TranslationValidatorResult CompareEntry(
    string key,
    string baseText,
    string translationText,
    Func<string, string> getSyntax)
  {
    string str1 = getSyntax(baseText);
    string str2 = getSyntax(translationText);
    if (str1 != str2)
      return new TranslationValidatorResult(TranslationValidatorIssue.SyntaxMismatch, key, str1, baseText, str2, translationText, $"The translation has a different syntax than the base text.\nSyntax:\n    base:  {str1}\n    local: {str2}\n           {"".PadRight(this.GetDiffIndex(str1, str2), ' ')}^\nText:\n    base:  {baseText}\n    local: {translationText}\n\n           {"".PadRight(this.GetDiffIndex(baseText, translationText), ' ')}^\n");
    string error;
    string errorBlock;
    if (!this.ValidateGenderSwitchBlocks(baseText, out error, out errorBlock))
      return new TranslationValidatorResult(TranslationValidatorIssue.MalformedSyntax, key, str1, baseText, str2, translationText, $"Base text has invalid gender switch block: {error}.\nAffected block: {errorBlock}.");
    if (this.ValidateGenderSwitchBlocks(baseText, out error, out errorBlock))
      return (TranslationValidatorResult) null;
    return new TranslationValidatorResult(TranslationValidatorIssue.MalformedSyntax, key, str1, baseText, str2, translationText, $"Translated text has invalid gender switch block: {error}.\nAffected block: {errorBlock}.");
  }

  /// <summary>Validate that all gender-switch blocks in a given text are correctly formatted.</summary>
  /// <param name="text">The text which may contain gender-switch blocks to validate.</param>
  /// <param name="error">If applicable, a human-readable phrase indicating why the gender-switch blocks are invalid.</param>
  /// <param name="errorBlock">The gender-switch block which is invalid.</param>
  public bool ValidateGenderSwitchBlocks(string text, out string error, out string errorBlock)
  {
    int startIndex1 = 0;
    int startIndex2;
    char separator;
    string[] strArray;
    while (true)
    {
      startIndex2 = text.IndexOf("${", startIndex1, StringComparison.OrdinalIgnoreCase);
      if (startIndex2 != -1)
      {
        int num = text.IndexOf("}$", startIndex2, StringComparison.OrdinalIgnoreCase);
        if (num != -1)
        {
          errorBlock = text.Substring(startIndex2, num - startIndex2);
          string str = text.Substring(startIndex2 + 2, num - startIndex2 - 2);
          separator = str.Contains('^') ? '^' : '¦';
          strArray = str.Split(separator);
          if (!str.Contains("${"))
          {
            if (strArray.Length >= 2)
            {
              if (strArray.Length <= 3)
              {
                string dialogueSyntax1 = this.Abstractor.ExtractDialogueSyntax(strArray[0]);
                for (int index = 1; index < strArray.Length; ++index)
                {
                  string dialogueSyntax2 = this.Abstractor.ExtractDialogueSyntax(strArray[1]);
                  if (dialogueSyntax1 != dialogueSyntax2)
                  {
                    error = $"branches have different syntax (0: `{dialogueSyntax1}`, {index}: `{dialogueSyntax2}`)";
                    return false;
                  }
                }
                startIndex1 = num + 2;
              }
              else
                goto label_9;
            }
            else
              goto label_7;
          }
          else
            goto label_5;
        }
        else
          break;
      }
      else
        goto label_16;
    }
    error = "closing '}$' not found";
    errorBlock = text.Substring(startIndex2);
    return false;
label_5:
    error = "can't start a new gender-switch block inside another";
    return false;
label_7:
    error = $"must have at least two branches delimited by {'^'} or {'¦'}";
    return false;
label_9:
    error = $"found {strArray.Length} branches delimited by {separator}, must be two (male{separator}female) or three (male{separator}female{separator}other)";
    return false;
label_16:
    error = (string) null;
    errorBlock = (string) null;
    return true;
  }

  /// <summary>Get the index at which two strings first differ.</summary>
  /// <param name="baseText">The base text being compare to.</param>
  /// <param name="translatedText">The translated text to compare with the base text.</param>
  public int GetDiffIndex(string baseText, string translatedText)
  {
    int diffIndex = Math.Min(baseText.Length, translatedText.Length);
    for (int index = 0; index < diffIndex; ++index)
    {
      if ((int) baseText[index] != (int) translatedText[index])
        return index;
    }
    return diffIndex;
  }
}
