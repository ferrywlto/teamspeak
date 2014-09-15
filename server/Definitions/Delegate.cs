/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

namespace teamspeak.definition
{
    using System.Runtime.InteropServices;

    //Ferry: consolidated delegate declaration
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientTalkingEventHandler(ulong serverID, ushort clientID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelEventHandler(ulong serverID, ushort invokerClientID, ulong channelID);

    //original delegate declaration
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VoiceDataEventHandler(ulong serverID, ushort clientID, string voiceData, uint voiceDataSize, uint frequency);
    #region Consolidated
    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //public delegate void onClientStartTalkingEventDelegate(ulong serverID, anyID clientID);

    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //public delegate void onClientStopTalkingEventDelegate(ulong serverID, anyID clientID);
    #endregion
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientConnectedEventHandler(ulong serverID, ushort clientID, ulong channelID, ref uint removeClientError);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientDisconnectedEventHandler(ulong serverID, ushort clientID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientMovedEventHandler(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID);
    #region Consolidated
    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //public delegate void onChannelCreatedDelegate(ulong serverID, anyID invokerClientID, ulong channelID);

    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //public delegate void onChannelEditedDelegate(ulong serverID, anyID invokerClientID, ulong channelID);

    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //public delegate void onChannelDeletedDelegate(ulong serverID, anyID invokerClientID, ulong channelID);
    #endregion
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerTextMessageEventHandler(ulong serverID, ushort invokerClientID, string textMessage);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelTextMessageEventHandler(ulong serverID, ushort invokerClientID, ulong targetChannelID, string textMessage);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UserLoggingMessageEventHandler(string logMessage, int logLevel, string logChannel, ulong logID, string logTime, string completeLogString);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AccountingErrorEventHandler(ulong serverID, int errorCode);

    /* For unused callbacks [WARNING] Don't comment it out, they are required to be here even unused! */
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UnusedEventHandler();

    //Ferry: Below are our own Delegates
    public delegate void MessageEventHandler(string message);
}