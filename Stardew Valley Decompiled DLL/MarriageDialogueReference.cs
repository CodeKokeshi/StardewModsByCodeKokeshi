// Decompiled with JetBrains decompiler
// Type: StardewValley.MarriageDialogueReference
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Netcode;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace StardewValley;

public class MarriageDialogueReference : INetObject<NetFields>, IEquatable<MarriageDialogueReference>
{
  public const string ENDEARMENT_TOKEN = "%endearment";
  public const string ENDEARMENT_TOKEN_LOWER = "%endearmentlower";
  private readonly NetString _dialogueFile = new NetString("");
  private readonly NetString _dialogueKey = new NetString("");
  private readonly NetBool _isGendered = new NetBool(false);
  private readonly NetStringList _substitutions = new NetStringList();

  public NetFields NetFields { get; } = new NetFields(nameof (MarriageDialogueReference));

  public MarriageDialogueReference()
  {
    this.NetFields.SetOwner((INetObject<NetFields>) this).AddField((INetSerializable) this._dialogueFile, nameof (_dialogueFile)).AddField((INetSerializable) this._dialogueKey, nameof (_dialogueKey)).AddField((INetSerializable) this._isGendered, nameof (_isGendered)).AddField((INetSerializable) this._substitutions, nameof (_substitutions));
  }

  public MarriageDialogueReference(
    string dialogue_file,
    string dialogue_key,
    bool gendered = false,
    params string[] substitutions)
    : this()
  {
    this._dialogueFile.Value = dialogue_file;
    this._dialogueKey.Value = dialogue_key;
    this._isGendered.Value = gendered;
    if (substitutions.Length == 0)
      return;
    this._substitutions.AddRange((IEnumerable<string>) substitutions);
  }

  public string GetText() => "";

  public bool IsItemGrabDialogue(NPC n) => this.GetDialogue(n).isItemGrabDialogue();

  /// <summary>Replace any tokens in the dialogue text with their localized variants.</summary>
  /// <param name="dialogue">The dialogue to modify.</param>
  /// <param name="npc">The NPC for which to replace tokens.</param>
  protected void _ReplaceTokens(Dialogue dialogue, NPC npc)
  {
    for (int index = 0; index < dialogue.dialogues.Count; ++index)
      dialogue.dialogues[index].Text = this._ReplaceTokens(dialogue.dialogues[index].Text, npc);
  }

  /// <summary>Replace any tokens in the dialogue text with their localized variants.</summary>
  /// <param name="text">The dialogue text to modify.</param>
  /// <param name="npc">The NPC for which to replace tokens.</param>
  protected string _ReplaceTokens(string text, NPC npc)
  {
    text = text.Replace("%endearmentlower", npc.getTermOfSpousalEndearment().ToLower());
    text = text.Replace("%endearment", npc.getTermOfSpousalEndearment());
    return text;
  }

  public Dialogue GetDialogue(NPC n)
  {
    if (this._dialogueFile.Value.Contains("Marriage"))
    {
      Dialogue dialogue = n.tryToGetMarriageSpecificDialogue(this._dialogueKey.Value);
      if (dialogue == null)
      {
        Game1.log.Warn($"NPC '{n.Name}' couldn't get marriage dialogue key '{this._dialogueKey.Value}': not found.");
        dialogue = Dialogue.GetFallbackForError(n);
      }
      dialogue.removeOnNextMove = true;
      this._ReplaceTokens(dialogue, n);
      return dialogue;
    }
    string str1 = $"{this._dialogueFile.Value}:{this._dialogueKey.Value}";
    string str2;
    if (!this._isGendered.Value)
      str2 = Game1.content.LoadString(str1, (object) this._substitutions);
    else
      str2 = Game1.LoadStringByGender(n.Gender, str1, (object) this._substitutions);
    string text = str2;
    return new Dialogue(n, str1, this._ReplaceTokens(text, n))
    {
      removeOnNextMove = true
    };
  }

  public string DialogueFile => this._dialogueFile.Value;

  public string DialogueKey => this._dialogueKey.Value;

  public bool IsGendered => this._isGendered.Value;

  public string[] Substitutions => this._substitutions.ToArray<string>();

  public bool Equals(MarriageDialogueReference other)
  {
    return object.Equals((object) this._dialogueFile.Value, (object) other._dialogueFile.Value) && object.Equals((object) this._dialogueKey.Value, (object) other._dialogueKey.Value) && object.Equals((object) this._isGendered.Value, (object) other._isGendered.Value) && this._substitutions.SequenceEqual<string>((IEnumerable<string>) other._substitutions);
  }

  public override bool Equals(object obj)
  {
    return obj is MarriageDialogueReference other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    int hashCode = ((13 * 7 + (this._dialogueFile.Value == null ? 0 : this._dialogueFile.Value.GetHashCode())) * 7 + (this._dialogueKey.Value == null ? 0 : this._dialogueFile.Value.GetHashCode())) * 7 + (!this._isGendered.Value ? 1 : 0);
    foreach (string substitution in (NetList<string, NetString>) this._substitutions)
      hashCode = hashCode * 7 + substitution.GetHashCode();
    return hashCode;
  }
}
