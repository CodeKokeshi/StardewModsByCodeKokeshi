// Decompiled with JetBrains decompiler
// Type: StardewValley.LocalizedContentManager
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using ContentManifest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewValley.GameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

#nullable disable
namespace StardewValley;

/// <summary>Loads assets and translations from the game's content folder.</summary>
/// <summary>Loads assets and translations from the game's content folder.</summary>
public class LocalizedContentManager : ContentManager
{
  private const bool OnlyCheckManifest = true;
  private static readonly object ManifestLocker = new object();
  private static HashSet<string> _manifest = (HashSet<string>) null;
  public static readonly Dictionary<string, string> localizedAssetNames = new Dictionary<string, string>();
  /// <summary>The backing field for <see cref="M:StardewValley.LocalizedContentManager.GetContentRoot" />.</summary>
  protected string _CachedContentRoot;
  /// <summary>The backing field for <see cref="P:StardewValley.LocalizedContentManager.CurrentLanguageCode" />.</summary>
  private static LocalizedContentManager.LanguageCode _currentLangCode = LocalizedContentManager.GetDefaultLanguageCode();
  /// <summary>The backing field for <see cref="P:StardewValley.LocalizedContentManager.CurrentLanguageString" />.</summary>
  private static string _currentLangString = (string) null;
  private static ModLanguage _currentModLanguage = (ModLanguage) null;
  public CultureInfo CurrentCulture;
  protected static StringBuilder _timeFormatStringBuilder = new StringBuilder();

  public static event LocalizedContentManager.LanguageChangedHandler OnLanguageChange;

  private void PlatformEnsureManifestInitialized()
  {
    if (LocalizedContentManager._manifest != null)
      return;
    LocalizedContentManager._manifest = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    string str = Path.Combine(this.GetContentRoot(), "ContentHashes.json");
    if (File.Exists(str))
    {
      Dictionary<string, object> dictionary = (Dictionary<string, object>) null;
      try
      {
        dictionary = ContentHashParser.ParseFromFile(str);
      }
      catch (Exception ex)
      {
        Game1.log.Error("Error parsing ContentHashes.json:", ex);
      }
      if (dictionary == null || dictionary.Count == 0)
      {
        Game1.log.Warn("Parsing ContentHashes.json resulted in a null or empty dictionary.");
      }
      else
      {
        Game1.log.Verbose($"Successfully loaded ContentHashes.json containing {dictionary.Count} file(s);");
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
          foreach (string key in dictionary.Keys)
            LocalizedContentManager._manifest.Add(key.Replace('/', '\\'));
        }
        else
          LocalizedContentManager._manifest.UnionWith((IEnumerable<string>) dictionary.Keys);
      }
    }
    else
      Game1.log.Warn($"Could not find ContentHashes at path '{str}'");
  }

  private void EnsureManifestInitialized()
  {
    if (LocalizedContentManager._manifest != null)
      return;
    lock (LocalizedContentManager.ManifestLocker)
    {
      if (LocalizedContentManager._manifest != null)
        return;
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.PlatformEnsureManifestInitialized();
      stopwatch.Stop();
      Game1.log.Verbose($"EnsureManifestInitialized() finished, elapsed = '{stopwatch.Elapsed}'");
    }
  }

  public static LocalizedContentManager.LanguageCode GetDefaultLanguageCode()
  {
    return LocalizedContentManager.LanguageCode.en;
  }

  /// <summary>The current language as a string which appears in localized asset names (like <c>pt-BR</c>).</summary>
  public static string CurrentLanguageString
  {
    get
    {
      if (LocalizedContentManager._currentLangString == null)
        LocalizedContentManager._currentLangString = LocalizedContentManager.LanguageCodeString(LocalizedContentManager.CurrentLanguageCode);
      return LocalizedContentManager._currentLangString;
    }
  }

  /// <summary>The current language as an enum.</summary>
  /// <remarks>Note that <see cref="F:StardewValley.LocalizedContentManager.LanguageCode.mod" /> is used for any custom language, so you'll need to use <see cref="P:StardewValley.LocalizedContentManager.CurrentLanguageString" /> to distinguish those.</remarks>
  public static LocalizedContentManager.LanguageCode CurrentLanguageCode
  {
    get => LocalizedContentManager._currentLangCode;
    set
    {
      if (LocalizedContentManager._currentLangCode == value)
        return;
      LocalizedContentManager.LanguageCode currentLangCode = LocalizedContentManager._currentLangCode;
      LocalizedContentManager._currentLangCode = value;
      LocalizedContentManager._currentLangString = (string) null;
      if (LocalizedContentManager._currentLangCode != LocalizedContentManager.LanguageCode.mod)
        LocalizedContentManager._currentModLanguage = (ModLanguage) null;
      Game1.log.Verbose($"LocalizedContentManager.CurrentLanguageCode CHANGING from '{currentLangCode.ToString()}' to '{LocalizedContentManager._currentLangCode.ToString()}'");
      LocalizedContentManager.LanguageChangedHandler onLanguageChange = LocalizedContentManager.OnLanguageChange;
      if (onLanguageChange != null)
        onLanguageChange(LocalizedContentManager._currentLangCode);
      Game1.log.Verbose($"LocalizedContentManager.CurrentLanguageCode CHANGED from '{currentLangCode.ToString()}' to '{LocalizedContentManager._currentLangCode.ToString()}'");
    }
  }

  public static bool CurrentLanguageLatin
  {
    get
    {
      switch (LocalizedContentManager.CurrentLanguageCode)
      {
        case LocalizedContentManager.LanguageCode.en:
        case LocalizedContentManager.LanguageCode.pt:
        case LocalizedContentManager.LanguageCode.es:
        case LocalizedContentManager.LanguageCode.de:
        case LocalizedContentManager.LanguageCode.fr:
        case LocalizedContentManager.LanguageCode.it:
        case LocalizedContentManager.LanguageCode.tr:
        case LocalizedContentManager.LanguageCode.hu:
          return true;
        case LocalizedContentManager.LanguageCode.mod:
          return LocalizedContentManager._currentModLanguage.UseLatinFont;
        default:
          return false;
      }
    }
  }

  public LocalizedContentManager(
    IServiceProvider serviceProvider,
    string rootDirectory,
    CultureInfo currentCulture)
    : base(serviceProvider, rootDirectory)
  {
    this.CurrentCulture = currentCulture;
  }

  public LocalizedContentManager(IServiceProvider serviceProvider, string rootDirectory)
    : this(serviceProvider, rootDirectory, Thread.CurrentThread.CurrentUICulture)
  {
  }

  public static ModLanguage CurrentModLanguage => LocalizedContentManager._currentModLanguage;

  protected static bool _IsStringAt(string source, string string_to_find, int index)
  {
    for (int index1 = 0; index1 < string_to_find.Length; ++index1)
    {
      int index2 = index + index1;
      if (index2 >= source.Length || (int) source[index2] != (int) string_to_find[index1])
        return false;
    }
    return true;
  }

  public static StringBuilder FormatTimeString(int time, string format)
  {
    LocalizedContentManager._timeFormatStringBuilder.Clear();
    int index1 = -1;
    for (int index2 = 0; index2 < format.Length; ++index2)
    {
      char ch = format[index2];
      switch (ch)
      {
        case '[':
          if (index1 < 0)
          {
            index1 = index2;
            break;
          }
          for (int index3 = index1; index3 <= index2; ++index3)
            LocalizedContentManager._timeFormatStringBuilder.Append(format[index3]);
          index1 = index2;
          break;
        case ']':
          if (index1 >= 0)
          {
            int num;
            if (LocalizedContentManager._IsStringAt(format, "[HOURS_12]", index1))
            {
              StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
              string str;
              if (time / 100 % 12 != 0)
              {
                num = time / 100 % 12;
                str = num.ToString();
              }
              else
                str = "12";
              formatStringBuilder.Append(str);
            }
            else if (LocalizedContentManager._IsStringAt(format, "[HOURS_12_0]", index1))
            {
              StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
              string str;
              if (time / 100 % 12 != 0)
              {
                num = time / 100 % 12;
                str = num.ToString();
              }
              else
                str = "0";
              formatStringBuilder.Append(str);
            }
            else if (LocalizedContentManager._IsStringAt(format, "[HOURS_24]", index1))
              LocalizedContentManager._timeFormatStringBuilder.Append(time / 100 % 24);
            else if (LocalizedContentManager._IsStringAt(format, "[HOURS_24_00]", index1))
            {
              StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
              num = time / 100 % 24;
              string str = num.ToString("00");
              formatStringBuilder.Append(str);
            }
            else if (LocalizedContentManager._IsStringAt(format, "[MINUTES]", index1))
            {
              StringBuilder formatStringBuilder = LocalizedContentManager._timeFormatStringBuilder;
              num = time % 100;
              string str = num.ToString("00");
              formatStringBuilder.Append(str);
            }
            else if (LocalizedContentManager._IsStringAt(format, "[AM_PM]", index1))
            {
              if (time < 1200 || time >= 2400)
                LocalizedContentManager._timeFormatStringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370"));
              else
                LocalizedContentManager._timeFormatStringBuilder.Append(Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371"));
            }
            else
            {
              for (int index4 = index1; index4 <= index2; ++index4)
                LocalizedContentManager._timeFormatStringBuilder.Append(format[index4]);
            }
            index1 = -1;
            break;
          }
          goto default;
        default:
          if (index1 < 0)
          {
            LocalizedContentManager._timeFormatStringBuilder.Append(ch);
            break;
          }
          break;
      }
    }
    return LocalizedContentManager._timeFormatStringBuilder;
  }

  public static void SetModLanguage(ModLanguage new_mod_language)
  {
    if (new_mod_language == LocalizedContentManager._currentModLanguage)
      return;
    LocalizedContentManager._currentModLanguage = new_mod_language;
    LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.LanguageCode.mod;
  }

  /// <summary>Get the absolute path to the root content directory from which this manager loads assets.</summary>
  public virtual string GetContentRoot()
  {
    if (this._CachedContentRoot == null)
      this._CachedContentRoot = Path.Combine((string) (typeof (TitleContainer).GetProperty("Location", BindingFlags.Static | BindingFlags.NonPublic) ?? throw new InvalidOperationException("Can't get TitleContainer.Location property from MonoGame")).GetValue((object) null, (object[]) null) ?? throw new InvalidOperationException("Can't get value of TitleContainer.Location property from MonoGame"), this.RootDirectory);
    return this._CachedContentRoot;
  }

  /// <summary>Get whether an asset exists without loading it.</summary>
  /// <typeparam name="T">The expected asset type.</typeparam>
  /// <param name="assetName">The asset name to check.</param>
  public virtual bool DoesAssetExist<T>(string assetName)
  {
    if (assetName == null)
      return false;
    bool flag = false;
    char ch1 = Environment.OSVersion.Platform == PlatformID.Win32NT ? '\\' : '/';
    StringBuilder stringBuilder = new StringBuilder(assetName.Length + 4);
    for (int index = 0; index < assetName.Length; ++index)
    {
      char ch2 = assetName[index];
      switch (ch2)
      {
        case '/':
        case '\\':
          if (!flag)
          {
            ch2 = ch1;
            flag = true;
            break;
          }
          continue;
        default:
          flag = false;
          break;
      }
      stringBuilder.Append(ch2);
    }
    stringBuilder.Append(".xnb");
    string str = stringBuilder.ToString();
    this.EnsureManifestInitialized();
    return LocalizedContentManager._manifest.Contains(str);
  }

  /// <summary>Load an asset through the content pipeline.</summary>
  /// <typeparam name="T">The type of asset to load.</typeparam>
  /// <param name="baseAssetName">The unlocalized asset name relative to the game's root directory.</param>
  /// <param name="localizedAssetName">The localized asset name relative to the game's root directory.</param>
  /// <param name="languageCode">The language for which to load the asset.</param>
  public virtual T LoadImpl<T>(
    string baseAssetName,
    string localizedAssetName,
    LocalizedContentManager.LanguageCode languageCode)
  {
    return this.DoesAssetExist<T>(localizedAssetName) ? base.Load<T>(localizedAssetName) : throw new ContentLoadException($"Could not load {localizedAssetName} asset!");
  }

  /// <summary>Load an asset through the content pipeline.</summary>
  /// <typeparam name="T">The type of asset to load.</typeparam>
  /// <param name="assetName">The unlocalized asset name relative to the game's root directory.</param>
  public override T Load<T>(string assetName)
  {
    return this.Load<T>(assetName, LocalizedContentManager.CurrentLanguageCode);
  }

  /// <summary>Load an asset through the content pipeline.</summary>
  /// <typeparam name="T">The type of asset to load.</typeparam>
  /// <param name="assetName">The unlocalized asset name relative to the game's root directory.</param>
  /// <param name="language">The language for which to load the asset.</param>
  public virtual T Load<T>(string assetName, LocalizedContentManager.LanguageCode language)
  {
    if (language == LocalizedContentManager.LanguageCode.en)
      return this.LoadImpl<T>(assetName, assetName, LocalizedContentManager.LanguageCode.en);
    if (!LocalizedContentManager.localizedAssetNames.TryGetValue(assetName, out string _))
    {
      bool flag1 = false;
      string str1 = $"{assetName}.{(language == LocalizedContentManager.CurrentLanguageCode ? LocalizedContentManager.CurrentLanguageString : LocalizedContentManager.LanguageCodeString(language))}";
      if (!this.DoesAssetExist<T>(str1))
        flag1 = true;
      if (!flag1)
      {
        try
        {
          this.LoadImpl<T>(assetName, str1, language);
          LocalizedContentManager.localizedAssetNames[assetName] = str1;
        }
        catch (ContentLoadException ex)
        {
          flag1 = true;
        }
      }
      if (flag1)
      {
        bool flag2 = false;
        string str2 = assetName + "_international";
        if (!this.DoesAssetExist<T>(str2))
          flag2 = true;
        if (!flag2)
        {
          try
          {
            this.LoadImpl<T>(assetName, str2, language);
            LocalizedContentManager.localizedAssetNames[assetName] = str2;
          }
          catch (ContentLoadException ex)
          {
            flag2 = true;
          }
        }
        if (flag2)
          LocalizedContentManager.localizedAssetNames[assetName] = assetName;
      }
    }
    return this.LoadImpl<T>(assetName, LocalizedContentManager.localizedAssetNames[assetName], language);
  }

  /// <summary>Get the language string which appears in localized asset names for a language (like <c>pt-BR</c>).</summary>
  /// <param name="code">The language whose asset name code to get.</param>
  /// <remarks>For the current language, see <see cref="P:StardewValley.LocalizedContentManager.CurrentLanguageString" /> instead.</remarks>
  public static string LanguageCodeString(LocalizedContentManager.LanguageCode code)
  {
    switch (code)
    {
      case LocalizedContentManager.LanguageCode.ja:
        return "ja-JP";
      case LocalizedContentManager.LanguageCode.ru:
        return "ru-RU";
      case LocalizedContentManager.LanguageCode.zh:
        return "zh-CN";
      case LocalizedContentManager.LanguageCode.pt:
        return "pt-BR";
      case LocalizedContentManager.LanguageCode.es:
        return "es-ES";
      case LocalizedContentManager.LanguageCode.de:
        return "de-DE";
      case LocalizedContentManager.LanguageCode.th:
        return "th-TH";
      case LocalizedContentManager.LanguageCode.fr:
        return "fr-FR";
      case LocalizedContentManager.LanguageCode.ko:
        return "ko-KR";
      case LocalizedContentManager.LanguageCode.it:
        return "it-IT";
      case LocalizedContentManager.LanguageCode.tr:
        return "tr-TR";
      case LocalizedContentManager.LanguageCode.hu:
        return "hu-HU";
      case LocalizedContentManager.LanguageCode.mod:
        return (LocalizedContentManager._currentModLanguage ?? throw new InvalidOperationException("The game language is set to a custom one, but the language info is no longer available.")).LanguageCode;
      default:
        return "";
    }
  }

  /// <summary>Get the current language as an enum.</summary>
  public LocalizedContentManager.LanguageCode GetCurrentLanguage()
  {
    return LocalizedContentManager.CurrentLanguageCode;
  }

  /// <summary>Read a translation key from a loaded strings asset.</summary>
  /// <param name="strings">The loaded strings asset.</param>
  /// <param name="key">The translation key to load.</param>
  /// <returns>Returns the matching string, or <c>null</c> if it wasn't found.</returns>
  private string GetString(Dictionary<string, string> strings, string key)
  {
    if (strings == null)
      return (string) null;
    string str;
    return strings.TryGetValue(key + ".desktop", out str) || strings.TryGetValue(key, out str) ? str : (string) null;
  }

  /// <summary>Get whether a string is a valid translation key which can be loaded by methods like <see cref="M:StardewValley.LocalizedContentManager.LoadString(System.String)" />.</summary>
  /// <param name="path">The potential translation key to check.</param>
  public virtual bool IsValidTranslationKey(string path)
  {
    try
    {
      return this.LoadString(path) != path;
    }
    catch
    {
      return false;
    }
  }

  /// <summary>Get translation text from a data asset, if found.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="localeFallback">Whether to get the English text if the translation isn't defined for the current language.</param>
  /// <returns>Returns the loaded string if found, else <c>null</c>.</returns>
  public virtual string LoadStringReturnNullIfNotFound(string path, bool localeFallback = true)
  {
    string assetName;
    string key;
    this.parseStringPath(path, out assetName, out key);
    return this.PreprocessString(this.GetString(this.Load<Dictionary<string, string>>(assetName), key) ?? (localeFallback ? this.LoadBaseStringOrNull(path) : (string) null));
  }

  /// <summary>Get translation text from a data asset, if found.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="localeFallback">Whether to get the English text if the translation isn't defined for the current language.</param>
  /// <returns>Returns the loaded string if found, else <c>null</c>.</returns>
  public virtual string LoadStringReturnNullIfNotFound(
    string path,
    string sub1,
    bool localeFallback = true)
  {
    string format = this.LoadStringReturnNullIfNotFound(path, localeFallback);
    if (format != null)
      format = string.Format(format, (object) sub1);
    return format;
  }

  /// <summary>Get translation text from a data asset, if found.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="sub2">The value with which to replace the <c>{1}</c> placeholder in the loaded text.</param>
  /// <param name="localeFallback">Whether to get the English text if the translation isn't defined for the current language.</param>
  /// <returns>Returns the loaded string if found, else <c>null</c>.</returns>
  public virtual string LoadStringReturnNullIfNotFound(
    string path,
    string sub1,
    string sub2,
    bool localeFallback = true)
  {
    string format = this.LoadStringReturnNullIfNotFound(path, localeFallback);
    if (format != null)
      format = string.Format(format, (object) sub1, (object) sub2);
    return format;
  }

  /// <summary>Get translation text from a data asset, if found.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="substitutions">The values with which to replace placeholders like <c>{0}</c> in the loaded text.</param>
  /// <param name="localeFallback">Whether to get the English text if the translation isn't defined for the current language.</param>
  /// <returns>Returns the loaded string if found, else <c>null</c>.</returns>
  public virtual string LoadStringReturnNullIfNotFound(
    string path,
    object[] substitutions,
    bool localeFallback = true)
  {
    string format = this.LoadStringReturnNullIfNotFound(path, localeFallback);
    if (format != null)
      format = string.Format(format, substitutions);
    return format;
  }

  /// <summary>Get translation text from a data asset.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <returns>Returns the loaded string if found, else the <paramref name="path" />.</returns>
  public virtual string LoadString(string path)
  {
    return this.LoadStringReturnNullIfNotFound(path) ?? path;
  }

  /// <summary>Apply generic preprocessing to strings loaded from <see cref="M:StardewValley.LocalizedContentManager.LoadString(System.String)" /> and its overloads.</summary>
  /// <param name="text">The text to preprocess.</param>
  public virtual string PreprocessString(string text)
  {
    if (text == null)
      return (string) null;
    Farmer player = Game1.player;
    int gender = player != null ? (int) player.Gender : 0;
    text = Dialogue.applyGenderSwitchBlocks((Gender) gender, text);
    text = Dialogue.applyGenderSwitch((Gender) gender, text, true);
    return text;
  }

  public virtual bool ShouldUseGenderedCharacterTranslations()
  {
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.pt:
        return true;
      case LocalizedContentManager.LanguageCode.mod:
        if (LocalizedContentManager.CurrentModLanguage != null)
          return LocalizedContentManager.CurrentModLanguage.UseGenderedCharacterTranslations;
        break;
    }
    return false;
  }

  /// <summary>Get translation text from a data asset.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <returns>Returns the loaded string if found, else the <paramref name="path" />.</returns>
  public virtual string LoadString(string path, object sub1)
  {
    string format = this.LoadString(path);
    try
    {
      return string.Format(format, sub1);
    }
    catch (Exception ex)
    {
    }
    return format;
  }

  /// <summary>Get translation text from a data asset.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="sub2">The value with which to replace the <c>{1}</c> placeholder in the loaded text.</param>
  /// <returns>Returns the loaded string if found, else the <paramref name="path" />.</returns>
  public virtual string LoadString(string path, object sub1, object sub2)
  {
    string format = this.LoadString(path);
    try
    {
      return string.Format(format, sub1, sub2);
    }
    catch (Exception ex)
    {
    }
    return format;
  }

  /// <summary>Get translation text from a data asset.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="sub2">The value with which to replace the <c>{1}</c> placeholder in the loaded text.</param>
  /// <param name="sub3">The value with which to replace the <c>{2}</c> placeholder in the loaded text.</param>
  /// <returns>Returns the loaded string if found, else the <paramref name="path" />.</returns>
  public virtual string LoadString(string path, object sub1, object sub2, object sub3)
  {
    string format = this.LoadString(path);
    try
    {
      return string.Format(format, sub1, sub2, sub3);
    }
    catch (Exception ex)
    {
    }
    return format;
  }

  /// <summary>Get translation text from a data asset.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="substitutions">The values with which to replace placeholders like <c>{0}</c> in the loaded text.</param>
  /// <returns>Returns the loaded string if found, else the <paramref name="path" />.</returns>
  public virtual string LoadString(string path, params object[] substitutions)
  {
    string format = this.LoadString(path);
    if (substitutions.Length != 0)
    {
      try
      {
        return string.Format(format, substitutions);
      }
      catch (Exception ex)
      {
      }
    }
    return format;
  }

  /// <summary>Get the default English text for a translation, or <c>null</c> if it's not found.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  public virtual string LoadBaseStringOrNull(string path)
  {
    string assetName;
    string key;
    this.parseStringPath(path, out assetName, out key);
    return this.GetString(this.LoadImpl<Dictionary<string, string>>(assetName, assetName, LocalizedContentManager.LanguageCode.en), key);
  }

  /// <summary>Get the default English text for a translation, or the input <paramref name="path" /> if it's not found.</summary>
  /// <param name="path">The translation from which to take the text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  public virtual string LoadBaseString(string path) => this.LoadBaseStringOrNull(path) ?? path;

  private void parseStringPath(string path, out string assetName, out string key)
  {
    int length = path.IndexOf(':');
    assetName = length != -1 ? path.Substring(0, length) : throw new ContentLoadException("Unable to parse string path: " + path);
    key = path.Substring(length + 1, path.Length - length - 1);
  }

  public virtual LocalizedContentManager CreateTemporary()
  {
    return new LocalizedContentManager(this.ServiceProvider, this.RootDirectory, this.CurrentCulture);
  }

  public delegate void LanguageChangedHandler(LocalizedContentManager.LanguageCode code);

  /// <summary>A language supported by the game.</summary>
  public enum LanguageCode
  {
    /// <summary>The English language.</summary>
    en,
    /// <summary>The Japanese language.</summary>
    ja,
    /// <summary>The Russian language.</summary>
    ru,
    /// <summary>The Chinese language.</summary>
    zh,
    /// <summary>The Portuguese language.</summary>
    pt,
    /// <summary>The Spanish language.</summary>
    es,
    /// <summary>The German language.</summary>
    de,
    /// <summary>The Thai language.</summary>
    th,
    /// <summary>The French language.</summary>
    fr,
    /// <summary>The Korean language.</summary>
    ko,
    /// <summary>The Italian language.</summary>
    it,
    /// <summary>The Turkish language.</summary>
    tr,
    /// <summary>The Hungarian language.</summary>
    hu,
    /// <summary>A custom language added by a mod.</summary>
    mod,
  }
}
