// Decompiled with JetBrains decompiler
// Type: StardewValley.Program
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Network.Compress;
using StardewValley.SDKs;
using StardewValley.SDKs.Steam;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#nullable disable
namespace StardewValley;

public static class Program
{
  public const int build_steam = 0;
  public const int build_gog = 1;
  public const int build_rail = 2;
  public const int build_gdk = 3;
  public static bool GameTesterMode = false;
  public static bool releaseBuild = true;
  public static bool enableCheats = !Program.releaseBuild;
  public const int buildType = 0;
  private static SDKHelper _sdk;
  internal static readonly INetCompression defaultCompression = (INetCompression) new LZ4NetCompression();
  internal static INetCompression netCompression = Program.defaultCompression;
  public static Game1 gamePtr;
  public static bool handlingException;
  public static bool hasTriedToPrintLog;
  public static bool successfullyPrintedLog;

  internal static SDKHelper sdk
  {
    get
    {
      if (Program._sdk == null)
      {
        Program._sdk = (SDKHelper) new SteamHelper();
        if (Program._sdk == null)
          Program._sdk = (SDKHelper) new NullSDKHelper();
      }
      return Program._sdk;
    }
  }

  /// <summary>The main entry point for the application.</summary>
  public static void Main(string[] args)
  {
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    Program.GameTesterMode = true;
    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.handleException);
    using (GameRunner gameRunner = new GameRunner())
    {
      GameRunner.instance = gameRunner;
      gameRunner.Run();
    }
  }

  /// <summary>Get the absolute path to the folder containing local app data (like error logs and screenshots), creating it if needed.</summary>
  /// <param name="subfolder">The name of the subfolder to append to the path, if any.</param>
  /// <param name="createIfMissing">Whether to create the folder if it doesn't exist already.</param>
  public static string GetLocalAppDataFolder(string subfolder = null, bool createIfMissing = true)
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
      string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      if (!string.IsNullOrWhiteSpace(folderPath))
      {
        string path = subfolder != null ? Path.Combine(folderPath, "StardewValley", subfolder) : Path.Combine(folderPath, "StardewValley");
        if (createIfMissing)
          Directory.CreateDirectory(path);
        return path;
      }
    }
    return Program.GetAppDataFolder(subfolder, createIfMissing);
  }

  /// <summary>Get the absolute path to the folder containing global app data (like saves), creating it if needed.</summary>
  /// <param name="subfolder">The name of the subfolder to append to the path, if any.</param>
  /// <param name="createIfMissing">Whether to create the folder if it doesn't exist already.</param>
  public static string GetAppDataFolder(string subfolder = null, bool createIfMissing = true)
  {
    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    string path = subfolder != null ? Path.Combine(folderPath, "StardewValley", subfolder) : Path.Combine(folderPath, "StardewValley");
    if (createIfMissing)
      Directory.CreateDirectory(path);
    return path;
  }

  /// <summary>Get the absolute path to the debug log file.</summary>
  public static string GetDebugLogPath()
  {
    return Path.Combine(Program.GetLocalAppDataFolder("ErrorLogs"), "game-latest.txt");
  }

  /// <summary>Get the absolute path to the folder containing save folders, creating it if needed.</summary>
  public static string GetSavesFolder() => Program.GetAppDataFolder("Saves");

  public static string WriteLog(Program.LogType logType, string message, bool append = false)
  {
    string str1 = string.Join("-", string.Join("-", (Game1.player?.Name ?? "NullPlayer").Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.').Split(new char[0], StringSplitOptions.RemoveEmptyEntries));
    string subfolder;
    string path2;
    if (logType == Program.LogType.Disconnect)
    {
      subfolder = "DisconnectLogs";
      string str2 = str1;
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
      interpolatedStringHandler.AppendLiteral("_");
      ref DefaultInterpolatedStringHandler local1 = ref interpolatedStringHandler;
      DateTime now = DateTime.Now;
      int month = now.Month;
      local1.AppendFormatted<int>(month);
      interpolatedStringHandler.AppendLiteral("-");
      ref DefaultInterpolatedStringHandler local2 = ref interpolatedStringHandler;
      now = DateTime.Now;
      int day = now.Day;
      local2.AppendFormatted<int>(day);
      interpolatedStringHandler.AppendLiteral(".txt");
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      path2 = str2 + stringAndClear;
    }
    else
    {
      subfolder = "ErrorLogs";
      path2 = str1 + $"_{Game1.uniqueIDForThisGame}_{(Game1.player == null ? (ulong) Game1.random.Next(999999) : Game1.player.millisecondsPlayed)}.txt";
    }
    string localAppDataFolder = Program.GetLocalAppDataFolder(subfolder);
    if (localAppDataFolder == null)
    {
      Game1.log.Error($"WriteLog failed on GetLocalAppDataFolder(\"{subfolder}\")");
      return (string) null;
    }
    string path = Path.Combine(localAppDataFolder, path2);
    try
    {
      if (append)
        File.AppendAllText(path, message + Environment.NewLine);
      else
        File.WriteAllText(path, message);
    }
    catch (Exception ex)
    {
      Game1.log.Error("WriteLog failed with exception:", ex);
      return (string) null;
    }
    return path;
  }

  public static void AppendDiagnostics(StringBuilder sb)
  {
    sb.AppendLine("Game Version: " + Game1.GetVersionString());
    try
    {
      if (Program.sdk != null)
        sb.AppendLine("SDK Helper: " + Program.sdk.GetType().Name);
      sb.AppendLine("Game Language: " + LocalizedContentManager.CurrentLanguageCode.ToString());
      try
      {
        sb.AppendLine("GPU: " + Game1.graphics.GraphicsDevice.Adapter.Description);
      }
      catch (Exception ex)
      {
        sb.AppendLine("GPU: Could not detect.");
      }
      sb.AppendLine($"OS: {Environment.OSVersion.Platform.ToString()} {Environment.OSVersion.VersionString}");
      if (GameRunner.instance != null && GameRunner.instance.GetType().FullName.StartsWith("StardewModdingAPI."))
        sb.AppendLine("Running SMAPI");
      if (Game1.IsMultiplayer)
      {
        if (LocalMultiplayer.IsLocalMultiplayer())
          sb.AppendLine("Multiplayer (Split Screen)");
        else if (Game1.IsMasterGame)
          sb.AppendLine("Multiplayer (Host)");
        else
          sb.AppendLine("Multiplayer (Client)");
      }
      if (Game1.options.gamepadControls)
        sb.AppendLine("Playing on Controller");
      sb.AppendLine($"In-game Date: {Game1.season.ToString()} {Game1.dayOfMonth.ToString()} Y{Game1.year.ToString()} Time of Day: {Game1.timeOfDay.ToString()}");
      sb.AppendLine("Game Location: " + (Game1.currentLocation == null ? "null" : Game1.currentLocation.NameOrUniqueName));
    }
    catch (Exception ex)
    {
    }
  }

  public static void handleException(object sender, UnhandledExceptionEventArgs args)
  {
    if (Program.handlingException || !Program.GameTesterMode)
      return;
    Game1.gameMode = (byte) 11;
    Program.handlingException = true;
    StringBuilder sb = new StringBuilder();
    if (args != null)
    {
      Exception exceptionObject = (Exception) args.ExceptionObject;
      sb.AppendLine("Message: " + exceptionObject.Message);
      sb.AppendLine("InnerException: " + exceptionObject.InnerException?.ToString());
      sb.AppendLine("Stack Trace: " + exceptionObject.StackTrace);
      sb.AppendLine("");
    }
    Program.AppendDiagnostics(sb);
    Game1.errorMessage = sb.ToString();
    if (!Program.hasTriedToPrintLog)
    {
      Program.hasTriedToPrintLog = true;
      string str = Program.WriteLog(Program.LogType.Error, Game1.errorMessage);
      if (str != null)
      {
        Program.successfullyPrintedLog = true;
        Game1.errorMessage = $"(Error Report created at {str}){Environment.NewLine}{Game1.errorMessage}";
      }
    }
    if (args == null)
      return;
    Game1.gameMode = (byte) 3;
  }

  public enum LogType
  {
    Error,
    Disconnect,
  }
}
