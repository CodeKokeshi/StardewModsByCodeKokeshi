// Decompiled with JetBrains decompiler
// Type: StardewValley.Dialogue
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;
using StardewValley.TokenizableStrings;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace StardewValley;

public class Dialogue
{
  public const string dialogueHappy = "$h";
  public const string dialogueSad = "$s";
  public const string dialogueUnique = "$u";
  public const string dialogueNeutral = "$neutral";
  public const string dialogueLove = "$l";
  public const string dialogueAngry = "$a";
  public const string dialogueEnd = "$e";
  /// <summary>The character which begins a command name.</summary>
  public const char dialogueCommandPrefix = '$';
  /// <summary>A dialogue code which splits the subsequent text into a separate dialogue box shown after the player clicks.</summary>
  public const string dialogueBreak = "$b";
  /// <summary>Equivalent to <see cref="F:StardewValley.Dialogue.dialogueBreak" />, but wrapped with command delimiters so it can be added directly to dialogue text.</summary>
  public const string dialogueBreakDelimited = "#$b#";
  public const string multipleDialogueDelineator = "||";
  public const string dialogueKill = "$k";
  public const string dialogueChance = "$c";
  public const string dialogueDependingOnWorldState = "$d";
  public const string dialogueEvent = "$v";
  public const string dialogueQuickResponse = "$y";
  public const string dialoguePrerequisite = "$p";
  public const string dialogueSingle = "$1";
  /// <summary>A command which toggles between two dialogues depending on the result of a game state query.</summary>
  public const string dialogueGameStateQuery = "$query";
  /// <summary>A command which switches between gendered text based on the player gender.</summary>
  public const string dialogueGenderSwitch_startBlock = "${";
  /// <summary>The end token for a <see cref="F:StardewValley.Dialogue.dialogueGenderSwitch_startBlock" /> command.</summary>
  public const string dialogueGenderSwitch_endBlock = "}$";
  /// <summary>A command which runs an action.</summary>
  public const string dialogueRunAction = "$action";
  /// <summary>A command which begins a conversation topic.</summary>
  public const string dialogueStartConversationTopic = "$t";
  /// <summary>A command which begins a question.</summary>
  public const string dialogueQuestion = "$q";
  /// <summary>A command which starts an inquiry initiated by the player or an answer to an NPC's question.</summary>
  public const string dialogueResponse = "$r";
  /// <summary>A special character added to dialogues to signify that they are part of a broken up series of dialogues.</summary>
  public const string breakSpecialCharacter = "{";
  public const string playerNameSpecialCharacter = "@";
  public const char genderDialogueSplitCharacter = '^';
  public const char genderDialogueSplitCharacter2 = '¦';
  public const string quickResponseDelineator = "*";
  public const string randomAdjectiveSpecialCharacter = "%adj";
  public const string randomNounSpecialCharacter = "%noun";
  public const string randomPlaceSpecialCharacter = "%place";
  public const string spouseSpecialCharacter = "%spouse";
  public const string randomNameSpecialCharacter = "%name";
  public const string firstNameLettersSpecialCharacter = "%firstnameletter";
  public const string timeSpecialCharacter = "%time";
  public const string bandNameSpecialCharacter = "%band";
  public const string bookNameSpecialCharacter = "%book";
  public const string petSpecialCharacter = "%pet";
  public const string farmNameSpecialCharacter = "%farm";
  public const string favoriteThingSpecialCharacter = "%favorite";
  public const string eventForkSpecialCharacter = "%fork";
  public const string yearSpecialCharacter = "%year";
  public const string kid1specialCharacter = "%kid1";
  public const string kid2SpecialCharacter = "%kid2";
  public const string revealTasteCharacter = "%revealtaste";
  public const string seasonCharacter = "%season";
  public const string dontfacefarmer = "%noturn";
  /// <summary>A prefix added to a dialogue line to indicate it should be drawn as a small dialogue box with no portrait.</summary>
  /// <remarks>This is only applied if it's not part of another token like <c>%year</c>.</remarks>
  public const char noPortraitPrefix = '%';
  /// <summary>The translation key for <see cref="M:StardewValley.Dialogue.GetFallbackForError(StardewValley.NPC)" />.</summary>
  public const string FallbackDialogueForErrorKey = "Strings\\Characters:FallbackDialogueForError";
  /// <summary>The tokens like <see cref="F:StardewValley.Dialogue.spouseSpecialCharacter" /> which begin with a <c>%</c> symbol.</summary>
  public static readonly string[] percentTokens = new string[18]
  {
    "%adj",
    "%noun",
    "%place",
    "%spouse",
    "%name",
    "%firstnameletter",
    "%time",
    "%band",
    "%book",
    "%pet",
    "%farm",
    "%favorite",
    "%fork",
    "%year",
    "%kid1",
    "%kid2",
    "%revealtaste",
    "%season"
  };
  private static bool nameArraysTranslated = false;
  public static string[] adjectives = new string[20]
  {
    "Purple",
    "Gooey",
    "Chalky",
    "Green",
    "Plush",
    "Chunky",
    "Gigantic",
    "Greasy",
    "Gloomy",
    "Practical",
    "Lanky",
    "Dopey",
    "Crusty",
    "Fantastic",
    "Rubbery",
    "Silly",
    "Courageous",
    "Reasonable",
    "Lonely",
    "Bitter"
  };
  public static string[] nouns = new string[23]
  {
    "Dragon",
    "Buffet",
    "Biscuit",
    "Robot",
    "Planet",
    "Pepper",
    "Tomb",
    "Hyena",
    "Lip",
    "Quail",
    "Cheese",
    "Disaster",
    "Raincoat",
    "Shoe",
    "Castle",
    "Elf",
    "Pump",
    "Chip",
    "Wig",
    "Mermaid",
    "Drumstick",
    "Puppet",
    "Submarine"
  };
  public static string[] verbs = new string[13]
  {
    "ran",
    "danced",
    "spoke",
    "galloped",
    "ate",
    "floated",
    "stood",
    "flowed",
    "smelled",
    "swam",
    "grilled",
    "cracked",
    "melted"
  };
  public static string[] positional = new string[13]
  {
    "atop",
    "near",
    "with",
    "alongside",
    "away from",
    "too close to",
    "dangerously close to",
    "far, far away from",
    "uncomfortably close to",
    "way above the",
    "miles below",
    "on a different planet from",
    "in a different century than"
  };
  public static string[] places = new string[12]
  {
    "Castle Village",
    "Basket Town",
    "Pine Mesa City",
    "Point Drake",
    "Minister Valley",
    "Grampleton",
    "Zuzu City",
    "a small island off the coast",
    "Fort Josa",
    "Chestervale",
    "Fern Islands",
    "Tanker Grove"
  };
  public static string[] colors = new string[16 /*0x10*/]
  {
    "/crimson",
    "/green",
    "/tan",
    "/purple",
    "/deep blue",
    "/neon pink",
    "/pale/yellow",
    "/chocolate/brown",
    "/sky/blue",
    "/bubblegum/pink",
    "/blood/red",
    "/bright/orange",
    "/aquamarine",
    "/silvery",
    "/glimmering/gold",
    "/rainbow"
  };
  /// <summary>The dialogues to show in their own message boxes, and/or actions to perform when selected.</summary>
  public List<DialogueLine> dialogues = new List<DialogueLine>();
  /// <summary>The <see cref="F:StardewValley.Dialogue.currentDialogueIndex" /> values for which to disable the portrait due to <see cref="F:StardewValley.Dialogue.noPortraitPrefix" />.</summary>
  public HashSet<int> indexesWithoutPortrait = new HashSet<int>();
  /// <summary>The responses which the player can choose from, if any.</summary>
  private List<NPCDialogueResponse> playerResponses;
  private List<string> quickResponses;
  private bool isLastDialogueInteractive;
  private bool quickResponse;
  public bool isCurrentStringContinuedOnNextScreen;
  private bool finishedLastDialogue;
  public bool showPortrait;
  public bool removeOnNextMove;
  public bool dontFaceFarmer;
  public string temporaryDialogueKey;
  public int currentDialogueIndex;
  /// <summary>The backing field for <see cref="P:StardewValley.Dialogue.CurrentEmotion" />.</summary>
  /// <remarks>Most code shouldn't use this directly.</remarks>
  private string currentEmotion;
  public NPC speaker;
  public Dialogue.onAnswerQuestion answerQuestionBehavior;
  public Texture2D overridePortrait;
  public Action onFinish;
  /// <summary>The translation key from which the <see cref="F:StardewValley.Dialogue.dialogues" /> were taken, if known, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>. This is informational only, and has no effect on the dialogue text. The displayed text may not match the translation text exactly (e.g. due to token substitutions or dialogue parsing).</summary>
  public readonly string TranslationKey;

  private static void TranslateArraysOfStrings()
  {
    Dialogue.colors = new string[16 /*0x10*/]
    {
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.795"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.796"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.797"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.798"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.799"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.800"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.801"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.802"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.803"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.804"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.805"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.806"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.807"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.808"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.809"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.810")
    };
    Dialogue.adjectives = new string[20]
    {
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.679"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.680"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.681"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.682"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.683"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.684"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.685"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.686"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.687"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.688"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.689"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.690"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.691"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.692"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.693"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.694"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.695"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.696"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.697"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.698")
    };
    Dialogue.nouns = new string[23]
    {
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.699"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.700"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.701"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.702"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.703"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.704"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.705"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.706"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.707"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.708"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.709"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.710"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.711"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.712"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.713"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.714"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.715"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.716"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.717"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.718"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.719"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.720"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.721")
    };
    Dialogue.verbs = new string[13]
    {
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.722"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.723"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.724"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.725"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.726"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.727"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.728"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.729"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.730"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.731"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.732"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.733"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.734")
    };
    Dialogue.positional = new string[13]
    {
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.735"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.736"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.737"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.738"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.739"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.740"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.741"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.742"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.743"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.744"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.745"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.746"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.747")
    };
    Dialogue.places = new string[12]
    {
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.748"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.749"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.750"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.751"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.752"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.753"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.754"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.755"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.756"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.757"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.758"),
      Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.759")
    };
    Dialogue.nameArraysTranslated = true;
  }

  /// <summary>The portrait command for the current dialogue, usually matching a constant like <see cref="F:StardewValley.Dialogue.dialogueHappy" /> or a numeric index like <c>$1</c>.</summary>
  public string CurrentEmotion
  {
    get => this.currentEmotion ?? "$neutral";
    set => this.currentEmotion = value;
  }

  /// <summary>Whether the <see cref="P:StardewValley.Dialogue.CurrentEmotion" /> was set explicitly (e.g. via a dialogue command like <see cref="F:StardewValley.Dialogue.dialogueNeutral" />), instead of being the default value.</summary>
  public bool CurrentEmotionSetExplicitly => this.currentEmotion != null;

  public Farmer farmer => Game1.CurrentEvent != null ? Game1.CurrentEvent.farmer : Game1.player;

  /// <summary>Construct an instance.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which the <paramref name="dialogueText" /> was taken, if known, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>. This is informational only, and has no effect on the dialogue text.</param>
  /// <param name="dialogueText">The literal dialogue text to display.</param>
  /// <remarks>This constructor allows setting literal text. To use a translation as-is, see the other constructor.</remarks>
  public Dialogue(NPC speaker, string translationKey, string dialogueText)
  {
    if (!Dialogue.nameArraysTranslated)
      Dialogue.TranslateArraysOfStrings();
    this.speaker = speaker;
    this.TranslationKey = translationKey;
    try
    {
      this.parseDialogueString(dialogueText, translationKey);
      this.checkForSpecialDialogueAttributes();
    }
    catch (Exception ex)
    {
      Game1.log.Error($"Failed parsing dialogue string for NPC {speaker?.Name} (key: {translationKey}, text: {dialogueText}).", ex);
      this.parseDialogueString(Dialogue.GetFallbackTextForError(), "Strings\\Characters:FallbackDialogueForError");
      this.checkForSpecialDialogueAttributes();
    }
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="isGendered">Whether the <paramref name="translationKey" /> matches a gendered translation.</param>
  /// <remarks>This matches the most common convention, i.e. a translation with no format placeholders. For more advanced cases, see <c>FromTranslation</c> or the constructor which takes a <c>dialogueText</c> parameter.</remarks>
  public Dialogue(NPC speaker, string translationKey, bool isGendered = false)
    : this(speaker, translationKey, isGendered ? Game1.LoadStringByGender(speaker.Gender, translationKey) : Game1.content.LoadString(translationKey))
  {
  }

  /// <summary>Construct an instance.</summary>
  /// <param name="other">The data to copy.</param>
  public Dialogue(Dialogue other)
  {
    foreach (DialogueLine dialogue in other.dialogues)
      this.dialogues.Add(new DialogueLine(dialogue.Text, dialogue.SideEffects));
    this.indexesWithoutPortrait = new HashSet<int>((IEnumerable<int>) other.indexesWithoutPortrait);
    if (other.playerResponses != null)
    {
      this.playerResponses = new List<NPCDialogueResponse>();
      foreach (NPCDialogueResponse playerResponse in other.playerResponses)
        this.playerResponses.Add(new NPCDialogueResponse(playerResponse));
    }
    if (other.quickResponses != null)
      this.quickResponses = new List<string>((IEnumerable<string>) other.quickResponses);
    this.isLastDialogueInteractive = other.isLastDialogueInteractive;
    this.quickResponse = other.quickResponse;
    this.isCurrentStringContinuedOnNextScreen = other.isCurrentStringContinuedOnNextScreen;
    this.finishedLastDialogue = other.finishedLastDialogue;
    this.showPortrait = other.showPortrait;
    this.removeOnNextMove = other.removeOnNextMove;
    this.dontFaceFarmer = other.dontFaceFarmer;
    this.temporaryDialogueKey = other.temporaryDialogueKey;
    this.currentDialogueIndex = other.currentDialogueIndex;
    this.currentEmotion = other.currentEmotion;
    this.speaker = other.speaker;
    this.answerQuestionBehavior = other.answerQuestionBehavior;
    this.overridePortrait = other.overridePortrait;
    this.onFinish = other.onFinish;
    this.TranslationKey = other.TranslationKey;
  }

  /// <summary>Get a dialogue instance if the given translation key exists.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  public static Dialogue TryGetDialogue(NPC speaker, string translationKey)
  {
    string dialogueText = Game1.content.LoadStringReturnNullIfNotFound(translationKey);
    return dialogueText == null ? (Dialogue) null : new Dialogue(speaker, translationKey, dialogueText);
  }

  /// <summary>Get a dialogue instance for a translation key.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  public static Dialogue FromTranslation(NPC speaker, string translationKey)
  {
    return new Dialogue(speaker, translationKey);
  }

  /// <summary>Get a dialogue instance for a translation key.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  public static Dialogue FromTranslation(NPC speaker, string translationKey, object sub1)
  {
    return new Dialogue(speaker, translationKey, Game1.content.LoadString(translationKey, sub1));
  }

  /// <summary>Get a dialogue instance for a translation key.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="sub2">The value with which to replace the <c>{1}</c> placeholder in the loaded text.</param>
  public static Dialogue FromTranslation(
    NPC speaker,
    string translationKey,
    object sub1,
    object sub2)
  {
    return new Dialogue(speaker, translationKey, Game1.content.LoadString(translationKey, sub1, sub2));
  }

  /// <summary>Get a dialogue instance for a translation key.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="sub1">The value with which to replace the <c>{0}</c> placeholder in the loaded text.</param>
  /// <param name="sub2">The value with which to replace the <c>{1}</c> placeholder in the loaded text.</param>
  /// <param name="sub3">The value with which to replace the <c>{2}</c> placeholder in the loaded text.</param>
  public static Dialogue FromTranslation(
    NPC speaker,
    string translationKey,
    object sub1,
    object sub2,
    object sub3)
  {
    return new Dialogue(speaker, translationKey, Game1.content.LoadString(translationKey, sub1, sub2, sub3));
  }

  /// <summary>Get a dialogue instance for a translation key.</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  /// <param name="translationKey">The translation from which to take the dialogue text, in the form <c>assetName:fieldKey</c> like <c>Strings/UI:Confirm</c>.</param>
  /// <param name="substitutions">The values with which to replace placeholders like <c>{0}</c> in the loaded text.</param>
  public static Dialogue FromTranslation(
    NPC speaker,
    string translationKey,
    params object[] substitutions)
  {
    return new Dialogue(speaker, translationKey, Game1.content.LoadString(translationKey, substitutions));
  }

  /// <summary>Get a fallback dialogue to show when an error happens and a suitable dialogue can't be loaded. This is usually some variation of "...".</summary>
  /// <param name="speaker">The NPC saying the line.</param>
  public static Dialogue GetFallbackForError(NPC speaker)
  {
    return Dialogue.TryGetDialogue(speaker, "Strings\\Characters:FallbackDialogueForError") ?? new Dialogue(speaker, "Strings\\Characters:FallbackDialogueForError", "...");
  }

  /// <summary>Get a fallback dialogue text to show when an error happens and a suitable dialogue can't be loaded. This is usually some variation of "...".</summary>
  /// <remarks>Most code should use <see cref="M:StardewValley.Dialogue.GetFallbackForError(StardewValley.NPC)" /> instead.</remarks>
  public static string GetFallbackTextForError()
  {
    return Game1.content.LoadStringReturnNullIfNotFound("Strings\\Characters:FallbackDialogueForError") ?? "...";
  }

  public static string getRandomVerb()
  {
    if (!Dialogue.nameArraysTranslated)
      Dialogue.TranslateArraysOfStrings();
    return Game1.random.Choose<string>(Dialogue.verbs);
  }

  public static string getRandomAdjective()
  {
    if (!Dialogue.nameArraysTranslated)
      Dialogue.TranslateArraysOfStrings();
    return Game1.random.Choose<string>(Dialogue.adjectives);
  }

  public static string getRandomNoun()
  {
    if (!Dialogue.nameArraysTranslated)
      Dialogue.TranslateArraysOfStrings();
    return Game1.random.Choose<string>(Dialogue.nouns);
  }

  public static string getRandomPositional()
  {
    if (!Dialogue.nameArraysTranslated)
      Dialogue.TranslateArraysOfStrings();
    return Game1.random.Choose<string>(Dialogue.positional);
  }

  public int getPortraitIndex()
  {
    if (this.speaker != null && Game1.isGreenRain && this.speaker.Name.Equals("Demetrius") && Game1.year == 1)
      return 7;
    switch (this.CurrentEmotion)
    {
      case "$neutral":
        return 0;
      case "$h":
        return 1;
      case "$s":
        return 2;
      case "$u":
        return 3;
      case "$l":
        return 4;
      case "$a":
        return 5;
      default:
        int result;
        return !int.TryParse(this.CurrentEmotion.Substring(1), out result) ? 0 : result;
    }
  }

  /// <summary>Parse raw dialogue text.</summary>
  /// <param name="masterString">The raw dialogue text to parse.</param>
  /// <param name="translationKey">The translation key from which the dialogue was loaded, if known.</param>
  protected virtual void parseDialogueString(string masterString, string translationKey)
  {
    masterString = TokenParser.ParseText(masterString ?? "...");
    string[] strArray1 = masterString.Split("||");
    if (strArray1.Length > 1)
      masterString = strArray1[(long) (Game1.stats.DaysPlayed / 7U) % (long) strArray1.Length];
    this.playerResponses?.Clear();
    string[] source = masterString.Split('#');
    for (int count = 0; count < source.Length; ++count)
    {
      string str1 = source[count];
      if (str1.Length >= 2)
      {
        string str2;
        source[count] = str2 = this.checkForSpecialCharacters(str1);
        bool flag1 = false;
        if (str2.StartsWith('$'))
        {
          string[] array1 = ArgUtility.SplitBySpace(str2, 2);
          string str3 = array1[0];
          string commandArgs = ArgUtility.Get(array1, 1);
          flag1 = true;
          if (str3 != null)
          {
            switch (str3.Length)
            {
              case 2:
                switch (str3[1])
                {
                  case '1':
                    if (str3 == "$1")
                    {
                      string str4 = ArgUtility.SplitBySpaceAndGet(commandArgs, 0);
                      if (str4 != null)
                      {
                        if (this.farmer.mailReceived.Contains(str4))
                        {
                          count += 3;
                          if (count < source.Length)
                          {
                            source[count] = this.checkForSpecialCharacters(source[count]);
                            this.dialogues.Add(new DialogueLine(source[count]));
                            goto label_76;
                          }
                          goto label_76;
                        }
                        source[count + 1] = this.checkForSpecialCharacters(source[count + 1]);
                        this.dialogues.Add(new DialogueLine($"{str4}}}{source[count + 1]}"));
                        count = 99999;
                        goto label_76;
                      }
                      break;
                    }
                    break;
                  case 'b':
                    if (str3 == "$b")
                    {
                      if (this.dialogues.Count > 0)
                      {
                        this.dialogues[this.dialogues.Count - 1].Text += "{";
                        goto label_76;
                      }
                      goto label_76;
                    }
                    break;
                  case 'c':
                    if (str3 == "$c")
                    {
                      string str5 = ArgUtility.SplitBySpaceAndGet(commandArgs, 0);
                      if (str5 != null)
                      {
                        double chance = Convert.ToDouble(str5);
                        if (!Game1.random.NextBool(chance))
                        {
                          ++count;
                          goto label_76;
                        }
                        this.dialogues.Add(new DialogueLine(source[count + 1]));
                        count += 3;
                        goto label_76;
                      }
                      break;
                    }
                    break;
                  case 'd':
                    if (str3 == "$d")
                    {
                      string[] strArray2 = ArgUtility.SplitBySpace(commandArgs);
                      string str6 = masterString.Substring(masterString.IndexOf('#') + 1);
                      bool flag2 = false;
                      switch (strArray2[0].ToLower())
                      {
                        case "joja":
                          flag2 = Game1.isLocationAccessible("JojaMart");
                          break;
                        case "cc":
                        case "communitycenter":
                          flag2 = Game1.isLocationAccessible("CommunityCenter");
                          break;
                        case "bus":
                          flag2 = Game1.MasterPlayer.mailReceived.Contains("ccVault");
                          break;
                        case "kent":
                          flag2 = Game1.year >= 2;
                          break;
                      }
                      char separator = str6.Contains('|') ? '|' : '#';
                      source = !flag2 ? str6.Split(separator)[1].Split('#') : str6.Split(separator)[0].Split('#');
                      --count;
                      goto label_76;
                    }
                    break;
                  case 'e':
                    if (str3 == "$e")
                      goto label_76;
                    break;
                  case 'k':
                    if (str3 == "$k")
                      goto label_76;
                    break;
                  case 'p':
                    if (str3 == "$p")
                    {
                      string[] strArray3 = ArgUtility.SplitBySpace(commandArgs);
                      string[] strArray4 = source[count + 1].Split('|');
                      bool flag3 = false;
                      for (int index = 0; index < strArray3.Length; ++index)
                      {
                        if (this.farmer.DialogueQuestionsAnswered.Contains(strArray3[index]))
                        {
                          flag3 = true;
                          break;
                        }
                      }
                      if (flag3)
                      {
                        source = strArray4[0].Split('#');
                        count = -1;
                        goto label_76;
                      }
                      source[count + 1] = ((IEnumerable<string>) source[count + 1].Split('|')).Last<string>();
                      goto label_76;
                    }
                    break;
                  case 'q':
                    if (str3 == "$q")
                    {
                      if (this.dialogues.Count > 0)
                        this.dialogues[this.dialogues.Count - 1].Text += "{";
                      string[] strArray5 = ArgUtility.SplitBySpace(commandArgs);
                      string[] strArray6 = strArray5[0].Split('/');
                      bool flag4 = false;
                      for (int index = 0; index < strArray6.Length; ++index)
                      {
                        if (this.farmer.DialogueQuestionsAnswered.Contains(strArray6[index]))
                        {
                          flag4 = true;
                          break;
                        }
                      }
                      if (flag4 && strArray6[0] != "-1")
                      {
                        if (!strArray5[1].Equals("null"))
                        {
                          source = ((IEnumerable<string>) source).Take<string>(count).Concat<string>((IEnumerable<string>) this.speaker.Dialogue[strArray5[1]].Split('#')).ToArray<string>();
                          --count;
                          goto label_76;
                        }
                        goto label_76;
                      }
                      this.isLastDialogueInteractive = true;
                      goto label_76;
                    }
                    break;
                  case 'r':
                    if (str3 == "$r")
                    {
                      string[] strArray7 = ArgUtility.SplitBySpace(commandArgs);
                      if (this.playerResponses == null)
                        this.playerResponses = new List<NPCDialogueResponse>();
                      this.isLastDialogueInteractive = true;
                      this.playerResponses.Add(new NPCDialogueResponse(strArray7[0], Convert.ToInt32(strArray7[1]), strArray7[2], source[count + 1]));
                      ++count;
                      goto label_76;
                    }
                    break;
                  case 't':
                    if (str3 == "$t")
                    {
                      this.dialogues.Add(new DialogueLine("", (Action) (() =>
                      {
                        string[] array2 = ArgUtility.SplitBySpace(commandArgs);
                        string key;
                        string error;
                        int num;
                        if (!ArgUtility.TryGet(array2, 0, out key, out error, false, "string topicId") || !ArgUtility.TryGetOptionalInt(array2, 1, out num, out error, 4, "int daysDuration"))
                          Game1.log.Warn($"Failed to parse {"$t"} token for {translationKey ?? this.speaker?.Name ?? $"\"{masterString}\""}: {error}.");
                        else
                          Game1.player.activeDialogueEvents.TryAdd(key, num);
                      })));
                      goto label_76;
                    }
                    break;
                  case 'y':
                    if (str3 == "$y")
                    {
                      this.quickResponse = true;
                      this.isLastDialogueInteractive = true;
                      if (this.quickResponses == null)
                        this.quickResponses = new List<string>();
                      if (this.playerResponses == null)
                        this.playerResponses = new List<NPCDialogueResponse>();
                      string str7 = str2.Substring(str2.IndexOf('\'') + 1);
                      string[] strArray8 = str7.Substring(0, str7.Length - 1).Split('_');
                      this.dialogues.Add(new DialogueLine(strArray8[0]));
                      for (int index = 1; index < strArray8.Length; index += 2)
                      {
                        string text = strArray8[index];
                        string str8 = strArray8[index + 1];
                        if (str8.Contains("*"))
                          str8 = str8.Replace("**", "<<<<asterisk>>>>").Replace("*", "#$b#").Replace("<<<<asterisk>>>>", "*");
                        this.playerResponses.Add(new NPCDialogueResponse((string) null, -1, "quickResponse" + index.ToString(), Game1.parseText(text)));
                        this.quickResponses.Add(str8);
                      }
                      goto label_76;
                    }
                    break;
                }
                break;
              case 6:
                if (str3 == "$query")
                {
                  string queryString = commandArgs;
                  string[] array3 = ArgUtility.Get(masterString.Split('#', 2), 1)?.Split('|') ?? LegacyShims.EmptyArray<string>();
                  source = GameStateQuery.CheckConditions(queryString) ? array3[0].Split('#') : ArgUtility.Get(array3, 1, array3[0]).Split('#');
                  --count;
                  goto label_76;
                }
                break;
              case 7:
                if (str3 == "$action")
                {
                  this.dialogues.Add(new DialogueLine("", (Action) (() =>
                  {
                    string error;
                    Exception exception;
                    if (TriggerActionManager.TryRunAction(commandArgs, out error, out exception))
                      return;
                    string str9 = $"Failed to parse {"$action"} token for {translationKey ?? this.speaker?.Name ?? $"\"{masterString}\""}: {error}.";
                    if (exception == null)
                      Game1.log.Warn(str9);
                    else
                      Game1.log.Error(str9, exception);
                  })));
                  goto label_76;
                }
                break;
            }
          }
          flag1 = false;
        }
label_76:
        if (!flag1)
          this.dialogues.Add(new DialogueLine(this.applyGenderSwitch(str2)));
      }
    }
  }

  public virtual void prepareDialogueForDisplay()
  {
    Friendship friendship;
    if (this.dialogues.Count <= 0 || this.speaker == null || !this.speaker.shouldWearIslandAttire.Value || !Game1.player.friendshipData.TryGetValue(this.speaker.Name, out friendship) || !friendship.IsDivorced() || !(this.CurrentEmotion == "$u"))
      return;
    this.CurrentEmotion = "$neutral";
  }

  /// <summary>Parse dialogue commands and tokens in the current dialogue (i.e. the <see cref="F:StardewValley.Dialogue.currentDialogueIndex" /> entry in <see cref="F:StardewValley.Dialogue.dialogues" />).</summary>
  public virtual void prepareCurrentDialogueForDisplay()
  {
    this.applyAndSkipPlainSideEffects();
    if (this.currentDialogueIndex >= this.dialogues.Count)
      return;
    string str1 = Utility.ParseGiftReveals(this.dialogues[this.currentDialogueIndex].Text);
    this.showPortrait = true;
    if (str1.StartsWith("$v"))
    {
      string[] strArray = ArgUtility.SplitBySpace(str1);
      string eventId = strArray[1];
      bool flag1 = true;
      bool flag2 = true;
      if (strArray.Length > 2 && strArray[2] == "false")
        flag1 = false;
      if (strArray.Length > 3 && strArray[3] == "false")
        flag2 = false;
      int num1 = flag1 ? 1 : 0;
      int num2 = flag2 ? 1 : 0;
      if (Game1.PlayEvent(eventId, num1 != 0, num2 != 0))
      {
        this.dialogues.Clear();
        this.exitCurrentDialogue();
      }
      else
      {
        this.exitCurrentDialogue();
        if (this.isDialogueFinished())
          return;
        this.prepareCurrentDialogueForDisplay();
      }
    }
    else
    {
      if (str1.Contains('}'))
      {
        this.farmer.mailReceived.Add(str1.Split('}')[0]);
        str1 = str1.Substring(str1.IndexOf("}") + 1).Replace("$k", "");
      }
      if (str1.Contains("$k"))
      {
        str1 = str1.Replace("$k", "");
        this.dialogues.RemoveRange(this.currentDialogueIndex + 1, this.dialogues.Count - 1 - this.currentDialogueIndex);
        if (str1.Length < 2)
          this.finishedLastDialogue = true;
      }
      if (str1.StartsWith('%'))
      {
        bool flag = false;
        foreach (string percentToken in Dialogue.percentTokens)
        {
          if (str1.StartsWith(percentToken))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this.indexesWithoutPortrait.Add(this.currentDialogueIndex);
          this.showPortrait = false;
          str1 = str1.Substring(1);
        }
      }
      else if (this.indexesWithoutPortrait.Contains(this.currentDialogueIndex))
        this.showPortrait = false;
      string str2 = this.ReplacePlayerEnteredStrings(str1);
      if (str2.Contains('['))
      {
        int num3 = -1;
        do
        {
          num3 = str2.IndexOf('[', Math.Max(num3, 0));
          if (num3 >= 0)
          {
            int num4 = str2.IndexOf(']', num3);
            if (num4 >= 0)
            {
              string[] strArray = ArgUtility.SplitBySpace(str2.Substring(num3 + 1, num4 - num3 - 1));
              bool flag = false;
              foreach (string itemId in strArray)
              {
                if (ItemRegistry.GetData(itemId) == null)
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                ++num3;
              }
              else
              {
                Item obj = ItemRegistry.Create(Game1.random.Choose<string>(strArray));
                if (obj != null)
                {
                  if (this.farmer.addItemToInventoryBool(obj, true))
                    this.farmer.showCarrying();
                  else
                    this.farmer.addItemByMenuIfNecessary(obj, forceQueue: true);
                }
                str2 = str2.Remove(num3, num4 - num3 + 1);
              }
            }
            else
              break;
          }
        }
        while (num3 >= 0 && num3 < str2.Length);
      }
      string str3 = str2.Replace("%time", Game1.getTimeOfDayString(Game1.timeOfDay));
      bool? nullable = this.speaker?.SpeaksDwarvish();
      if (nullable.HasValue && nullable.GetValueOrDefault() && !this.farmer.canUnderstandDwarves)
        str3 = Dialogue.convertToDwarvish(str3);
      this.dialogues[this.currentDialogueIndex].Text = str3;
    }
  }

  public virtual string getCurrentDialogue()
  {
    if (this.currentDialogueIndex >= this.dialogues.Count || this.finishedLastDialogue)
      return "";
    return this.dialogues.Count <= 0 ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.792") : this.dialogues[this.currentDialogueIndex].Text;
  }

  public bool isItemGrabDialogue()
  {
    return this.currentDialogueIndex < this.dialogues.Count && this.dialogues[this.currentDialogueIndex].Text.Contains('[');
  }

  /// <summary>Whether we're currently displaying the last entry in <see cref="F:StardewValley.Dialogue.dialogues" /> which has text to display.</summary>
  public bool isOnFinalDialogue()
  {
    for (int index = this.currentDialogueIndex + 1; index < this.dialogues.Count; ++index)
    {
      if (this.dialogues[index].HasText)
        return false;
    }
    return true;
  }

  public bool isDialogueFinished() => this.finishedLastDialogue;

  public string ReplacePlayerEnteredStrings(string str)
  {
    if (string.IsNullOrEmpty(str))
      return str;
    string newValue1 = Utility.FilterUserName(this.farmer.Name);
    str = str.Replace("@", newValue1);
    if (str.Contains('%'))
    {
      str = str.Replace("%firstnameletter", newValue1.Substring(0, Math.Max(0, newValue1.Length / 2)));
      if (str.Contains("%spouse"))
      {
        if (this.farmer.spouse != null)
        {
          string displayName = NPC.GetDisplayName(this.farmer.spouse);
          str = str.Replace("%spouse", displayName);
        }
        else
        {
          long? spouse = this.farmer.team.GetSpouse(this.farmer.UniqueMultiplayerID);
          if (spouse.HasValue)
          {
            Farmer player = Game1.GetPlayer(spouse.Value);
            str = str.Replace("%spouse", player.Name);
          }
        }
      }
      string newValue2 = Utility.FilterUserName(this.farmer.farmName.Value);
      str = str.Replace("%farm", newValue2);
      string newValue3 = Utility.FilterUserName(this.farmer.favoriteThing.Value);
      str = str.Replace("%favorite", newValue3);
      int numberOfChildren = this.farmer.getNumberOfChildren();
      str = str.Replace("%kid1", numberOfChildren > 0 ? this.farmer.getChildren()[0].displayName : Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.793"));
      str = str.Replace("%kid2", numberOfChildren > 1 ? this.farmer.getChildren()[1].displayName : Game1.content.LoadString("Strings\\StringsFromCSFiles:Dialogue.cs.794"));
      str = str.Replace("%pet", this.farmer.getPetDisplayName());
    }
    return str;
  }

  public string checkForSpecialCharacters(string str)
  {
    str = this.applyGenderSwitch(str, true);
    if (str.Contains('%'))
    {
      str = str.Replace("%adj", Game1.random.Choose<string>(Dialogue.adjectives).ToLower());
      if (str.Contains("%noun"))
        str = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.de ? str.Substring(0, str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Game1.random.Choose<string>(Dialogue.nouns)) + str.Substring(str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Game1.random.Choose<string>(Dialogue.nouns)) : str.Substring(0, str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Game1.random.Choose<string>(Dialogue.nouns).ToLower()) + str.Substring(str.IndexOf("%noun") + "%noun".Length).Replace("%noun", Game1.random.Choose<string>(Dialogue.nouns).ToLower());
      str = str.Replace("%place", Game1.random.Choose<string>(Dialogue.places));
      str = str.Replace("%name", Dialogue.randomName());
      str = str.Replace("%band", Game1.samBandName);
      if (str.Contains("%book"))
        str = str.Replace("%book", Game1.elliottBookName);
      str = str.Replace("%year", Game1.year.ToString() ?? "");
      str = str.Replace("%season", Game1.CurrentSeasonDisplayName);
      if (str.Contains("%fork"))
      {
        str = str.Replace("%fork", "");
        if (Game1.currentLocation.currentEvent != null)
          Game1.currentLocation.currentEvent.specialEventVariable1 = true;
      }
    }
    return str;
  }

  /// <summary>Get the gender-appropriate dialogue from a dialogue string which may contain a gender-switch token.</summary>
  /// <param name="str">The dialogue string to parse.</param>
  /// <param name="altTokenOnly">Only apply the <see cref="F:StardewValley.Dialogue.genderDialogueSplitCharacter2" /> token, and ignore <see cref="F:StardewValley.Dialogue.genderDialogueSplitCharacter" />.</param>
  public string applyGenderSwitch(string str, bool altTokenOnly = false)
  {
    return Dialogue.applyGenderSwitch(this.farmer.Gender, str, altTokenOnly);
  }

  /// <summary>Get the gender-appropriate dialogue from a dialogue string which may contain gender-switch tokens.</summary>
  /// <param name="gender">The gender for which to apply tokens.</param>
  /// <param name="str">The dialogue string to parse.</param>
  /// <param name="altTokenOnly">Only apply the <see cref="F:StardewValley.Dialogue.genderDialogueSplitCharacter2" /> token, and ignore <see cref="F:StardewValley.Dialogue.genderDialogueSplitCharacter" />.</param>
  public static string applyGenderSwitch(Gender gender, string str, bool altTokenOnly = false)
  {
    str = Dialogue.applyGenderSwitchBlocks(gender, str);
    int length = !altTokenOnly ? str.IndexOf('^') : -1;
    if (length == -1)
      length = str.IndexOf('¦');
    if (length != -1)
      str = gender == Gender.Male ? str.Substring(0, length) : str.Substring(length + 1);
    return str;
  }

  /// <summary>Replace gender-switch blocks like <c>${male^female}$</c> or <c>${male¦female}$</c> in the input string with the gender-appropriate text.</summary>
  /// <param name="gender">The gender for which to apply tokens.</param>
  /// <param name="str">The dialogue string to parse.</param>
  /// <remarks>This should only be called directly in cases where <see cref="M:StardewValley.Dialogue.applyGenderSwitch(StardewValley.Gender,System.String,System.Boolean)" /> isn't applied, since that includes gender-switch blocks.</remarks>
  public static string applyGenderSwitchBlocks(Gender gender, string str)
  {
    int startIndex = 0;
    while (true)
    {
      int num1 = str.IndexOf("${", startIndex, StringComparison.Ordinal);
      if (num1 != -1)
      {
        int num2 = str.IndexOf("}$", num1, StringComparison.Ordinal);
        if (num2 != -1)
        {
          string str1 = str.Substring(num1 + 2, num2 - num1 - 2);
          string[] array = str1.Contains('¦') ? str1.Split('¦') : str1.Split('^');
          string str2;
          switch (gender)
          {
            case Gender.Male:
              str2 = array[0];
              break;
            case Gender.Female:
              str2 = ArgUtility.Get(array, 1, array[0]);
              break;
            default:
              str2 = ArgUtility.Get(array, 2, array[0]);
              break;
          }
          str = str.Substring(0, num1) + str2 + str.Substring(num2 + "}$".Length);
          startIndex = num1 + str2.Length;
        }
        else
          goto label_4;
      }
      else
        break;
    }
    return str;
label_4:
    return str;
  }

  /// <summary>If the next dialogue(s) in <see cref="F:StardewValley.Dialogue.dialogues" /> have side-effects without text, apply them and set <see cref="F:StardewValley.Dialogue.currentDialogueIndex" /> to the next dialogue which has text.</summary>
  public void applyAndSkipPlainSideEffects()
  {
    for (; this.currentDialogueIndex < this.dialogues.Count; ++this.currentDialogueIndex)
    {
      DialogueLine dialogue = this.dialogues[this.currentDialogueIndex];
      if (dialogue.HasText)
        break;
      Action sideEffects = dialogue.SideEffects;
      if (sideEffects != null)
        sideEffects();
    }
  }

  public static string randomName()
  {
    switch (LocalizedContentManager.CurrentLanguageCode)
    {
      case LocalizedContentManager.LanguageCode.ja:
        string[] strArray1 = new string[38]
        {
          "ローゼン",
          "ミルド",
          "ココ",
          "ナミ",
          "こころ",
          "サルコ",
          "ハンゾー",
          "クッキー",
          "ココナツ",
          "せん",
          "ハル",
          "ラン",
          "オサム",
          "ヨシ",
          "ソラ",
          "ホシ",
          "まこと",
          "マサ",
          "ナナ",
          "リオ",
          "リン",
          "フジ",
          "うどん",
          "ミント",
          "さくら",
          "ボンボン",
          "レオ",
          "モリ",
          "コーヒー",
          "ミルク",
          "マロン",
          "クルミ",
          "サムライ",
          "カミ",
          "ゴロ",
          "マル",
          "チビ",
          "ユキダマ"
        };
        return Game1.random.Choose<string>(strArray1);
      case LocalizedContentManager.LanguageCode.ru:
        string[] strArray2 = new string[50]
        {
          "Августина",
          "Альф",
          "Анфиса",
          "Ариша",
          "Афоня",
          "Баламут",
          "Балкан",
          "Бандит",
          "Бланка",
          "Бобик",
          "Боня",
          "Борька",
          "Буренка",
          "Бусинка",
          "Вася",
          "Гаврюша",
          "Глаша",
          "Гоша",
          "Дуня",
          "Дуся",
          "Зорька",
          "Ивонна",
          "Игнат",
          "Кеша",
          "Клара",
          "Кузя",
          "Лада",
          "Максимус",
          "Маня",
          "Марта",
          "Маруся",
          "Моня",
          "Мотя",
          "Мурзик",
          "Мурка",
          "Нафаня",
          "Ника",
          "Нюша",
          "Проша",
          "Пятнушка",
          "Сеня",
          "Сивка",
          "Тихон",
          "Тоша",
          "Фунтик",
          "Шайтан",
          "Юнона",
          "Юпитер",
          "Ягодка",
          "Яшка"
        };
        return Game1.random.Choose<string>(strArray2);
      case LocalizedContentManager.LanguageCode.zh:
        string[] strArray3 = new string[183]
        {
          "雨果",
          "蛋挞",
          "小百合",
          "毛毛",
          "小雨",
          "小溪",
          "精灵",
          "安琪儿",
          "小糕",
          "玫瑰",
          "小黄",
          "晓雨",
          "阿江",
          "铃铛",
          "马琪",
          "果粒",
          "郁金香",
          "小黑",
          "雨露",
          "小江",
          "灵力",
          "萝拉",
          "豆豆",
          "小莲",
          "斑点",
          "小雾",
          "阿川",
          "丽丹",
          "玛雅",
          "阿豆",
          "花花",
          "琉璃",
          "滴答",
          "阿山",
          "丹麦",
          "梅西",
          "橙子",
          "花儿",
          "晓璃",
          "小夕",
          "山大",
          "咪咪",
          "卡米",
          "红豆",
          "花朵",
          "洋洋",
          "太阳",
          "小岩",
          "汪汪",
          "玛利亚",
          "小菜",
          "花瓣",
          "阳阳",
          "小夏",
          "石头",
          "阿狗",
          "邱洁",
          "苹果",
          "梨花",
          "小希",
          "天天",
          "浪子",
          "阿猫",
          "艾薇儿",
          "雪梨",
          "桃花",
          "阿喜",
          "云朵",
          "风儿",
          "狮子",
          "绮丽",
          "雪莉",
          "樱花",
          "小喜",
          "朵朵",
          "田田",
          "小红",
          "宝娜",
          "梅子",
          "小樱",
          "嘻嘻",
          "云儿",
          "小草",
          "小黄",
          "纳香",
          "阿梅",
          "茶花",
          "哈哈",
          "芸儿",
          "东东",
          "小羽",
          "哈豆",
          "桃子",
          "茶叶",
          "双双",
          "沫沫",
          "楠楠",
          "小爱",
          "麦当娜",
          "杏仁",
          "椰子",
          "小王",
          "泡泡",
          "小林",
          "小灰",
          "马格",
          "鱼蛋",
          "小叶",
          "小李",
          "晨晨",
          "小琳",
          "小慧",
          "布鲁",
          "晓梅",
          "绿叶",
          "甜豆",
          "小雪",
          "晓林",
          "康康",
          "安妮",
          "樱桃",
          "香板",
          "甜甜",
          "雪花",
          "虹儿",
          "美美",
          "葡萄",
          "薇儿",
          "金豆",
          "雪玲",
          "瑶瑶",
          "龙眼",
          "丁香",
          "晓云",
          "雪豆",
          "琪琪",
          "麦子",
          "糖果",
          "雪丽",
          "小艺",
          "小麦",
          "小圆",
          "雨佳",
          "小火",
          "麦茶",
          "圆圆",
          "春儿",
          "火灵",
          "板子",
          "黑点",
          "冬冬",
          "火花",
          "米粒",
          "喇叭",
          "晓秋",
          "跟屁虫",
          "米果",
          "欢欢",
          "爱心",
          "松子",
          "丫头",
          "双子",
          "豆芽",
          "小子",
          "彤彤",
          "棉花糖",
          "阿贵",
          "仙儿",
          "冰淇淋",
          "小彬",
          "贤儿",
          "冰棒",
          "仔仔",
          "格子",
          "水果",
          "悠悠",
          "莹莹",
          "巧克力",
          "梦洁",
          "汤圆",
          "静香",
          "茄子",
          "珍珠"
        };
        return Game1.random.Choose<string>(strArray3);
      default:
        int num = Game1.random.Next(3, 6);
        string[] strArray4 = new string[24]
        {
          "B",
          "Br",
          "J",
          "F",
          "S",
          "M",
          "C",
          "Ch",
          "L",
          "P",
          "K",
          "W",
          "G",
          "Z",
          "Tr",
          "T",
          "Gr",
          "Fr",
          "Pr",
          "N",
          "Sn",
          "R",
          "Sh",
          "St"
        };
        string[] strArray5 = new string[12]
        {
          "ll",
          "tch",
          "l",
          "m",
          "n",
          "p",
          "r",
          "s",
          "t",
          "c",
          "rt",
          "ts"
        };
        string[] source1 = new string[5]
        {
          "a",
          "e",
          "i",
          "o",
          "u"
        };
        string[] strArray6 = new string[5]
        {
          "ie",
          "o",
          "a",
          "ers",
          "ley"
        };
        Dictionary<string, string[]> dictionary1 = new Dictionary<string, string[]>()
        {
          ["a"] = new string[6]
          {
            "nie",
            "bell",
            "bo",
            "boo",
            "bella",
            "s"
          },
          ["e"] = new string[4]{ "ll", "llo", "", "o" },
          ["i"] = new string[18]
          {
            "ck",
            "e",
            "bo",
            "ba",
            "lo",
            "la",
            "to",
            "ta",
            "no",
            "na",
            "ni",
            "a",
            "o",
            "zor",
            "que",
            "ca",
            "co",
            "mi"
          },
          ["o"] = new string[12]
          {
            "nie",
            "ze",
            "dy",
            "da",
            "o",
            "ver",
            "la",
            "lo",
            "s",
            "ny",
            "mo",
            "ra"
          },
          ["u"] = new string[4]{ "rt", "mo", "", "s" }
        };
        Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>()
        {
          ["a"] = new string[12]
          {
            "nny",
            "sper",
            "trina",
            "bo",
            "-bell",
            "boo",
            "lbert",
            "sko",
            "sh",
            "ck",
            "ishe",
            "rk"
          },
          ["e"] = new string[9]
          {
            "lla",
            "llo",
            "rnard",
            "cardo",
            "ffe",
            "ppo",
            "ppa",
            "tch",
            "x"
          },
          ["i"] = new string[18]
          {
            "llard",
            "lly",
            "lbo",
            "cky",
            "card",
            "ne",
            "nnie",
            "lbert",
            "nono",
            "nano",
            "nana",
            "ana",
            "nsy",
            "msy",
            "skers",
            "rdo",
            "rda",
            "sh"
          },
          ["o"] = new string[17]
          {
            "nie",
            "zzy",
            "do",
            "na",
            "la",
            "la",
            "ver",
            "ng",
            "ngus",
            "ny",
            "-mo",
            "llo",
            "ze",
            "ra",
            "ma",
            "cco",
            "z"
          },
          ["u"] = new string[11]
          {
            "ssie",
            "bbie",
            "ffy",
            "bba",
            "rt",
            "s",
            "mby",
            "mbo",
            "mbus",
            "ngus",
            "cky"
          }
        };
        string str1 = strArray4[Game1.random.Next(strArray4.Length - 1)];
        for (int index = 1; index < num - 1; ++index)
        {
          str1 = index % 2 != 0 ? str1 + Game1.random.Choose<string>(source1) : str1 + Game1.random.Choose<string>(strArray5);
          if (str1.Length >= num)
            break;
        }
        char ch = str1[str1.Length - 1];
        string key = ch.ToString();
        if (Game1.random.NextBool() && !((IEnumerable<string>) source1).Contains<string>(key))
          str1 += Game1.random.Choose<string>(strArray6);
        else if (((IEnumerable<string>) source1).Contains<string>(key))
        {
          if (Game1.random.NextDouble() < 0.8)
            str1 = str1.Length > 3 ? str1 + Game1.random.ChooseFrom<string>((IList<string>) dictionary1[key]) : str1 + Game1.random.ChooseFrom<string>((IList<string>) dictionary2[key]);
        }
        else
          str1 += Game1.random.Choose<string>(source1);
        for (int index = str1.Length - 1; index > 2; --index)
        {
          string[] source2 = source1;
          ch = str1[index];
          string str2 = ch.ToString();
          if (((IEnumerable<string>) source2).Contains<string>(str2))
          {
            string[] source3 = source1;
            ch = str1[index - 2];
            string str3 = ch.ToString();
            if (((IEnumerable<string>) source3).Contains<string>(str3))
            {
              ch = str1[index - 1];
              switch (ch)
              {
                case 'c':
                  str1 = $"{str1.Substring(0, index)}k{str1.Substring(index)}";
                  --index;
                  continue;
                case 'l':
                  str1 = $"{str1.Substring(0, index - 1)}n{str1.Substring(index)}";
                  --index;
                  continue;
                case 'r':
                  str1 = $"{str1.Substring(0, index - 1)}k{str1.Substring(index)}";
                  --index;
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        if (str1.Length <= 3 && Game1.random.NextDouble() < 0.1)
          str1 = Game1.random.NextBool() ? str1 + str1 : $"{str1}-{str1}";
        if (str1.Length <= 2 && str1.Last<char>() == 'e')
        {
          string str4 = str1;
          ch = Game1.random.Choose<char>('m', 'p', 'b');
          string str5 = ch.ToString();
          str1 = str4 + str5;
        }
        return Dialogue.ReplaceBadRandomName(str1);
    }
  }

  /// <summary>Get an alternative name if the input name contains bad words.</summary>
  /// <param name="name">The random name </param>
  public static string ReplaceBadRandomName(string name)
  {
    string lower = name.ToLower();
    if (lower.Contains("bitch") || lower.Contains("cock") || lower.Contains("cum") || lower.Contains("fuck") || lower.Contains("goock") || lower.Contains("gook") || lower.Contains("kike") || lower.Contains("nigg") || lower.Contains("pusie") || lower.Contains("puss") || lower.Contains("puta") || lower.Contains("rape") || lower.Contains("sex") || lower.Contains("shart") || lower.Contains("shit") || lower.Contains("taboo") || lower.Contains("trann") || lower.Contains("willy"))
      return Game1.random.Choose<string>("Bobo", "Wumbus");
    if (lower != null)
    {
      switch (lower.Length)
      {
        case 5:
          switch (lower[3])
          {
            case 'e':
              if (lower == "boner")
                goto label_23;
              goto label_32;
            case 'i':
              if (lower == "rapie")
                return "Rapimi";
              goto label_32;
            case 'k':
              switch (lower)
              {
                case "cucka":
                case "cucke":
                case "cucko":
                case "cucky":
                  goto label_25;
                case "packi":
                  goto label_28;
                default:
                  goto label_32;
              }
            case 'n':
              if (lower == "trani")
                goto label_31;
              goto label_32;
            case 'o':
              if (lower == "penos")
                break;
              goto label_32;
            case 'p':
              switch (lower)
              {
                case "grope":
                  goto label_26;
                case "trapi":
                  goto label_31;
                default:
                  goto label_32;
              }
            case 's':
              if (lower == "natsi")
                return "Natsia";
              goto label_32;
            case 'u':
              if (lower == "penus")
                break;
              goto label_32;
            default:
              goto label_32;
          }
          return "Penono";
        case 6:
          switch (lower[3])
          {
            case 'e':
              if (lower == "boners")
                break;
              goto label_32;
            case 'k':
              switch (lower)
              {
                case "cuckas":
                case "cuckie":
                case "cuckos":
                  goto label_25;
                case "packie":
                  goto label_28;
                default:
                  goto label_32;
              }
            case 'n':
              if (lower == "tranie")
                goto label_31;
              goto label_32;
            case 'p':
              if (lower == "trapie")
                goto label_31;
              goto label_32;
            case 's':
              if (lower == "bussie")
                return "Busu";
              goto label_32;
            default:
              goto label_32;
          }
          break;
        case 7:
          switch (lower[0])
          {
            case 'c':
              if (lower == "cuckers")
                goto label_25;
              goto label_32;
            case 'g':
              if (lower == "gropers")
                goto label_26;
              goto label_32;
            default:
              goto label_32;
          }
        case 8:
          if (lower == "trananie")
            goto label_31;
          goto label_32;
        default:
          goto label_32;
      }
label_23:
      return "Boneo";
label_25:
      return "Cubbie";
label_26:
      return "Gropello";
label_28:
      return "Packina";
label_31:
      return "Tranello";
    }
label_32:
    return name;
  }

  public virtual string exitCurrentDialogue()
  {
    if (this.isOnFinalDialogue())
    {
      ++this.currentDialogueIndex;
      this.applyAndSkipPlainSideEffects();
      Action onFinish = this.onFinish;
      if (onFinish != null)
        onFinish();
    }
    int num = this.isCurrentStringContinuedOnNextScreen ? 1 : 0;
    if (this.currentDialogueIndex < this.dialogues.Count - 1)
    {
      ++this.currentDialogueIndex;
      this.applyAndSkipPlainSideEffects();
      this.checkForSpecialDialogueAttributes();
    }
    else
      this.finishedLastDialogue = true;
    return num != 0 ? this.getCurrentDialogue() : (string) null;
  }

  private void checkForSpecialDialogueAttributes()
  {
    this.CurrentEmotion = (string) null;
    this.isCurrentStringContinuedOnNextScreen = false;
    this.dontFaceFarmer = false;
    if (this.currentDialogueIndex >= this.dialogues.Count)
      return;
    DialogueLine dialogue = this.dialogues[this.currentDialogueIndex];
    if (dialogue.Text.Contains("{"))
    {
      dialogue.Text = dialogue.Text.Replace("{", "");
      this.isCurrentStringContinuedOnNextScreen = true;
    }
    if (dialogue.Text.Contains("%noturn"))
    {
      dialogue.Text = dialogue.Text.Replace("%noturn", "");
      this.dontFaceFarmer = true;
    }
    this.checkEmotions();
  }

  private void checkEmotions()
  {
    this.CurrentEmotion = (string) null;
    if (this.currentDialogueIndex >= this.dialogues.Count)
      return;
    DialogueLine dialogue = this.dialogues[this.currentDialogueIndex];
    string text = dialogue.Text;
    int startIndex = text.IndexOf('$');
    if (startIndex == -1 || this.dialogues.Count <= 0)
      return;
    if (text.Contains("$h"))
    {
      this.CurrentEmotion = "$h";
      dialogue.Text = text.Replace("$h", "");
    }
    else if (text.Contains("$s"))
    {
      this.CurrentEmotion = "$s";
      dialogue.Text = text.Replace("$s", "");
    }
    else if (text.Contains("$u"))
    {
      this.CurrentEmotion = "$u";
      dialogue.Text = text.Replace("$u", "");
    }
    else if (text.Contains("$l"))
    {
      this.CurrentEmotion = "$l";
      dialogue.Text = text.Replace("$l", "");
    }
    else if (text.Contains("$a"))
    {
      this.CurrentEmotion = "$a";
      dialogue.Text = text.Replace("$a", "");
    }
    else
    {
      int num = 0;
      for (int index = startIndex + 1; index < text.Length && char.IsDigit(text[index]); ++index)
        ++num;
      if (num <= 0)
        return;
      string oldValue = text.Substring(startIndex, num + 1);
      this.CurrentEmotion = oldValue;
      dialogue.Text = text.Replace(oldValue, "");
    }
  }

  public List<NPCDialogueResponse> getNPCResponseOptions() => this.playerResponses;

  public Response[] getResponseOptions()
  {
    return this.playerResponses.Cast<Response>().ToArray<Response>();
  }

  public bool isCurrentDialogueAQuestion()
  {
    return this.isLastDialogueInteractive && this.currentDialogueIndex == this.dialogues.Count - 1;
  }

  public virtual bool chooseResponse(Response response)
  {
    for (int index = 0; index < this.playerResponses.Count; ++index)
    {
      if (this.playerResponses[index].responseKey != null && response.responseKey != null && this.playerResponses[index].responseKey.Equals(response.responseKey))
      {
        if (this.answerQuestionBehavior != null)
        {
          if (this.answerQuestionBehavior(index))
            Game1.currentSpeaker = (NPC) null;
          this.isLastDialogueInteractive = false;
          this.finishedLastDialogue = true;
          this.answerQuestionBehavior = (Dialogue.onAnswerQuestion) null;
          return true;
        }
        if (this.quickResponse)
        {
          this.isLastDialogueInteractive = false;
          this.finishedLastDialogue = true;
          this.isCurrentStringContinuedOnNextScreen = true;
          this.speaker.setNewDialogue(new Dialogue(this.speaker, (string) null, this.quickResponses[index]));
          Game1.drawDialogue(this.speaker);
          this.speaker.faceTowardFarmerForPeriod(4000, 3, false, this.farmer);
          return true;
        }
        if (Game1.isFestival())
        {
          Game1.currentLocation.currentEvent.answerDialogueQuestion(this.speaker, this.playerResponses[index].responseKey);
          this.isLastDialogueInteractive = false;
          this.finishedLastDialogue = true;
          return false;
        }
        this.farmer.changeFriendship(this.playerResponses[index].friendshipChange, this.speaker);
        if (this.playerResponses[index].id != null)
          this.farmer.addSeenResponse(this.playerResponses[index].id);
        if (this.playerResponses[index].extraArgument != null)
        {
          try
          {
            this.performDialogueResponseExtraArgument(this.farmer, this.playerResponses[index].extraArgument);
          }
          catch (Exception ex)
          {
          }
        }
        this.isLastDialogueInteractive = false;
        this.finishedLastDialogue = false;
        this.parseDialogueString(this.speaker.Dialogue[this.playerResponses[index].responseKey], $"{this.speaker.LoadedDialogueKey}:{this.playerResponses[index].responseKey}");
        this.isCurrentStringContinuedOnNextScreen = true;
        return false;
      }
    }
    return false;
  }

  public void performDialogueResponseExtraArgument(Farmer farmer, string argument)
  {
    string[] strArray = argument.Split("_");
    if (!strArray[0].EqualsIgnoreCase("friend"))
      return;
    farmer.changeFriendship(Convert.ToInt32(strArray[2]), Game1.getCharacterFromName(strArray[1]));
  }

  /// <summary>Convert the current dialogue text into Dwarvish, as spoken by Dwarf when the player doesn't have the Dwarvish Translation Guide.</summary>
  public void convertToDwarvish()
  {
    for (int index = 0; index < this.dialogues.Count; ++index)
      this.dialogues[index].Text = Dialogue.convertToDwarvish(this.dialogues[index].Text);
  }

  /// <summary>Convert dialogue text into Dwarvish, as spoken by Dwarf when the player doesn't have the Dwarvish Translation Guide.</summary>
  /// <param name="str">The text to translate.</param>
  public static string convertToDwarvish(string str)
  {
    if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.zh)
    {
      string str1 = "bcdfghjklmnpqrstvwxyz";
      string str2 = "bcd fghj klmn pqrst vwxy z";
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (char ch1 in str)
      {
        int num = (int) ch1;
        if (19968 <= num && num <= 40959 /*0x9FFF*/ || 12352 <= num && num <= 12543 || ch1 == '々' || 44032 <= num && num <= 55215)
        {
          char upper = str1[num % str1.Length];
          if (flag)
          {
            upper = char.ToUpper(upper);
            flag = false;
          }
          stringBuilder.Append(upper);
          char ch2 = str2[(num >> 1) % str2.Length];
          stringBuilder.Append(ch2);
        }
        else
        {
          stringBuilder.Append(ch1);
          if (ch1 != ' ')
            flag = true;
        }
      }
      return stringBuilder.ToString();
    }
    StringBuilder stringBuilder1 = new StringBuilder();
    for (int index = 0; index < str.Length; ++index)
    {
      switch (str[index])
      {
        case '\n':
        case 'n':
        case 'p':
          continue;
        case ' ':
        case '!':
        case '"':
        case '\'':
        case ',':
        case '.':
        case '?':
        case 'h':
        case 'm':
        case 's':
          stringBuilder1.Append(str[index]);
          continue;
        case '0':
          stringBuilder1.Append('Q');
          continue;
        case '1':
          stringBuilder1.Append('M');
          continue;
        case '5':
          stringBuilder1.Append('X');
          continue;
        case '9':
          stringBuilder1.Append('V');
          continue;
        case 'A':
          stringBuilder1.Append('O');
          continue;
        case 'E':
          stringBuilder1.Append('U');
          continue;
        case 'I':
          stringBuilder1.Append("E");
          continue;
        case 'O':
          stringBuilder1.Append('A');
          continue;
        case 'U':
          stringBuilder1.Append("I");
          continue;
        case 'Y':
          stringBuilder1.Append("Ol");
          continue;
        case 'Z':
          stringBuilder1.Append('B');
          continue;
        case 'a':
          stringBuilder1.Append('o');
          continue;
        case 'c':
          stringBuilder1.Append('t');
          continue;
        case 'd':
          stringBuilder1.Append('p');
          continue;
        case 'e':
          stringBuilder1.Append('u');
          continue;
        case 'g':
          stringBuilder1.Append('l');
          continue;
        case 'i':
          stringBuilder1.Append("e");
          continue;
        case 'o':
          stringBuilder1.Append('a');
          continue;
        case 't':
          stringBuilder1.Append('n');
          continue;
        case 'u':
          stringBuilder1.Append("i");
          continue;
        case 'y':
          stringBuilder1.Append("ol");
          continue;
        case 'z':
          stringBuilder1.Append('b');
          continue;
        default:
          if (char.IsLetterOrDigit(str[index]))
          {
            stringBuilder1.Append((char) ((uint) str[index] + 2U));
            continue;
          }
          continue;
      }
    }
    return stringBuilder1.ToString().Replace("nhu", "doo");
  }

  public delegate bool onAnswerQuestion(int whichResponse);
}
