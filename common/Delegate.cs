/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

namespace teamspeak.definitions
{
    using System.Runtime.InteropServices;

    #region Shared Delegates   
    /* For unused callbacks [WARNING] Don't comment it out, they are required to be here even unused! */
    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    //public delegate void UnusedEvent();
    
    // Custom Encryption
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CustomPacketEncryptEvent(string dataToSend, ref uint sizeOfData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void CustomPacketDecryptEvent(string dataReceived, ref uint dataReceivedSize);

    //Logging
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UserLoggingMessageEvent(string logMessage, int logLevel, string logChannel, ulong logID, string logTime, string completeLogString);
    #endregion

    #region Server Delegates
    #region Client
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ClientConnectedEvent(ulong serverID, ushort clientID, ulong channelID, ref uint removeClientError);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ClientDisconnectedEvent(ulong serverID, ushort clientID, ulong channelID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ClientMovedEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID);
    #endregion
    #region Channel
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelCreatedEvent(ulong serverID, ushort invokerClientID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelEditedEvent(ulong serverID, ushort invokerClientID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelDeletedEvent(ulong serverID, ushort invokerClientID, ulong channelID);
    #endregion
    #region Messaging
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ChannelTextMessageEvent(ulong serverID, ushort invokerClientID, ulong targetChannelID, string textMessage);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ServerTextMessageEvent(ulong serverID, ushort invokerClientID, string textMessage);
    #endregion
    #region Client Talking
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientStartTalkingEvent(ulong serverID, ushort clientID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientStopTalkingEvent(ulong serverID, ushort clientID);
    #endregion
    #region Others
    //Accounting & Licensing
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void AccountingErrorEvent(ulong serverID, int errorCode);

    //Voice Data
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VoiceDataEvent(ulong serverID, ushort clientID, string voiceData, uint voiceDataSize, uint frequency);
    #endregion
    #endregion

    #region Client Delegates

    #region Server
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerUpdatedEvent(ulong serverID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerConnectionInfoEvent(ulong serverID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerStopEvent(ulong serverID, string shutdownMessage);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerProtocolVersionEvent(ulong serverID, int protocolVersion);
    #endregion
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerEditedEvent(ulong serverID, ushort editerID, string editerName, string editerUniqueIdentifier);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ServerErrorEvent(ulong serverID, string errorMessage, uint error, string returnCode, string extraMessage);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PlaybackShutdownCompleteEvent(ulong serverID);

    #region Channel
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelDescriptionUpdateEvent(ulong serverID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelPasswordChangedEvent(ulong serverID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelSubscribeEvent(ulong serverID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelSubscribeFinishedEvent(ulong serverID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelUnsubscribeEvent(ulong serverID, ulong channelID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelUnsubscribeFinishedEvent(ulong serverID);
    #endregion

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DelChannelEvent(ulong serverID, ulong channelID, ushort invokerID, string invokerName, string invokerUniqueIdentifier);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ChannelMoveEvent(ulong serverID, ulong channelID, ulong newChannelParentID, ushort invokerID, string invokerName, string invokerUniqueIdentifier);

    //Listen for which channel has been edited
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UpdateChannelEvent(ulong serverID, ulong channelID);

    //Listen for who edited the channel variable
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UpdateChannelEditedEvent(ulong serverID, ulong channelID, ushort invokerID, string invokerName, string invokerUniqueIdentifier);
   
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NewChannelEvent(ulong serverID, ulong channelID, ulong channelParentID);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void NewChannelCreatedEvent(ulong serverID, ulong channelID, ulong channelParentID, ushort invokerID, string invokerName, string invokerUniqueIdentifier);

    #region Client
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientIDsEvent(ulong serverID, string uniqueClientIdentifier, ushort clientID, string clientName);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientIDsFinishedEvent(ulong serverID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientMoveSubscriptionEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility);    
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientMoveTimeoutEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string timeoutMessage);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientMoveEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string moveMessage);

    //Client Variable Updated
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void UpdateClientEvent(ulong serverID, ushort clientID);
    #endregion
    // Sound editing functions not implemented in this sample
    #region Sound Editing
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EditPlaybackVoiceDataEvent(ulong serverID, ushort clientID, ref short samples, int sampleCount, int channels);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EditCapturedVoiceDataEvent(ulong serverID, ref short samples, int sampleCount, int channels, ref int edited);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EditMixedPlaybackVoiceDataEvent(ulong serverID, ref short samples, int sampleCount, int channels, ref uint channelSpeakerArray, ref uint channelFillMask);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void EditPostProcessVoiceDataEvent(ulong serverID, ushort clientID, ref short samples, int sampleCount, int channels, ref uint channelSpeakerArray, ref uint channelFillMask);

    // 3D sounds
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Custom3dRolloffCalculationWaveEvent(ulong serverID, ulong waveHandle, float distance, ref float volume);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Custom3dRolloffCalculationClientEvent(ulong serverID, ushort clientID, float distance, ref float volume);
    
    // Misc
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SoundDeviceListChangedEvent(string modeID, int playOrCap);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VoiceRecordDataEvent(float[] data, uint dataSize);
    #endregion
    
    #region Kicking
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientMovedByOtherEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, ushort invokerID, string invokerName, string invokerUniqueIdentifier, string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientKickFromChannelEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, ushort invokerID, string invokerName, string invokerUniqueIdentifier, string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ClientKickFromServerEvent(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, ushort invokerID, string invokerName, string invokerUniqueIdentifier, string message);
    #endregion

    #region Misc
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TalkStatusChangeEvent(ulong serverID, int status, int isReceivedWhisper, ushort clientID);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ConnectStatusChangeEvent(ulong serverID, int newStatus, uint errorNumber);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ConnectionInfoEvent(ulong serverID, ushort clientID);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IgnoredWhisperEvent(ulong serverID, ushort clientID);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TextMessageEvent(ulong serverID, ushort targetMode, ushort toID, ushort fromID, string fromName, string fromUniqueIdentifier, string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ProvisioningSlotRequestResultEvent(uint error, ulong requestHandle, string connectionKey);
    #endregion
    #endregion
}