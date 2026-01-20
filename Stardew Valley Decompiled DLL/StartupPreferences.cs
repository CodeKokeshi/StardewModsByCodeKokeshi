// Decompiled with JetBrains decompiler
// Type: StardewValley.StartupPreferences
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.GameData;
using StardewValley.SaveSerialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

#nullable disable
namespace StardewValley;

public class StartupPreferences
{
  public const int windowed_borderless = 0;
  public const int windowed = 1;
  public const int fullscreen = 2;
  private static readonly string _filename = "startup_preferences";
  public static XmlSerializer serializer = (XmlSerializer) null;
  public bool startMuted;
  public bool levelTenFishing;
  public bool levelTenMining;
  public bool levelTenForaging;
  public bool levelTenCombat;
  public bool skipWindowPreparation;
  public bool sawAdvancedCharacterCreationIndicator;
  public int timesPlayed;
  public int windowMode;
  public int displayIndex = -1;
  public Options.GamepadModes gamepadMode;
  public int playerLimit = -1;
  public int fullscreenResolutionX;
  public int fullscreenResolutionY;
  public string lastEnteredIP = "";
  public string languageCode;
  public Options clientOptions = new Options();
  [XmlIgnore]
  public bool isLoaded;
  private bool _isBusy;
  private bool _pendingApplyLanguage;
  private Task _task;

  [XmlIgnore]
  public bool IsBusy
  {
    get
    {
      lock (this)
      {
        if (!this._isBusy)
          return false;
        if (this._task == null)
          throw new Exception("StartupPreferences.IsBusy; was busy but task is null?");
        if (this._task.IsFaulted)
        {
          Exception baseException = this._task.Exception.GetBaseException();
          Game1.log.Error("StartupPreferences._task failed with an exception.", baseException);
          throw baseException;
        }
        if (this._task.IsCompleted)
        {
          this._task = (Task) null;
          this._isBusy = false;
          if (this._pendingApplyLanguage)
            this._SetLanguageFromCode(this.languageCode);
        }
        return this._isBusy;
      }
    }
  }

  private void Init()
  {
    this.isLoaded = false;
    this.ensureFolderStructureExists();
  }

  public void OnLanguageChange(LocalizedContentManager.LanguageCode code)
  {
    string id = code.ToString();
    if (code == LocalizedContentManager.LanguageCode.mod && LocalizedContentManager.CurrentModLanguage != null)
      id = LocalizedContentManager.CurrentModLanguage.Id;
    if (!this.isLoaded || !(this.languageCode != id))
      return;
    this.savePreferences(false, true);
  }

  private void ensureFolderStructureExists() => Program.GetAppDataFolder();

  public void savePreferences(bool async, bool update_language_from_ingame_language = false)
  {
    lock (this)
    {
      if (update_language_from_ingame_language)
        this.languageCode = LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.mod ? LocalizedContentManager.CurrentLanguageCode.ToString() : LocalizedContentManager.CurrentModLanguage.Id;
      try
      {
        this._savePreferences();
      }
      catch (Exception ex)
      {
        Game1.log.Error("StartupPreferences._task failed with an exception.", ex);
        throw ex;
      }
    }
  }

  private void _savePreferences()
  {
    string path = Path.Combine(Program.GetAppDataFolder(), StartupPreferences._filename);
    try
    {
      this.ensureFolderStructureExists();
      if (File.Exists(path))
        File.Delete(path);
      using (FileStream fileStream = File.Create(path))
        this.writeSettings((Stream) fileStream);
    }
    catch (Exception ex)
    {
      Game1.debugOutput = Game1.parseText(ex.Message);
    }
  }

  private long writeSettings(Stream stream)
  {
    XmlWriterSettings settings = new XmlWriterSettings()
    {
      CloseOutput = true,
      Indent = true
    };
    using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
    {
      xmlWriter.WriteStartDocument();
      StartupPreferences.serializer.SerializeFast(xmlWriter, (object) this);
      xmlWriter.WriteEndDocument();
      xmlWriter.Flush();
      return stream.Length;
    }
  }

  public void loadPreferences(bool async, bool applyLanguage)
  {
    lock (this)
    {
      this._pendingApplyLanguage = applyLanguage;
      this.Init();
      try
      {
        this._loadPreferences();
      }
      catch (Exception ex)
      {
        Exception exception = this._task.Exception?.GetBaseException() ?? ex;
        Game1.log.Error("StartupPreferences._task failed with an exception.", exception);
        throw exception;
      }
      if (!applyLanguage)
        return;
      this._SetLanguageFromCode(this.languageCode);
    }
  }

  protected virtual void _SetLanguageFromCode(string language_code_string)
  {
    List<ModLanguage> modLanguageList = DataLoader.AdditionalLanguages(Game1.content);
    bool flag = false;
    if (modLanguageList != null)
    {
      foreach (ModLanguage new_mod_language in modLanguageList)
      {
        if (new_mod_language.Id == language_code_string)
        {
          LocalizedContentManager.SetModLanguage(new_mod_language);
          flag = true;
          break;
        }
      }
    }
    if (flag)
      return;
    LocalizedContentManager.LanguageCode parsed;
    if (Utility.TryParseEnum<LocalizedContentManager.LanguageCode>(language_code_string, out parsed) && parsed != LocalizedContentManager.LanguageCode.mod)
      LocalizedContentManager.CurrentLanguageCode = parsed;
    else
      LocalizedContentManager.CurrentLanguageCode = LocalizedContentManager.GetDefaultLanguageCode();
  }

  private void _loadPreferences()
  {
    string path = Path.Combine(Program.GetAppDataFolder(), StartupPreferences._filename);
    if (!File.Exists(path))
    {
      Game1.log.Verbose($"path '{path}' did not exist and will be created");
      try
      {
        this.languageCode = LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.mod ? LocalizedContentManager.CurrentLanguageCode.ToString() : LocalizedContentManager.CurrentModLanguage.Id;
        using (FileStream fileStream = File.Create(path))
          this.writeSettings((Stream) fileStream);
      }
      catch (Exception ex)
      {
        Game1.log.Error("_loadPreferences; exception occurred trying to create/write.", ex);
        Game1.debugOutput = Game1.parseText(ex.Message);
        return;
      }
    }
    try
    {
      using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read))
        this.readSettings((Stream) fileStream);
      this.isLoaded = true;
    }
    catch (Exception ex)
    {
      Game1.log.Error("_loadPreferences; exception occurred trying open/read.", ex);
      Game1.debugOutput = Game1.parseText(ex.Message);
    }
  }

  private void readSettings(Stream stream)
  {
    StartupPreferences startupPreferences = (StartupPreferences) StartupPreferences.serializer.DeserializeFast(stream);
    this.startMuted = startupPreferences.startMuted;
    this.timesPlayed = startupPreferences.timesPlayed + 1;
    this.levelTenCombat = startupPreferences.levelTenCombat;
    this.levelTenFishing = startupPreferences.levelTenFishing;
    this.levelTenForaging = startupPreferences.levelTenForaging;
    this.levelTenMining = startupPreferences.levelTenMining;
    this.skipWindowPreparation = startupPreferences.skipWindowPreparation;
    this.windowMode = startupPreferences.windowMode;
    this.displayIndex = startupPreferences.displayIndex;
    this.playerLimit = startupPreferences.playerLimit;
    this.gamepadMode = startupPreferences.gamepadMode;
    this.fullscreenResolutionX = startupPreferences.fullscreenResolutionX;
    this.fullscreenResolutionY = startupPreferences.fullscreenResolutionY;
    this.lastEnteredIP = startupPreferences.lastEnteredIP;
    this.languageCode = startupPreferences.languageCode;
    this.clientOptions = startupPreferences.clientOptions;
  }
}
