/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Runtime.InteropServices;
using teamspeak.enumeration.server;
using anyID = System.UInt16;
using uint64 = System.UInt64;

//Ferry: consolidated delegate declaration
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ClientTalkingEventHandler(uint64 serverID, anyID clientID);

//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
//public delegate void ChannelEventHandler(uint64 serverID, anyID invokerClientID, uint64 channelID);

//original delegate declaration
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void VoiceDataEventHandler(uint64 serverID, anyID clientID, string voiceData, uint voiceDataSize, uint frequency);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientStartTalkingEventDelegate(uint64 serverID, anyID clientID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onClientStopTalkingEventDelegate(uint64 serverID, anyID clientID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ClientConnectedEventHandler(uint64 serverID, anyID clientID, uint64 channelID, ref uint removeClientError);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ClientDisconnectedEventHandler(uint64 serverID, anyID clientID, uint64 channelID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ClientMovedEventHandler(uint64 serverID, anyID clientID, uint64 oldChannelID, uint64 newChannelID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelCreatedDelegate(uint64 serverID, anyID invokerClientID, uint64 channelID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelEditedDelegate(uint64 serverID, anyID invokerClientID, uint64 channelID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void onChannelDeletedDelegate(uint64 serverID, anyID invokerClientID, uint64 channelID);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ServerTextMessageEventHandler(uint64 serverID, anyID invokerClientID, string textMessage);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void ChannelTextMessageEventHandler(uint64 serverID, anyID invokerClientID, uint64 targetChannelID, string textMessage);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void UserLoggingMessageEventHandler(string logmessage, int logLevel, string logChannel, uint64 logID, string logTime, string completeLogString);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void AccountingErrorEventHandler(uint64 serverID, int errorCode);

/* Ferry: The documentation said they are not used so can comment out */
/* For unused callbacks */
//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
//public delegate void dummyDelegate();

[StructLayout(LayoutKind.Sequential)]
public struct ServerEventHandlerMapper
{
    public VoiceDataEventHandler onVoiceDataEvent;
    public onClientStartTalkingEventDelegate onClientStartTalkingEvent;
    public onClientStopTalkingEventDelegate onClientStopTalkingEvent;
    public ClientConnectedEventHandler onClientConnected;
    public ClientDisconnectedEventHandler onClientDisconnected;
    public ClientMovedEventHandler onClientMoved;
    public onChannelCreatedDelegate onChannelCreated;
    public onChannelEditedDelegate onChannelEdited;
    public onChannelDeletedDelegate onChannelDeleted;
    public ServerTextMessageEventHandler onServerTextMessageEvent;
    public ChannelTextMessageEventHandler onChannelTextMessageEvent;
    public UserLoggingMessageEventHandler onUserLoggingMessageEvent;
    public AccountingErrorEventHandler onAccountingErrorEvent;
    /* Ferry: The documentation said they are not used so can comment out */
    //public dummyDelegate dummy1;  // onCustomPacketEncryptEvent unused
    //public dummyDelegate dummy2;  // onCustomPacketDecryptEvent unused
}