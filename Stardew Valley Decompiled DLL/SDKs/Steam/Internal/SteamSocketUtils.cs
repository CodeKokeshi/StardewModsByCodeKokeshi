// Decompiled with JetBrains decompiler
// Type: StardewValley.SDKs.Steam.Internal.SteamSocketUtils
// Assembly: Stardew Valley, Version=1.6.15.24356, Culture=neutral, PublicKeyToken=null
// MVID: AA1F513A-94F0-4EF6-A35F-2D5B4A3721BF
// Assembly location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.dll
// XML documentation location: D:\SteamLibrary\steamapps\common\Stardew Valley\Stardew Valley.xml

using StardewValley.Network;
using Steamworks;
using System;
using System.IO;
using System.Runtime.InteropServices;

#nullable disable
namespace StardewValley.SDKs.Steam.Internal;

/// <summary>Simplifies interacting with Steam Networking Sockets for the Steam SDK client.</summary>
internal static class SteamSocketUtils
{
  /// <summary>Gets an array of configuration values to use when creating a Steam connection.</summary>
  internal static SteamNetworkingConfigValue_t[] GetNetworkingOptions()
  {
    return new SteamNetworkingConfigValue_t[1]
    {
      new SteamNetworkingConfigValue_t()
      {
        m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_SendBufferSize,
        m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
        m_val = {
          m_int32 = 1048576 /*0x100000*/
        }
      }
    };
  }

  /// <summary>Converts a <see cref="T:Steamworks.SteamNetworkingMessage_t" /> into an <see cref="T:StardewValley.Network.IncomingMessage" /> to be used internally, decompressing the data if needed.</summary>
  /// <param name="messagePtr">A pointer to the <see cref="T:Steamworks.SteamNetworkingMessage_t" /> that we will process.</param>
  /// <param name="message">A reference to message to write the data into.</param>
  /// <param name="messageConnection">The connection that sent the <see cref="T:Steamworks.SteamNetworkingMessage_t" />.</param>
  /// <param name="bandwidthLogger">A bandwidth logger with which to log the number of bytes received.</param>
  internal static void ProcessSteamMessage(
    IntPtr messagePtr,
    IncomingMessage message,
    out HSteamNetConnection messageConnection,
    BandwidthLogger bandwidthLogger)
  {
    SteamNetworkingMessage_t structure = (SteamNetworkingMessage_t) Marshal.PtrToStructure(messagePtr, typeof (SteamNetworkingMessage_t));
    messageConnection = structure.m_conn;
    byte[] numArray = new byte[structure.m_cbSize];
    Marshal.Copy(structure.m_pData, numArray, 0, numArray.Length);
    using (MemoryStream input = new MemoryStream(Program.netCompression.DecompressBytes(numArray)))
    {
      input.Position = 0L;
      using (BinaryReader reader = new BinaryReader((Stream) input))
        message.Read(reader);
    }
    SteamNetworkingMessage_t.Release(messagePtr);
    bandwidthLogger?.RecordBytesDown((long) numArray.Length);
  }

  /// <summary>Converts and sends an <see cref="T:StardewValley.Network.OutgoingMessage" /> over Steam's sockets, compressing the data if needed.</summary>
  /// <param name="messageConnection">The connection through which to send the message.</param>
  /// <param name="message">The message to be sent using Steam's sockets.</param>
  /// <param name="bandwidthLogger">A bandwidth logger with which to log the number of bytes sent.</param>
  /// <param name="onDisconnected">Cleans up all bookkeeping data about the connection if a message fails to send.</param>
  internal static unsafe void SendMessage(
    HSteamNetConnection messageConnection,
    OutgoingMessage message,
    BandwidthLogger bandwidthLogger,
    Action<HSteamNetConnection> onDisconnected = null)
  {
    byte[] data = (byte[]) null;
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
      {
        message.Write(writer);
        output.Seek(0L, SeekOrigin.Begin);
        data = output.ToArray();
      }
    }
    byte[] numArray = Program.netCompression.CompressAbove(data, 1024 /*0x0400*/);
    EResult connection;
    fixed (byte* pData = numArray)
      connection = SteamNetworkingSockets.SendMessageToConnection(messageConnection, (IntPtr) (void*) pData, Convert.ToUInt32(numArray.Length), 8, out long _);
    if (connection != EResult.k_EResultOK)
    {
      Game1.log.Warn($"Failed to send message ({connection.ToString()}). Closing connection.");
      SteamSocketUtils.CloseConnection(messageConnection, onDisconnected);
    }
    else
      bandwidthLogger?.RecordBytesUp((long) numArray.Length);
  }

  /// <summary>Closes a Steam connection if it's valid.</summary>
  /// <param name="connection">The connection to close.</param>
  /// <param name="onDisconnected">The callback invoked immediately before the connection is closed to perform any cleanup needed.</param>
  internal static void CloseConnection(
    HSteamNetConnection connection,
    Action<HSteamNetConnection> onDisconnected = null)
  {
    if (connection == HSteamNetConnection.Invalid)
      return;
    SteamNetworkingSockets.SetConnectionPollGroup(connection, HSteamNetPollGroup.Invalid);
    if (onDisconnected != null)
      onDisconnected(connection);
    SteamNetworkingSockets.CloseConnection(connection, 1000, (string) null, true);
  }
}
