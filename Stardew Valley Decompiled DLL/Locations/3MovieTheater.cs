// Decompiled with JetBrains decompiler
// Type: StardewValley.Locations.MovieTheater
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Movies;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Network;
using StardewValley.Pathfinding;
using StardewValley.TokenizableStrings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using xTile.Layers;
using xTile.ObjectModel;

#nullable disable
namespace StardewValley.Locations;

/// <summary>The movie theater location.</summary>
/// <remarks>See also <see cref="T:StardewValley.Events.MovieTheaterScreeningEvent" />.</remarks>
public class MovieTheater : GameLocation
{
  protected bool _startedMovie;
  protected static bool _isJojaTheater = false;
  protected static List<MovieData> _movieData;
  protected static Dictionary<string, MovieData> _movieDataById;
  protected static List<MovieCharacterReaction> _genericReactions;
  protected static List<ConcessionTaste> _concessionTastes;
  protected readonly NetStringDictionary<int, NetInt> _spawnedMoviePatrons = new NetStringDictionary<int, NetInt>();
  protected readonly NetStringDictionary<string, NetString> _purchasedConcessions = new NetStringDictionary<string, NetString>();
  protected readonly NetStringDictionary<int, NetInt> _playerInvitedPatrons = new NetStringDictionary<int, NetInt>();
  protected readonly NetStringDictionary<bool, NetBool> _characterGroupLookup = new NetStringDictionary<bool, NetBool>();
  protected Dictionary<int, List<Point>> _hangoutPoints;
  protected Dictionary<int, List<Point>> _availableHangoutPoints;
  protected int _maxHangoutGroups;
  protected int _movieStartTime = -1;
  [XmlElement("dayFirstEntered")]
  public readonly NetInt dayFirstEntered = new NetInt(-1);
  protected static Dictionary<string, MovieConcession> _concessions;
  public const int LOVE_MOVIE_FRIENDSHIP = 200;
  public const int LIKE_MOVIE_FRIENDSHIP = 100;
  public const int DISLIKE_MOVIE_FRIENDSHIP = 0;
  public const int LOVE_CONCESSION_FRIENDSHIP = 50;
  public const int LIKE_CONCESSION_FRIENDSHIP = 25;
  public const int DISLIKE_CONCESSION_FRIENDSHIP = 0;
  public const int OPEN_TIME = 900;
  public const int CLOSE_TIME = 2100;
  /// <summary>The main tile sheet ID for movie theater map tiles.</summary>
  public const string MainTileSheetId = "movieTheater_tileSheet";
  [XmlIgnore]
  protected Dictionary<string, KeyValuePair<Point, int>> _destinationPositions = new Dictionary<string, KeyValuePair<Point, int>>();
  [XmlIgnore]
  public PerchingBirds birds;
  /// <summary>If set, the movie ID to watch when a movie is requested, instead of the movie for the current date.</summary>
  [XmlIgnore]
  public static string forceMovieId;
  protected int _exitX;
  protected int _exitY;
  private NetEvent1<MovieViewerLockEvent> movieViewerLockEvent = new NetEvent1<MovieViewerLockEvent>();
  private NetEvent1<StartMovieEvent> startMovieEvent = new NetEvent1<StartMovieEvent>();
  private NetEvent1Field<long, NetLong> requestStartMovieEvent = new NetEvent1Field<long, NetLong>();
  private NetEvent1Field<long, NetLong> endMovieEvent = new NetEvent1Field<long, NetLong>();
  protected List<Farmer> _viewingFarmers = new List<Farmer>();
  protected List<List<Character>> _viewingGroups = new List<List<Character>>();
  protected List<List<Character>> _playerGroups = new List<List<Character>>();
  protected List<List<Character>> _npcGroups = new List<List<Character>>();
  protected static bool _hasRequestedMovieStart = false;
  protected static int _playerHangoutGroup = -1;
  protected int _farmerCount;
  protected readonly NetInt currentState = new NetInt();
  protected readonly NetInt showingId = new NetInt();
  public static string[][][][] possibleNPCGroups = new string[7][][][]
  {
    new string[3][][]
    {
      new string[1][]{ new string[1]{ "Lewis" } },
      new string[3][]
      {
        new string[3]{ "Jas", "Vincent", "Marnie" },
        new string[3]{ "Abigail", "Sebastian", "Sam" },
        new string[2]{ "Penny", "Maru" }
      },
      new string[1][]{ new string[2]{ "Lewis", "Marnie" } }
    },
    new string[3][][]
    {
      new string[3][]
      {
        new string[1]{ "Clint" },
        new string[2]{ "Demetrius", "Robin" },
        new string[1]{ "Lewis" }
      },
      new string[2][]
      {
        new string[2]{ "Caroline", "Jodi" },
        new string[3]{ "Abigail", "Sebastian", "Sam" }
      },
      new string[2][]
      {
        new string[1]{ "Lewis" },
        new string[3]{ "Abigail", "Sebastian", "Sam" }
      }
    },
    new string[3][][]
    {
      new string[2][]
      {
        new string[2]{ "Evelyn", "George" },
        new string[1]{ "Lewis" }
      },
      new string[2][]
      {
        new string[2]{ "Penny", "Pam" },
        new string[3]{ "Abigail", "Sebastian", "Sam" }
      },
      new string[2][]
      {
        new string[2]{ "Sandy", "Emily" },
        new string[1]{ "Elliot" }
      }
    },
    new string[3][][]
    {
      new string[3][]
      {
        new string[2]{ "Penny", "Pam" },
        new string[3]{ "Abigail", "Sebastian", "Sam" },
        new string[1]{ "Lewis" }
      },
      new string[2][]
      {
        new string[3]{ "Alex", "Haley", "Emily" },
        new string[3]{ "Abigail", "Sebastian", "Sam" }
      },
      new string[2][]
      {
        new string[2]{ "Pierre", "Caroline" },
        new string[3]{ "Shane", "Jas", "Marnie" }
      }
    },
    new string[3][][]
    {
      null,
      new string[3][]
      {
        new string[2]{ "Haley", "Emily" },
        new string[3]{ "Abigail", "Sebastian", "Sam" },
        new string[1]{ "Lewis" }
      },
      new string[2][]
      {
        new string[2]{ "Penny", "Pam" },
        new string[3]{ "Abigail", "Sebastian", "Sam" }
      }
    },
    new string[3][][]
    {
      new string[1][]{ new string[1]{ "Lewis" } },
      new string[2][]
      {
        new string[2]{ "Penny", "Pam" },
        new string[3]{ "Abigail", "Sebastian", "Sam" }
      },
      new string[2][]
      {
        new string[3]{ "Harvey", "Maru", "Penny" },
        new string[1]{ "Leah" }
      }
    },
    new string[3][][]
    {
      new string[3][]
      {
        new string[2]{ "Penny", "Pam" },
        new string[3]{ "George", "Evelyn", "Alex" },
        new string[1]{ "Lewis" }
      },
      new string[2][]
      {
        new string[2]{ "Gus", "Willy" },
        new string[2]{ "Maru", "Sebastian" }
      },
      new string[2][]
      {
        new string[2]{ "Penny", "Pam" },
        new string[2]{ "Sandy", "Emily" }
      }
    }
  };

  protected int CurrentState
  {
    get => this.currentState.Value;
    set
    {
      if (Game1.IsMasterGame)
        this.currentState.Value = value;
      else
        Game1.log.Warn("Tried to set MovieTheater::CurrentState as a farmhand.");
    }
  }

  protected int ShowingId
  {
    get => this.showingId.Value;
    set
    {
      if (Game1.IsMasterGame)
        this.showingId.Value = value;
      else
        Game1.log.Warn("Tried to set MovieTheater::ShowingId as a farmhand.");
    }
  }

  public MovieTheater()
  {
  }

  public static void AddMoviePoster(GameLocation location, float x, float y, bool isUpcoming = false)
  {
    MovieData movieData = isUpcoming ? MovieTheater.GetUpcomingMovie() : MovieTheater.GetMovieToday();
    if (movieData == null)
      return;
    Microsoft.Xna.Framework.Rectangle sourceRectForPoster = MovieTheater.GetSourceRectForPoster(movieData.SheetIndex);
    location.temporarySprites.Add(new TemporaryAnimatedSprite()
    {
      texture = Game1.temporaryContent.Load<Texture2D>(movieData.Texture ?? "LooseSprites\\Movies"),
      sourceRect = sourceRectForPoster,
      sourceRectStartingPos = new Vector2((float) sourceRectForPoster.X, (float) sourceRectForPoster.Y),
      animationLength = 1,
      totalNumberOfLoops = 9999,
      interval = 9999f,
      scale = 4f,
      position = new Vector2(x, y),
      layerDepth = 0.01f
    });
  }

  public MovieTheater(string map, string name)
    : base(map, name)
  {
    this.CurrentState = 0;
    MovieTheater.GetMovieData();
    this._InitializeMap();
    MovieTheater.GetMovieReactions();
  }

  public static List<MovieCharacterReaction> GetMovieReactions()
  {
    if (MovieTheater._genericReactions == null)
      MovieTheater._genericReactions = DataLoader.MoviesReactions(Game1.content);
    return MovieTheater._genericReactions;
  }

  public static string GetConcessionTasteForCharacter(
    Character character,
    MovieConcession concession)
  {
    if (MovieTheater._concessionTastes == null)
      MovieTheater._concessionTastes = DataLoader.ConcessionTastes(Game1.content);
    ConcessionTaste concessionTaste1 = (ConcessionTaste) null;
    foreach (ConcessionTaste concessionTaste2 in MovieTheater._concessionTastes)
    {
      if (concessionTaste2.Name == "*")
      {
        concessionTaste1 = concessionTaste2;
        break;
      }
    }
    foreach (ConcessionTaste concessionTaste3 in MovieTheater._concessionTastes)
    {
      if (concessionTaste3.Name == character.Name)
      {
        if (concessionTaste3.LovedTags.Contains(concession.Name))
          return "love";
        if (concessionTaste3.LikedTags.Contains(concession.Name))
          return "like";
        if (concessionTaste3.DislikedTags.Contains(concession.Name))
          return "dislike";
        if (concessionTaste1 != null)
        {
          if (concessionTaste1.LovedTags.Contains(concession.Name))
            return "love";
          if (concessionTaste1.LikedTags.Contains(concession.Name))
            return "like";
          if (concessionTaste1.DislikedTags.Contains(concession.Name))
            return "dislike";
        }
        if (concession.Tags != null)
        {
          using (List<string>.Enumerator enumerator = concession.Tags.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              string current = enumerator.Current;
              if (concessionTaste3.LovedTags.Contains(current))
                return "love";
              if (concessionTaste3.LikedTags.Contains(current))
                return "like";
              if (concessionTaste3.DislikedTags.Contains(current))
                return "dislike";
              if (concessionTaste1 != null)
              {
                if (concessionTaste1.LovedTags.Contains(current))
                  return "love";
                if (concessionTaste1.LikedTags.Contains(current))
                  return "like";
                if (concessionTaste1.DislikedTags.Contains(current))
                  return "dislike";
              }
            }
            break;
          }
        }
        break;
      }
    }
    return "like";
  }

  public static IEnumerable<string> GetPatronNames()
  {
    return (IEnumerable<string>) (Game1.getLocationFromName(nameof (MovieTheater)) is MovieTheater locationFromName ? locationFromName._spawnedMoviePatrons?.Keys : new NetDictionary<string, int, NetInt, SerializableDictionary<string, int>, NetStringDictionary<int, NetInt>>.KeysCollection?());
  }

  protected void _InitializeMap()
  {
    this._hangoutPoints = new Dictionary<int, List<Point>>();
    this._maxHangoutGroups = 0;
    Layer layer = this.map.GetLayer("Paths");
    if (layer != null)
    {
      for (int x = 0; x < layer.LayerWidth; ++x)
      {
        for (int y = 0; y < layer.LayerHeight; ++y)
        {
          string s;
          int result;
          if (layer.Tiles[x, y] != null && layer.GetTileIndexAt(x, y) == 7 && layer.Tiles[x, y].Properties.TryGetValue("group", out s) && int.TryParse(s, out result))
          {
            List<Point> pointList;
            if (!this._hangoutPoints.TryGetValue(result, out pointList))
              this._hangoutPoints[result] = pointList = new List<Point>();
            pointList.Add(new Point(x, y));
            this._maxHangoutGroups = Math.Max(this._maxHangoutGroups, result);
          }
        }
      }
    }
    this.ResetTheater();
  }

  protected override void initNetFields()
  {
    base.initNetFields();
    this.NetFields.AddField((INetSerializable) this._spawnedMoviePatrons, "_spawnedMoviePatrons").AddField((INetSerializable) this._purchasedConcessions, "_purchasedConcessions").AddField((INetSerializable) this.currentState, "currentState").AddField((INetSerializable) this.showingId, "showingId").AddField((INetSerializable) this.movieViewerLockEvent, "movieViewerLockEvent").AddField((INetSerializable) this.requestStartMovieEvent, "requestStartMovieEvent").AddField((INetSerializable) this.startMovieEvent, "startMovieEvent").AddField((INetSerializable) this.endMovieEvent, "endMovieEvent").AddField((INetSerializable) this._playerInvitedPatrons, "_playerInvitedPatrons").AddField((INetSerializable) this._characterGroupLookup, "_characterGroupLookup").AddField((INetSerializable) this.dayFirstEntered, "dayFirstEntered");
    this.movieViewerLockEvent.onEvent += new AbstractNetEvent1<MovieViewerLockEvent>.Event(this.OnMovieViewerLockEvent);
    this.requestStartMovieEvent.onEvent += new AbstractNetEvent1<long>.Event(this.OnRequestStartMovieEvent);
    this.startMovieEvent.onEvent += new AbstractNetEvent1<StartMovieEvent>.Event(this.OnStartMovieEvent);
  }

  public void OnStartMovieEvent(StartMovieEvent e)
  {
    if (e.uid != Game1.player.UniqueMultiplayerID)
      return;
    if (Game1.activeClickableMenu is ReadyCheckDialog activeClickableMenu)
      activeClickableMenu.closeDialog(Game1.player);
    StardewValley.Event viewing_event = new MovieTheaterScreeningEvent().getMovieEvent(MovieTheater.GetMovieToday().Id, e.playerGroups, e.npcGroups, this.GetConcessionsDictionary());
    Rumble.rumble(0.15f, 200f);
    Game1.player.completelyStopAnimatingOrDoingAction();
    this.playSound("doorClose", new Vector2?(Game1.player.Tile));
    Game1.globalFadeToBlack((Game1.afterFadeFunction) (() =>
    {
      Game1.changeMusicTrack("none");
      this.startEvent(viewing_event);
    }));
  }

  public void OnRequestStartMovieEvent(long uid)
  {
    if (!Game1.IsMasterGame)
      return;
    if (this.CurrentState == 0)
    {
      if (Game1.player.team.movieMutex.IsLocked())
        Game1.player.team.movieMutex.ReleaseLock();
      Game1.player.team.movieMutex.RequestLock();
      this._playerGroups = new List<List<Character>>();
      this._npcGroups = new List<List<Character>>();
      List<Character> collection = new List<Character>();
      foreach (string patronName in MovieTheater.GetPatronNames())
      {
        Character characterFromName = (Character) Game1.getCharacterFromName(patronName);
        collection.Add(characterFromName);
      }
      foreach (Farmer viewingFarmer in this._viewingFarmers)
      {
        List<Character> characterList = new List<Character>();
        characterList.Add((Character) viewingFarmer);
        for (int index = 0; index < Game1.player.team.movieInvitations.Count; ++index)
        {
          MovieInvitation movieInvitation = Game1.player.team.movieInvitations[index];
          if (movieInvitation.farmer == viewingFarmer && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == viewingFarmer && collection.Contains((Character) movieInvitation.invitedNPC))
          {
            collection.Remove((Character) movieInvitation.invitedNPC);
            characterList.Add((Character) movieInvitation.invitedNPC);
          }
        }
        this._playerGroups.Add(characterList);
      }
      foreach (List<Character> playerGroup in this._playerGroups)
      {
        foreach (Character character in playerGroup)
        {
          if (character is NPC npc)
            npc.lastSeenMovieWeek.Set(Game1.Date.TotalWeeks);
        }
      }
      this._npcGroups.Add(new List<Character>((IEnumerable<Character>) collection));
      this._PopulateNPCOnlyGroups(this._playerGroups, this._npcGroups);
      this._viewingGroups = new List<List<Character>>();
      List<Character> characterList1 = new List<Character>();
      foreach (List<Character> playerGroup in this._playerGroups)
      {
        foreach (Character character in playerGroup)
          characterList1.Add(character);
      }
      this._viewingGroups.Add(characterList1);
      foreach (IEnumerable<Character> npcGroup in this._npcGroups)
        this._viewingGroups.Add(new List<Character>(npcGroup));
      this.CurrentState = 1;
    }
    this.startMovieEvent.Fire(new StartMovieEvent(uid, this._playerGroups, this._npcGroups));
  }

  public void OnMovieViewerLockEvent(MovieViewerLockEvent e)
  {
    this._viewingFarmers = new List<Farmer>();
    this._movieStartTime = e.movieStartTime;
    foreach (long uid in e.uids)
    {
      Farmer player = Game1.GetPlayer(uid, true);
      if (player != null)
        this._viewingFarmers.Add(player);
    }
    if (this._viewingFarmers.Count > 0 && Game1.IsMultiplayer)
      Game1.showGlobalMessage(Game1.content.LoadString("Strings\\UI:MovieStartRequest"));
    if (!Game1.player.team.movieMutex.IsLockHeld())
      return;
    this._ShowMovieStartReady();
  }

  public void _ShowMovieStartReady()
  {
    if (!Game1.IsMultiplayer)
    {
      this.requestStartMovieEvent.Fire(Game1.player.UniqueMultiplayerID);
    }
    else
    {
      string str = $"start_movie_{this.ShowingId}";
      Game1.netReady.SetLocalRequiredFarmers(str, this._viewingFarmers);
      Game1.netReady.SetLocalReady(str, true);
      Game1.dialogueUp = false;
      MovieTheater._hasRequestedMovieStart = true;
      Game1.activeClickableMenu = (IClickableMenu) new ReadyCheckDialog(str, true, (ConfirmationDialog.behavior) (farmer =>
      {
        if (!MovieTheater._hasRequestedMovieStart)
          return;
        MovieTheater._hasRequestedMovieStart = false;
        this.requestStartMovieEvent.Fire(farmer.UniqueMultiplayerID);
      }), (ConfirmationDialog.behavior) (farmer =>
      {
        if (Game1.activeClickableMenu is ReadyCheckDialog)
          (Game1.activeClickableMenu as ReadyCheckDialog).closeDialog(farmer);
        if (!Game1.player.team.movieMutex.IsLockHeld())
          return;
        Game1.player.team.movieMutex.ReleaseLock();
      }));
    }
  }

  /// <summary>Get the data for all movies.</summary>
  public static List<MovieData> GetMovieData()
  {
    if (MovieTheater._movieData == null)
    {
      MovieTheater._movieData = new List<MovieData>();
      MovieTheater._movieDataById = new Dictionary<string, MovieData>();
      foreach (MovieData movie in DataLoader.Movies(Game1.content))
      {
        if (string.IsNullOrWhiteSpace(movie.Id))
          Game1.log.Warn("Ignored movie with no ID.");
        else if (!MovieTheater._movieDataById.TryAdd(movie.Id, movie))
          Game1.log.Warn($"Ignored duplicate movie with ID '{movie.Id}'.");
        else
          MovieTheater._movieData.Add(movie);
      }
    }
    return MovieTheater._movieData;
  }

  /// <summary>Get the data for all movies by ID.</summary>
  public static Dictionary<string, MovieData> GetMovieDataById()
  {
    if (MovieTheater._movieDataById == null)
      MovieTheater.GetMovieData();
    return MovieTheater._movieDataById;
  }

  /// <summary>Get the data for a specific movie, if it exists.</summary>
  /// <param name="id">The movie ID in <c>Data/Movies</c>.</param>
  /// <param name="data">The movie data, if found.</param>
  /// <returns>Returns whether the movie data was found.</returns>
  public static bool TryGetMovieData(string id, out MovieData data)
  {
    if (id != null)
      return MovieTheater.GetMovieDataById().TryGetValue(id, out data);
    data = (MovieData) null;
    return false;
  }

  /// <summary>Get the movie ID corresponding to a pre-1.6 movie index.</summary>
  /// <param name="id">The movie index.</param>
  public static string GetMovieIdFromLegacyIndex(string id)
  {
    int result;
    if (int.TryParse(id, out result))
    {
      foreach (MovieData movieData in MovieTheater.GetMovieData())
      {
        if (movieData.SheetIndex == result && (string.IsNullOrWhiteSpace(movieData.Texture) || movieData.Texture == "LooseSprites\\Movies"))
          return movieData.Id;
      }
    }
    return id;
  }

  /// <summary>Get the pixel area in a movie's spritesheet which contains a screen frame.</summary>
  /// <param name="movieIndex">The movie's sprite index in its spritesheet.</param>
  /// <param name="frame">The screen index within the movie's area.</param>
  public static Microsoft.Xna.Framework.Rectangle GetSourceRectForScreen(int movieIndex, int frame)
  {
    int y = movieIndex * 128 /*0x80*/ + frame / 5 * 64 /*0x40*/;
    return new Microsoft.Xna.Framework.Rectangle(16 /*0x10*/ + frame % 5 * 96 /*0x60*/, y, 90, 61);
  }

  /// <summary>Get the pixel area in a movie's spritesheet which contains a screen frame.</summary>
  /// <param name="movieIndex">The movie's sprite index in its spritesheet.</param>
  public static Microsoft.Xna.Framework.Rectangle GetSourceRectForPoster(int movieIndex)
  {
    return new Microsoft.Xna.Framework.Rectangle(0, movieIndex * 128 /*0x80*/, 13, 19);
  }

  public NPC GetMoviePatron(string name)
  {
    for (int index = 0; index < this.characters.Count; ++index)
    {
      if (this.characters[index].name.Value == name)
        return this.characters[index];
    }
    return (NPC) null;
  }

  protected NPC AddMoviePatronNPC(string name, int x, int y, int facingDirection)
  {
    if (this._spawnedMoviePatrons.ContainsKey(name))
      return this.GetMoviePatron(name);
    string nameForCharacter = NPC.getTextureNameForCharacter(name);
    CharacterData data;
    NPC.TryGetData(name, out data);
    int spriteWidth = data != null ? data.Size.X : 16 /*0x10*/;
    int spriteHeight = data != null ? data.Size.Y : 32 /*0x20*/;
    NPC character = new NPC(new AnimatedSprite("Characters\\" + nameForCharacter, 0, spriteWidth, spriteHeight), new Vector2((float) (x * 64 /*0x40*/), (float) (y * 64 /*0x40*/)), this.Name, facingDirection, name, (Texture2D) null, true);
    character.EventActor = true;
    character.collidesWithOtherCharacters.Set(false);
    this.addCharacter(character);
    this._spawnedMoviePatrons.Add(name, 1);
    this.GetDialogueForCharacter(character);
    return character;
  }

  public void RemoveAllPatrons()
  {
    if (this._spawnedMoviePatrons == null)
      return;
    this.characters.RemoveWhere((Func<NPC, bool>) (npc => this._spawnedMoviePatrons.ContainsKey(npc.Name)));
    this._spawnedMoviePatrons.Clear();
  }

  protected override void resetSharedState()
  {
    base.resetSharedState();
    if (this.CurrentState != 0)
      return;
    MovieData movieToday = MovieTheater.GetMovieToday();
    Game1.multiplayer.globalChatInfoMessage("MovieStart", TokenStringBuilder.MovieName(movieToday.Id));
  }

  protected override void resetLocalState()
  {
    base.resetLocalState();
    Game1.getAchievement(36);
    this.birds = new PerchingBirds(Game1.birdsSpriteSheet, 2, 16 /*0x10*/, 16 /*0x10*/, new Vector2(8f, 14f), new Point[14]
    {
      new Point(19, 5),
      new Point(21, 4),
      new Point(16 /*0x10*/, 3),
      new Point(10, 13),
      new Point(2, 13),
      new Point(2, 6),
      new Point(9, 2),
      new Point(18, 12),
      new Point(21, 11),
      new Point(3, 11),
      new Point(4, 2),
      new Point(12, 12),
      new Point(11, 5),
      new Point(13, 13)
    }, new Point[6]
    {
      new Point(19, 5),
      new Point(21, 4),
      new Point(16 /*0x10*/, 3),
      new Point(9, 2),
      new Point(21, 11),
      new Point(4, 2)
    });
    if (!MovieTheater._isJojaTheater && Game1.MasterPlayer.mailReceived.Contains("ccMovieTheaterJoja"))
      MovieTheater._isJojaTheater = true;
    if (this.dayFirstEntered.Value == -1)
      this.dayFirstEntered.Value = Game1.Date.TotalDays;
    if (!MovieTheater._isJojaTheater)
    {
      this.birds.roosting = this.CurrentState == 2;
      for (int index = 0; index < Game1.random.Next(2, 5); ++index)
      {
        int bird_type = Game1.random.Next(0, 4);
        if (this.IsFallHere())
          bird_type = 10;
        this.birds.AddBird(bird_type);
      }
      if (Game1.timeOfDay > 2100 && Game1.random.NextBool())
        this.birds.AddBird(11);
    }
    MovieTheater.AddMoviePoster((GameLocation) this, 1104f, 292f);
    this.loadMap(this.mapPath.Value, true);
    if (MovieTheater._isJojaTheater)
    {
      string str = LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en ? "" : "_international";
      if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru)
        str = ".ru-RU";
      this.Map.RequireTileSheet(0, "movieTheater_tileSheet").ImageSource = "Maps\\MovieTheaterJoja_TileSheet" + str;
      this.Map.LoadTileSheets(Game1.mapDisplayDevice);
    }
    switch (this.CurrentState)
    {
      case 0:
        this.addRandomNPCs();
        break;
      case 2:
        Game1.changeMusicTrack("movieTheaterAfter");
        Game1.ambientLight = new Color(150, 170, 80 /*0x50*/);
        this.addSpecificRandomNPC(0);
        break;
    }
  }

  private void addRandomNPCs()
  {
    Season season = this.GetSeason();
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.Date.TotalDays);
    this.critters = new List<Critter>();
    if (this.dayFirstEntered.Value == Game1.Date.TotalDays || random.NextDouble() < 0.25)
      this.addSpecificRandomNPC(0);
    if (!MovieTheater._isJojaTheater && random.NextDouble() < 0.28)
    {
      this.addSpecificRandomNPC(4);
      this.addSpecificRandomNPC(11);
    }
    else if (MovieTheater._isJojaTheater && random.NextDouble() < 0.33)
      this.addSpecificRandomNPC(13);
    if (random.NextDouble() < 0.1)
    {
      this.addSpecificRandomNPC(9);
      this.addSpecificRandomNPC(7);
    }
    switch (season)
    {
      case Season.Spring:
        if (random.NextBool())
        {
          this.addSpecificRandomNPC(3);
          break;
        }
        break;
      case Season.Fall:
        if (random.NextBool())
        {
          this.addSpecificRandomNPC(1);
          break;
        }
        break;
    }
    if (random.NextDouble() < 0.25)
      this.addSpecificRandomNPC(2);
    if (random.NextDouble() < 0.25)
      this.addSpecificRandomNPC(6);
    if (random.NextDouble() < 0.25)
      this.addSpecificRandomNPC(8);
    if (random.NextDouble() < 0.2)
      this.addSpecificRandomNPC(10);
    if (random.NextDouble() < 0.2)
      this.addSpecificRandomNPC(12);
    if (random.NextDouble() < 0.2)
      this.addSpecificRandomNPC(5);
    if (MovieTheater._isJojaTheater)
      return;
    if (random.NextDouble() < 0.75)
      this.addCritter((Critter) new Butterfly((GameLocation) this, new Vector2(13f, 7f)).setStayInbounds(true));
    if (random.NextDouble() < 0.75)
      this.addCritter((Critter) new Butterfly((GameLocation) this, new Vector2(4f, 8f)).setStayInbounds(true));
    if (random.NextDouble() >= 0.75)
      return;
    this.addCritter((Critter) new Butterfly((GameLocation) this, new Vector2(17f, 10f)).setStayInbounds(true));
  }

  private void addSpecificRandomNPC(int whichRandomNPC)
  {
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.Date.TotalDays, (double) whichRandomNPC);
    switch (whichRandomNPC)
    {
      case 0:
        this.setMapTile(2, 9, 215, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_CraneMan" + random.Choose<string>("2", ""));
        this.setMapTile(2, 8, 199, "Front", "movieTheater_tileSheet");
        break;
      case 1:
        this.setMapTile(19, 7, 216, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_Welwick" + random.Choose<string>("2", ""));
        this.setMapTile(19, 6, 200, "Front", "movieTheater_tileSheet");
        break;
      case 2:
        this.setAnimatedMapTile(21, 7, new int[4]
        {
          217,
          217,
          217,
          218
        }, 700L, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_ShortsMan" + random.Choose<string>("2", ""));
        this.setAnimatedMapTile(21, 6, new int[4]
        {
          201,
          201,
          201,
          202
        }, 700L, "Front", "movieTheater_tileSheet");
        break;
      case 3:
        this.setMapTile(5, 9, 219, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_Mother" + random.Choose<string>("2", ""));
        this.setMapTile(6, 9, 220, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_Child" + random.Choose<string>("2", ""));
        this.setAnimatedMapTile(5, 8, new int[6]
        {
          203,
          203,
          203,
          204,
          204,
          204
        }, 1000L, "Front", "movieTheater_tileSheet");
        break;
      case 4:
        this.setMapTile(20, 9, 222, "Front", "movieTheater_tileSheet");
        this.setMapTile(21, 9, 223, "Front", "movieTheater_tileSheet");
        this.setMapTile(20, 10, 238, "Buildings", "movieTheater_tileSheet");
        this.setMapTile(21, 10, 239, "Buildings", "movieTheater_tileSheet");
        this.setMapTile(20, 11, 254, "Buildings", "movieTheater_tileSheet");
        this.setMapTile(21, 11, (int) byte.MaxValue, "Buildings", "movieTheater_tileSheet");
        break;
      case 5:
        this.setAnimatedMapTile(10, 7, new int[4]
        {
          251,
          251,
          251,
          252
        }, 900L, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_Lupini" + random.Choose<string>("2", ""));
        this.setAnimatedMapTile(10, 6, new int[4]
        {
          235,
          235,
          235,
          236
        }, 900L, "Front", "movieTheater_tileSheet");
        break;
      case 6:
        this.setAnimatedMapTile(5, 7, new int[4]
        {
          249,
          249,
          249,
          250
        }, 600L, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_ConcessionMan" + random.Choose<string>("2", ""));
        this.setAnimatedMapTile(5, 6, new int[4]
        {
          233,
          233,
          233,
          234
        }, 600L, "Front", "movieTheater_tileSheet");
        break;
      case 7:
        this.setMapTile(1, 12, 248, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_PurpleHairLady");
        this.setMapTile(1, 11, 232, "Front", "movieTheater_tileSheet");
        break;
      case 8:
        this.setMapTile(3, 8, 247, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_RedCapGuy" + random.Choose<string>("2", ""));
        this.setMapTile(3, 7, 231, "Front", "movieTheater_tileSheet");
        break;
      case 9:
        this.setMapTile(2, 11, 253, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_Governor" + random.Choose<string>("2", ""));
        this.setMapTile(2, 10, 237, "Front", "movieTheater_tileSheet");
        break;
      case 10:
        this.setMapTile(9, 7, 221, "Buildings", "movieTheater_tileSheet", "NPCSpeechMessageNoRadius Gunther MovieTheater_Gunther" + random.Choose<string>("2", ""));
        this.setMapTile(9, 6, 205, "Front", "movieTheater_tileSheet");
        break;
      case 11:
        this.setMapTile(19, 10, 208 /*0xD0*/, "Buildings", "movieTheater_tileSheet", "NPCSpeechMessageNoRadius Marlon MovieTheater_Marlon" + random.Choose<string>("2", ""));
        this.setMapTile(19, 9, 192 /*0xC0*/, "Front", "movieTheater_tileSheet");
        break;
      case 12:
        this.setMapTile(12, 4, 209, "Buildings", "movieTheater_tileSheet", "MessageSpeech MovieTheater_Marcello" + random.Choose<string>("2", ""));
        this.setMapTile(12, 3, 193, "Front", "movieTheater_tileSheet");
        break;
      case 13:
        this.setMapTile(17, 12, 241, "Buildings", "movieTheater_tileSheet", "NPCSpeechMessageNoRadius Morris MovieTheater_Morris" + random.Choose<string>("2", ""));
        this.setMapTile(17, 11, 225, "Front", "movieTheater_tileSheet");
        break;
    }
  }

  /// <summary>Get the movie that plays today.</summary>
  public static MovieData GetMovieToday()
  {
    if (MovieTheater.forceMovieId != null)
    {
      MovieData data;
      if (MovieTheater.TryGetMovieData(MovieTheater.forceMovieId, out data))
        return data;
      Game1.log.Warn($"Ignored invalid {nameof (MovieTheater)}.{"forceMovieId"} override '{MovieTheater.forceMovieId}'.");
      MovieTheater.forceMovieId = (string) null;
    }
    return MovieTheater.GetMovieForDate(Game1.Date);
  }

  /// <summary>Get the movies that play in a given season.</summary>
  /// <param name="date">The date whose season and year to check.</param>
  public static List<MovieData> GetMoviesForSeason(WorldDate date)
  {
    WorldDate worldDate = WorldDate.ForDaysPlayed((int) Game1.player.team.theaterBuildDate.Value);
    int year = date.Year - worldDate.Year;
    List<MovieData> movieData = MovieTheater.GetMovieData();
    List<MovieData> list = new List<MovieData>();
    foreach (MovieData movie in movieData)
    {
      if (MovieTheater.MovieSeasonMatches(movie, date.Season) && MovieTheater.MovieYearMatches(movie, year))
        list.Add(movie);
    }
    if (list.Count == 0)
    {
      foreach (MovieData movie in movieData)
      {
        if (MovieTheater.MovieSeasonMatches(movie, date.Season))
          list.Add(movie);
      }
    }
    if (list.Count == 0)
      list.AddRange((IEnumerable<MovieData>) movieData);
    if (list.Count > 28)
    {
      Utility.Shuffle<MovieData>(Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.season, (double) Game1.year), list);
      list.RemoveRange(28, list.Count - 28);
    }
    return list;
  }

  /// <summary>Get the movie that plays on the given date.</summary>
  /// <param name="date">The date to check.</param>
  public static MovieData GetMovieForDate(WorldDate date)
  {
    List<MovieData> movieDataList = MovieTheater.GetMoviesForSeason(date);
    if (movieDataList.Count == 0)
    {
      Game1.log.Warn($"There are no available movies for {date}. Defaulting to all movies.");
      movieDataList = MovieTheater.GetMovieData();
    }
    float num = 28f / (float) movieDataList.Count;
    int index = ((int) Math.Ceiling((double) date.DayOfMonth / (double) num) - 1) % movieDataList.Count;
    return movieDataList[index];
  }

  /// <summary>Get the next different movie that will play after today.</summary>
  public static MovieData GetUpcomingMovie() => MovieTheater.GetUpcomingMovieForDate(Game1.Date);

  /// <summary>Get the next different movie that will play after the given date.</summary>
  /// <param name="afterDate">The date of the current movie for which to get the upcoming movie.</param>
  public static MovieData GetUpcomingMovieForDate(WorldDate afterDate)
  {
    List<MovieData> moviesForSeason1 = MovieTheater.GetMoviesForSeason(afterDate);
    MovieData movieForDate = MovieTheater.GetMovieForDate(afterDate);
    bool flag = false;
    foreach (MovieData upcomingMovieForDate in moviesForSeason1)
    {
      if (upcomingMovieForDate.Id == movieForDate.Id)
        flag = true;
      else if (flag)
        return upcomingMovieForDate;
    }
    List<MovieData> moviesForSeason2 = MovieTheater.GetMoviesForSeason(WorldDate.ForDaysPlayed(afterDate.TotalDays + 28));
    foreach (MovieData upcomingMovieForDate in moviesForSeason2)
    {
      if (upcomingMovieForDate.Id != movieForDate.Id)
        return upcomingMovieForDate;
    }
    return moviesForSeason2[0];
  }

  /// <summary>Get whether a movie should play in a given year.</summary>
  /// <param name="movie">The movie data to check.</param>
  /// <param name="year">The relative year when the movie theater was built (e.g. 0 if built this year).</param>
  public static bool MovieYearMatches(MovieData movie, int year)
  {
    if (!movie.YearModulus.HasValue)
      return true;
    int num = movie.YearModulus.Value;
    int valueOrDefault = movie.YearRemainder.GetValueOrDefault();
    if (num >= 1)
      return year % num == valueOrDefault;
    Game1.log.Warn($"Movie '{movie.Id}' has invalid year modulus {movie.YearModulus}, must be a number greater than zero.");
    return false;
  }

  /// <summary>Get whether a movie should play in a given season.</summary>
  /// <param name="movie">The movie data to check.</param>
  /// <param name="season">The calendar season.</param>
  public static bool MovieSeasonMatches(MovieData movie, Season season)
  {
    List<Season> seasons = movie.Seasons;
    // ISSUE: explicit non-virtual call
    return (seasons != null ? (__nonvirtual (seasons.Count) > 0 ? 1 : 0) : 0) == 0 || movie.Seasons.Contains(season);
  }

  public override void DayUpdate(int dayOfMonth)
  {
    this.ShowingId = 0;
    this.ResetTheater();
    this._ResetHangoutPoints();
    base.DayUpdate(dayOfMonth);
  }

  public override void UpdateWhenCurrentLocation(GameTime time)
  {
    if (this._farmerCount != this.farmers.Count)
    {
      this._farmerCount = this.farmers.Count;
      if (Game1.activeClickableMenu is ReadyCheckDialog activeClickableMenu)
      {
        activeClickableMenu.closeDialog(Game1.player);
        if (Game1.player.team.movieMutex.IsLockHeld())
          Game1.player.team.movieMutex.ReleaseLock();
      }
    }
    this.birds?.Update(time);
    base.UpdateWhenCurrentLocation(time);
  }

  public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
  {
    this.birds?.Draw(b);
    base.drawAboveAlwaysFrontLayer(b);
  }

  public static bool Invite(Farmer farmer, NPC invited_npc)
  {
    if (farmer == null || invited_npc == null)
      return false;
    farmer.team.movieInvitations.Add(new MovieInvitation()
    {
      farmer = farmer,
      invitedNPC = invited_npc
    });
    return true;
  }

  public void ResetTheater()
  {
    MovieTheater._playerHangoutGroup = -1;
    this.RemoveAllPatrons();
    this._playerGroups.Clear();
    this._npcGroups.Clear();
    this._viewingGroups.Clear();
    this._viewingFarmers.Clear();
    this._purchasedConcessions.Clear();
    this._playerInvitedPatrons.Clear();
    this._characterGroupLookup.Clear();
    this._ResetHangoutPoints();
    Game1.player.team.movieMutex.ReleaseLock();
    this.CurrentState = 0;
  }

  public override void updateEvenIfFarmerIsntHere(GameTime time, bool ignoreWasUpdatedFlush = false)
  {
    base.updateEvenIfFarmerIsntHere(time, ignoreWasUpdatedFlush);
    this.movieViewerLockEvent.Poll();
    this.requestStartMovieEvent.Poll();
    this.startMovieEvent.Poll();
    this.endMovieEvent.Poll();
    if (!Game1.IsMasterGame)
      return;
    for (int index = 0; index < this._viewingFarmers.Count; ++index)
    {
      Farmer viewingFarmer = this._viewingFarmers[index];
      if (!Game1.getOnlineFarmers().Contains(viewingFarmer))
      {
        this._viewingFarmers.RemoveAt(index);
        --index;
      }
      else if (this.CurrentState == 2 && !this.farmers.Contains(viewingFarmer) && !this.HasFarmerWatchingBroadcastEventReturningHere() && viewingFarmer.currentLocation != null && !viewingFarmer.currentLocation.IsTemporary)
      {
        this._viewingFarmers.RemoveAt(index);
        --index;
      }
    }
    if (this.CurrentState != 0 && this._viewingFarmers.Count == 0)
    {
      MovieData movieToday = MovieTheater.GetMovieToday();
      Game1.multiplayer.globalChatInfoMessage("MovieEnd", TokenStringBuilder.MovieName(movieToday.Id));
      this.ResetTheater();
      ++this.ShowingId;
    }
    if (Game1.player.team.movieInvitations == null || this._playerInvitedPatrons.Count() >= 8)
      return;
    foreach (Farmer farmer in this.farmers)
    {
      for (int index = 0; index < Game1.player.team.movieInvitations.Count; ++index)
      {
        MovieInvitation movieInvitation = Game1.player.team.movieInvitations[index];
        if (!movieInvitation.fulfilled && !this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.displayName))
        {
          if (MovieTheater._playerHangoutGroup < 0)
            MovieTheater._playerHangoutGroup = Game1.random.Next(this._maxHangoutGroups);
          int key = MovieTheater._playerHangoutGroup;
          if (movieInvitation.farmer == farmer && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == farmer)
          {
            while (this._availableHangoutPoints[key].Count == 0)
              key = Game1.random.Next(this._maxHangoutGroups);
            Point point = Game1.random.ChooseFrom<Point>((IList<Point>) this._availableHangoutPoints[key]);
            NPC character = this.AddMoviePatronNPC(movieInvitation.invitedNPC.name.Value, 14, 15, 0);
            this._playerInvitedPatrons.Add(character.name.Value, 1);
            this._availableHangoutPoints[key].Remove(point);
            int result = 2;
            IPropertyCollection properties = this.map.GetLayer("Paths").Tiles[point.X, point.Y].Properties;
            string s;
            if (properties != null && properties.TryGetValue("direction", out s))
              int.TryParse(s, out result);
            this._destinationPositions[character.Name] = new KeyValuePair<Point, int>(point, result);
            this.PathCharacterToLocation(character, point, result);
            movieInvitation.fulfilled = true;
          }
        }
      }
    }
  }

  public static MovieCharacterReaction GetReactionsForCharacter(NPC character)
  {
    if (character == null)
      return (MovieCharacterReaction) null;
    foreach (MovieCharacterReaction movieReaction in MovieTheater.GetMovieReactions())
    {
      if (!(movieReaction.NPCName != character.Name))
        return movieReaction;
    }
    return (MovieCharacterReaction) null;
  }

  /// <inheritdoc />
  public override void checkForMusic(GameTime time)
  {
  }

  public static string GetResponseForMovie(NPC character)
  {
    string responseForMovie = "like";
    MovieData movieToday = MovieTheater.GetMovieToday();
    if (movieToday == null)
      return (string) null;
    if (movieToday != null)
    {
      foreach (MovieCharacterReaction movieReaction in MovieTheater.GetMovieReactions())
      {
        if (!(movieReaction.NPCName != character.Name))
        {
          foreach (MovieReaction reaction in movieReaction.Reactions)
          {
            if (reaction.ShouldApplyToMovie(movieToday, MovieTheater.GetPatronNames()))
            {
              string response = reaction.Response;
              if ((response != null ? (response.Length > 0 ? 1 : 0) : 0) != 0)
              {
                responseForMovie = reaction.Response;
                break;
              }
            }
          }
        }
      }
    }
    return responseForMovie;
  }

  public Dialogue GetDialogueForCharacter(NPC character)
  {
    MovieData movieToday = MovieTheater.GetMovieToday();
    if (movieToday != null)
    {
      foreach (MovieCharacterReaction genericReaction in MovieTheater._genericReactions)
      {
        if (!(genericReaction.NPCName != character.Name))
        {
          using (List<MovieReaction>.Enumerator enumerator = genericReaction.Reactions.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              MovieReaction current = enumerator.Current;
              if (current.ShouldApplyToMovie(movieToday, MovieTheater.GetPatronNames(), MovieTheater.GetResponseForMovie(character)))
              {
                string response = current.Response;
                if ((response != null ? (response.Length > 0 ? 1 : 0) : 0) != 0 && current.SpecialResponses != null)
                {
                  switch (this.CurrentState)
                  {
                    case 0:
                      if (current.SpecialResponses.BeforeMovie != null)
                        return new Dialogue(character, (string) null, this.FormatString(current.SpecialResponses.BeforeMovie.Text));
                      goto label_19;
                    case 1:
                      if (current.SpecialResponses.DuringMovie != null)
                        return new Dialogue(character, (string) null, this.FormatString(current.SpecialResponses.DuringMovie.Text));
                      goto label_19;
                    case 2:
                      if (current.SpecialResponses.AfterMovie != null)
                        return new Dialogue(character, (string) null, this.FormatString(current.SpecialResponses.AfterMovie.Text));
                      goto label_19;
                    default:
                      goto label_19;
                  }
                }
              }
            }
            break;
          }
        }
      }
    }
label_19:
    return (Dialogue) null;
  }

  public string FormatString(string text, params string[] args)
  {
    text = TokenParser.ParseText(text);
    string text1 = TokenParser.ParseText(MovieTheater.GetMovieToday().Title);
    return string.Format(text, (object) text1, (object) Game1.player.displayName, (object) args);
  }

  public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
  {
    Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64 /*0x40*/, tileLocation.Y * 64 /*0x40*/, 64 /*0x40*/, 64 /*0x40*/);
    string[] propertySplitBySpaces = this.GetTilePropertySplitBySpaces("Action", "Buildings", tileLocation.X, tileLocation.Y);
    if (propertySplitBySpaces.Length != 0)
      return this.performAction(propertySplitBySpaces, who, tileLocation);
    foreach (NPC character in this.characters)
    {
      if (character != null && !character.IsMonster && (!who.isRidingHorse() || !(character is Horse)) && character.GetBoundingBox().Intersects(rectangle))
      {
        if (!character.isMoving())
        {
          if (this._playerInvitedPatrons.ContainsKey(character.Name))
          {
            character.faceTowardFarmerForPeriod(5000, 4, false, who);
            Dialogue dialogueForCharacter = this.GetDialogueForCharacter(character);
            if (dialogueForCharacter != null)
            {
              character.CurrentDialogue.Push(dialogueForCharacter);
              Game1.drawDialogue(character);
              character.grantConversationFriendship(Game1.player);
            }
          }
          else
          {
            bool flag;
            if (this._characterGroupLookup.TryGetValue(character.Name, out flag))
            {
              if (!flag)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AfterMovieAlone", (object) character.displayName));
              else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_AfterMovie", (object) character.displayName));
            }
          }
        }
        return true;
      }
    }
    return base.checkAction(tileLocation, viewport, who);
  }

  protected void _PopulateNPCOnlyGroups(
    List<List<Character>> player_groups,
    List<List<Character>> groups)
  {
    HashSet<string> stringSet = new HashSet<string>();
    foreach (List<Character> playerGroup in player_groups)
    {
      foreach (Character character in playerGroup)
      {
        if (character is NPC)
          stringSet.Add(character.name.Value);
      }
    }
    foreach (List<Character> group in groups)
    {
      foreach (Character character in group)
      {
        if (character is NPC)
          stringSet.Add(character.name.Value);
      }
    }
    Random random = Utility.CreateRandom((double) Game1.uniqueIDForThisGame, (double) Game1.Date.TotalDays);
    int num = 0;
    for (int index = 0; index < 2; ++index)
    {
      if (random.NextDouble() < 0.75)
        ++num;
    }
    int index1 = 0;
    if (this._movieStartTime >= 1200)
      index1 = 1;
    if (this._movieStartTime >= 1800)
      index1 = 2;
    string[][] strArray1 = MovieTheater.possibleNPCGroups[(int) Game1.Date.DayOfWeek][index1];
    if (strArray1 == null)
      return;
    if (groups.Count > 0 && groups[0].Count == 0)
      groups.RemoveAt(0);
    for (int index2 = 0; index2 < num && groups.Count < 2; ++index2)
    {
      string[] strArray2 = random.Choose<string[]>(strArray1);
      bool flag1 = true;
      foreach (string str in strArray2)
      {
        bool flag2 = false;
        foreach (Farmer allFarmer in Game1.getAllFarmers())
        {
          if (allFarmer.friendshipData.ContainsKey(str))
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
        {
          flag1 = false;
          break;
        }
        if (stringSet.Contains(str))
        {
          flag1 = false;
          break;
        }
        if (MovieTheater.GetResponseForMovie(Game1.getCharacterFromName(str)) == "dislike" || MovieTheater.GetResponseForMovie(Game1.getCharacterFromName(str)) == "reject")
        {
          flag1 = false;
          break;
        }
      }
      if (flag1)
      {
        List<Character> characterList = new List<Character>();
        foreach (string str in strArray2)
        {
          NPC npc = this.AddMoviePatronNPC(str, 1000, 1000, 2);
          characterList.Add((Character) npc);
          stringSet.Add(str);
          this._characterGroupLookup[str] = strArray2.Length > 1;
        }
        groups.Add(characterList);
      }
    }
  }

  public Dictionary<Character, MovieConcession> GetConcessionsDictionary()
  {
    Dictionary<Character, MovieConcession> concessionsDictionary = new Dictionary<Character, MovieConcession>();
    foreach (string key in this._purchasedConcessions.Keys)
    {
      Character characterFromName = (Character) Game1.getCharacterFromName(key);
      MovieConcession movieConcession;
      if (characterFromName != null && MovieTheater.GetConcessions().TryGetValue(this._purchasedConcessions[key], out movieConcession))
        concessionsDictionary[characterFromName] = movieConcession;
    }
    return concessionsDictionary;
  }

  protected void _ResetHangoutPoints()
  {
    this._destinationPositions.Clear();
    this._availableHangoutPoints = new Dictionary<int, List<Point>>();
    foreach (int key in this._hangoutPoints.Keys)
      this._availableHangoutPoints[key] = new List<Point>((IEnumerable<Point>) this._hangoutPoints[key]);
  }

  public override void cleanupBeforePlayerExit()
  {
    if (!Game1.eventUp)
      Game1.changeMusicTrack("none");
    this.birds = (PerchingBirds) null;
    base.cleanupBeforePlayerExit();
  }

  public void RequestEndMovie(long uid)
  {
    if (!Game1.IsMasterGame)
      return;
    if (this.CurrentState == 1)
    {
      this.CurrentState = 2;
      for (int index1 = 0; index1 < this._viewingGroups.Count; ++index1)
      {
        int index2 = Game1.random.Next(this._viewingGroups.Count);
        List<Character> viewingGroup = this._viewingGroups[index1];
        this._viewingGroups[index1] = this._viewingGroups[index2];
        this._viewingGroups[index2] = viewingGroup;
      }
      this._ResetHangoutPoints();
      int num = 0;
      for (int index3 = 0; index3 < this._viewingGroups.Count; ++index3)
      {
        for (int index4 = 0; index4 < this._viewingGroups[index3].Count; ++index4)
        {
          if (this._viewingGroups[index3][index4] is NPC)
          {
            NPC moviePatron = this.GetMoviePatron(this._viewingGroups[index3][index4].Name);
            if (moviePatron != null)
            {
              moviePatron.setTileLocation(new Vector2(14f, (float) (4.0 + (double) num * 1.0)));
              Point point = Game1.random.ChooseFrom<Point>((IList<Point>) this._availableHangoutPoints[index3]);
              int result;
              if (!int.TryParse(this.doesTileHaveProperty(point.X, point.Y, "direction", "Paths"), out result))
                result = 2;
              this._destinationPositions[moviePatron.Name] = new KeyValuePair<Point, int>(point, result);
              this.PathCharacterToLocation(moviePatron, point, result);
              this._availableHangoutPoints[index3].Remove(point);
              ++num;
            }
          }
        }
      }
    }
    (Game1.GetPlayer(uid, true)?.team ?? Game1.MasterPlayer.team).endMovieEvent.Fire(uid);
  }

  public void PathCharacterToLocation(NPC character, Point point, int direction)
  {
    if (character.currentLocation != this)
      return;
    character.temporaryController = new PathFindController((Character) character, (GameLocation) this, character.TilePoint, direction)
    {
      pathToEndPoint = PathFindController.findPathForNPCSchedules(character.TilePoint, point, (GameLocation) this, 30000, (Character) character)
    };
    character.followSchedule = true;
    character.ignoreScheduleToday = true;
  }

  public static Dictionary<string, MovieConcession> GetConcessions()
  {
    if (MovieTheater._concessions == null)
    {
      MovieTheater._concessions = new Dictionary<string, MovieConcession>();
      foreach (ConcessionItemData concession in DataLoader.Concessions(Game1.content))
        MovieTheater._concessions[concession.Id] = new MovieConcession(concession);
    }
    return MovieTheater._concessions;
  }

  /// <summary>Get a movie concession.</summary>
  /// <param name="id">The concession ID.</param>
  public static MovieConcession GetConcessionItem(string id)
  {
    MovieConcession movieConcession;
    return id == null || !MovieTheater.GetConcessions().TryGetValue(id, out movieConcession) ? (MovieConcession) null : movieConcession;
  }

  /// <summary>Handle a movie concession being bought.</summary>
  /// <inheritdoc cref="T:StardewValley.Menus.ShopMenu.OnPurchaseDelegate" />
  public bool OnPurchaseConcession(
    ISalable salable,
    Farmer who,
    int countTaken,
    ItemStockInformation stock)
  {
    foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
    {
      if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player && this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.Name))
      {
        MovieConcession movieConcession = (MovieConcession) salable;
        this._purchasedConcessions[movieInvitation.invitedNPC.Name] = movieConcession.Id;
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionPurchased", (object) movieConcession.DisplayName, (object) movieInvitation.invitedNPC.displayName));
        return true;
      }
    }
    return false;
  }

  public bool HasInvitedSomeone(Farmer who)
  {
    foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
    {
      if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player && this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.Name))
        return true;
    }
    return false;
  }

  public bool HasPurchasedConcession(Farmer who)
  {
    if (!this.HasInvitedSomeone(who))
      return false;
    foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
    {
      if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
      {
        foreach (string key in this._purchasedConcessions.Keys)
        {
          if (key == movieInvitation.invitedNPC.Name && this._spawnedMoviePatrons.ContainsKey(movieInvitation.invitedNPC.Name))
            return true;
        }
      }
    }
    return false;
  }

  public static Farmer GetFirstInvitedPlayer(NPC npc)
  {
    foreach (MovieInvitation movieInvitation in Game1.player.team.movieInvitations)
    {
      if (movieInvitation.invitedNPC.Name == npc.Name)
        return movieInvitation.farmer;
    }
    return (Farmer) null;
  }

  /// <inheritdoc />
  public override void performTouchAction(string[] action, Vector2 playerStandingPosition)
  {
    if (this.IgnoreTouchActions())
      return;
    if (ArgUtility.Get(action, 0) == "Theater_Exit")
    {
      Point point;
      string error;
      if (!ArgUtility.TryGetPoint(action, 1, out point, out error, "Point exitTile"))
      {
        this.LogTileTouchActionError(action, playerStandingPosition, error);
      }
      else
      {
        Point theaterTileOffset = Town.GetTheaterTileOffset();
        this._exitX = point.X + theaterTileOffset.X;
        this._exitY = point.Y + theaterTileOffset.Y;
        if (Game1.player.lastSeenMovieWeek.Value >= Game1.Date.TotalWeeks)
        {
          this._Leave();
        }
        else
        {
          Game1.player.position.Y -= (float) (((double) Game1.player.Speed + (double) Game1.player.addedSpeed) * 2.0);
          Game1.player.Halt();
          Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_LeavePrompt"), Game1.currentLocation.createYesNoResponses(), "LeaveMovie");
        }
      }
    }
    else
      base.performTouchAction(action, playerStandingPosition);
  }

  public static List<MovieConcession> GetConcessionsForGuest()
  {
    string npc_name = (string) null;
    foreach (MovieInvitation movieInvitation in Game1.player.team.movieInvitations)
    {
      if (movieInvitation.farmer == Game1.player && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
      {
        npc_name = movieInvitation.invitedNPC.Name;
        break;
      }
    }
    return MovieTheater.GetConcessionsForGuest(npc_name);
  }

  public static List<MovieConcession> GetConcessionsForGuest(string npc_name)
  {
    if (npc_name == null)
      npc_name = "Abigail";
    List<MovieConcession> list1 = new List<MovieConcession>();
    List<MovieConcession> list2 = MovieTheater.GetConcessions().Values.ToList<MovieConcession>();
    Random daySaveRandom = Utility.CreateDaySaveRandom();
    Utility.Shuffle<MovieConcession>(daySaveRandom, list2);
    NPC characterFromName = Game1.getCharacterFromName(npc_name);
    if (characterFromName == null)
      return list1;
    int num1 = 1;
    int num2 = 2;
    int num3 = 1;
    int num4 = 5;
    for (int index1 = 0; index1 < num1; ++index1)
    {
      for (int index2 = 0; index2 < list2.Count; ++index2)
      {
        MovieConcession concession = list2[index2];
        if (MovieTheater.GetConcessionTasteForCharacter((Character) characterFromName, concession) == "love" && (!concession.Name.Equals("Stardrop Sorbet") || daySaveRandom.NextDouble() < 0.33))
        {
          list1.Add(concession);
          list2.RemoveAt(index2);
          int num5 = index2 - 1;
          break;
        }
      }
    }
    for (int index3 = 0; index3 < num2; ++index3)
    {
      for (int index4 = 0; index4 < list2.Count; ++index4)
      {
        MovieConcession concession = list2[index4];
        if (MovieTheater.GetConcessionTasteForCharacter((Character) characterFromName, concession) == "like")
        {
          list1.Add(concession);
          list2.RemoveAt(index4);
          int num6 = index4 - 1;
          break;
        }
      }
    }
    for (int index5 = 0; index5 < num3; ++index5)
    {
      for (int index6 = 0; index6 < list2.Count; ++index6)
      {
        MovieConcession concession = list2[index6];
        if (MovieTheater.GetConcessionTasteForCharacter((Character) characterFromName, concession) == "dislike")
        {
          list1.Add(concession);
          list2.RemoveAt(index6);
          int num7 = index6 - 1;
          break;
        }
      }
    }
    for (int count = list1.Count; count < num4; ++count)
    {
      int index = 0;
      if (index < list2.Count)
      {
        MovieConcession movieConcession = list2[index];
        list1.Add(movieConcession);
        list2.RemoveAt(index);
        int num8 = index - 1;
      }
    }
    if (MovieTheater._isJojaTheater && !list1.Exists((Predicate<MovieConcession>) (x => x.Name.Equals("JojaCorn"))))
    {
      MovieConcession movieConcession = list2.Find((Predicate<MovieConcession>) (x => x.Name.Equals("JojaCorn")));
      if (movieConcession != null)
        list1.Add(movieConcession);
    }
    Utility.Shuffle<MovieConcession>(daySaveRandom, list1);
    return list1;
  }

  public override bool answerDialogueAction(string questionAndAnswer, string[] questionParams)
  {
    switch (questionAndAnswer)
    {
      case null:
        return false;
      case "LeaveMovie_Yes":
        this._Leave();
        return true;
      case "Concession_Yes":
        Utility.TryOpenShopMenu("Concessions", (GameLocation) this, forceOpen: true);
        if (Game1.activeClickableMenu is ShopMenu activeClickableMenu)
          activeClickableMenu.onPurchase = new ShopMenu.OnPurchaseDelegate(this.OnPurchaseConcession);
        return true;
      default:
        return base.answerDialogueAction(questionAndAnswer, questionParams);
    }
  }

  protected void _Leave()
  {
    MovieTheater.forceMovieId = (string) null;
    Game1.player.completelyStopAnimatingOrDoingAction();
    Game1.warpFarmer("Town", this._exitX, this._exitY, 2);
  }

  /// <inheritdoc />
  public override bool performAction(string[] action, Farmer who, Location tileLocation)
  {
    switch (ArgUtility.Get(action, 0))
    {
      case "Concessions":
        if (this.CurrentState > 0)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionAfterMovie"));
          return true;
        }
        if (!this.HasInvitedSomeone(who))
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionAlone"));
          return true;
        }
        if (this.HasPurchasedConcession(who))
        {
          foreach (MovieInvitation movieInvitation in who.team.movieInvitations)
          {
            if (movieInvitation.farmer == who && MovieTheater.GetFirstInvitedPlayer(movieInvitation.invitedNPC) == Game1.player)
            {
              foreach (string key in this._purchasedConcessions.Keys)
              {
                if (key == movieInvitation.invitedNPC.Name)
                {
                  MovieConcession concessions = this.GetConcessionsDictionary()[(Character) Game1.getCharacterFromName(key)];
                  Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_ConcessionPurchased", (object) concessions.DisplayName, (object) Game1.RequireCharacter(key).displayName));
                  return true;
                }
              }
            }
          }
          return true;
        }
        Game1.currentLocation.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:MovieTheater_Concession"), Game1.currentLocation.createYesNoResponses(), "Concession");
        return true;
      case "Theater_Doors":
        if (this.CurrentState > 0)
        {
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Theater_MovieEndReEntry"));
          return true;
        }
        if (Game1.player.team.movieMutex.IsLocked())
        {
          this._ShowMovieStartReady();
          return true;
        }
        Game1.player.team.movieMutex.RequestLock((Action) (() =>
        {
          List<Farmer> present_farmers = new List<Farmer>();
          foreach (Farmer farmer in this.farmers)
          {
            if (farmer.isActive() && farmer.currentLocation == this)
              present_farmers.Add(farmer);
          }
          this.movieViewerLockEvent.Fire(new MovieViewerLockEvent(present_farmers, Game1.timeOfDay));
        }));
        return true;
      case "CraneGame":
        if (!this.hasTileAt(2, 9, "Buildings"))
          this.createQuestionDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:MovieTheater_CranePlay", (object) 500), this.createYesNoResponses(), new GameLocation.afterQuestionBehavior(this.tryToStartCraneGame));
        else
          Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromMaps:MovieTheater_CraneOccupied"));
        return true;
      default:
        return base.performAction(action, who, tileLocation);
    }
  }

  private void tryToStartCraneGame(Farmer who, string whichAnswer)
  {
    if (!whichAnswer.EqualsIgnoreCase("yes"))
      return;
    if (Game1.player.Money >= 500)
    {
      Game1.player.Money -= 500;
      Game1.changeMusicTrack("none", music_context: MusicContext.MiniGame);
      Game1.globalFadeToBlack((Game1.afterFadeFunction) (() => Game1.currentMinigame = (IMinigame) new CraneGame()), 0.008f);
    }
    else
      Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:PurchaseAnimalsMenu.cs.11325"));
  }

  public static void ClearCachedLocalizedData()
  {
    MovieTheater._concessions = (Dictionary<string, MovieConcession>) null;
    MovieTheater._genericReactions = (List<MovieCharacterReaction>) null;
    MovieTheater._movieData = (List<MovieData>) null;
  }

  /// <summary>Reset the cached concession tastes, so they're reloaded from <c>Data/ConcessionTastes</c> next time they're accessed.</summary>
  public static void ClearCachedConcessionTastes()
  {
    MovieTheater._concessionTastes = (List<ConcessionTaste>) null;
  }

  public enum MovieStates
  {
    Preshow,
    Show,
    PostShow,
  }
}
