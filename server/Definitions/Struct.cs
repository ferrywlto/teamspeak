/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System.Runtime.InteropServices;

namespace teamspeak.definition
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ServerEventCallbackMapper
    {
        public VoiceDataEventHandler onVoiceData;
        public ClientTalkingEventHandler onClientStartTalking;
        public ClientTalkingEventHandler onClientStopTalking;
        public ClientConnectedEventHandler onClientConnected;
        public ClientDisconnectedEventHandler onClientDisconnected;
        public ClientMovedEventHandler onClientMoved;
        public ChannelEventHandler onChannelCreated;
        public ChannelEventHandler onChannelEdited;
        public ChannelEventHandler onChannelDeleted;
        public ServerTextMessageEventHandler onServerTextMessage;
        public ChannelTextMessageEventHandler onChannelTextMessage;
        public UserLoggingMessageEventHandler onUserLoggingMessage;
        public AccountingErrorEventHandler onAccountingError;

        //[WARING] Ferry: Do NOT remove these two lines, they are required for the SDK DLL,
        //remove or comment out them will make the program crash when client connects!
        public UnusedEventHandler unusedButHaveToBeHere1;  // onCustomPacketEncryptEvent unused

        public UnusedEventHandler unusedButHaveToBeHere2;  // onCustomPacketDecryptEvent unused
    }
}