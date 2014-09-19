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
    public delegate void CustomPacketDecryptEvent(string dataReceived, ref uint sizeOfData);

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
    #region Original Definition in C
    /*	
    void (*onVoiceDataEvent)           (uint64 serverID, anyID clientID, unsigned char* voiceData, unsigned int voiceDataSize, unsigned int frequency);
	void (*onClientStartTalkingEvent)  (uint64 serverID, anyID clientID);
	void (*onClientStopTalkingEvent)   (uint64 serverID, anyID clientID);
	void (*onClientConnected)          (uint64 serverID, anyID clientID, uint64 channelID, unsigned int* removeClientError);
	void (*onClientDisconnected)       (uint64 serverID, anyID clientID, uint64 channelID);
	void (*onClientMoved)              (uint64 serverID, anyID clientID, uint64 oldChannelID, uint64 newChannelID);
	void (*onChannelCreated)           (uint64 serverID, anyID invokerClientID, uint64 channelID);
	void (*onChannelEdited)            (uint64 serverID, anyID invokerClientID, uint64 channelID);
	void (*onChannelDeleted)           (uint64 serverID, anyID invokerClientID, uint64 channelID);
	void (*onServerTextMessageEvent)   (uint64 serverID, anyID invokerClientID, const char* textMessage);
	void (*onChannelTextMessageEvent)  (uint64 serverID, anyID invokerClientID, uint64 targetChannelID, const char* textMessage);
	void (*onUserLoggingMessageEvent)  (const char* logmessage, int logLevel, const char* logChannel, uint64 logID, const char* logTime, const char* completeLogString);
	void (*onAccountingErrorEvent)     (uint64 serverID, unsigned int errorCode);
	void (*onCustomPacketEncryptEvent) (char** dataToSend, unsigned int* sizeOfData);
	void (*onCustomPacketDecryptEvent) (char** dataReceived, unsigned int* dataReceivedSize);*/
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
    public delegate void ConnectStatusChangeEvent(ulong serverID, ConnectStatus newStatus, uint errorNumber);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ConnectionInfoEvent(ulong serverID, ushort clientID);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void IgnoredWhisperEvent(ulong serverID, ushort clientID);
    
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void TextMessageEvent(ulong serverID, TextMessageTargetMode targetMode, ushort toID, ushort fromID, string fromName, string fromUniqueIdentifier, string message);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void ProvisioningSlotRequestResultEvent(uint error, ulong requestHandle, string connectionKey);
    #endregion

    #region Original Definition in C
    /*struct ClientUIFunctions {
	void (*onConnectStatusChangeEvent)              (uint64 serverConnectionHandlerID, int newStatus, unsigned int errorNumber);
	void (*onServerProtocolVersionEvent)            (uint64 serverConnectionHandlerID, int protocolVersion);
	void (*onNewChannelEvent)                       (uint64 serverConnectionHandlerID, uint64 channelID, uint64 channelParentID);
	void (*onNewChannelCreatedEvent)                (uint64 serverConnectionHandlerID, uint64 channelID, uint64 channelParentID, anyID invokerID, const char* invokerName, const char* invokerUniqueIdentifier);
	void (*onDelChannelEvent)                       (uint64 serverConnectionHandlerID, uint64 channelID, anyID invokerID, const char* invokerName, const char* invokerUniqueIdentifier);
	void (*onChannelMoveEvent)                      (uint64 serverConnectionHandlerID, uint64 channelID, uint64 newChannelParentID, anyID invokerID, const char* invokerName, const char* invokerUniqueIdentifier);
	void (*onUpdateChannelEvent)                    (uint64 serverConnectionHandlerID, uint64 channelID);
	void (*onUpdateChannelEditedEvent)              (uint64 serverConnectionHandlerID, uint64 channelID, anyID invokerID, const char* invokerName, const char* invokerUniqueIdentifier);
	void (*onUpdateClientEvent)                     (uint64 serverConnectionHandlerID, anyID clientID, anyID invokerID, const char* invokerName, const char* invokerUniqueIdentifier);
	void (*onClientMoveEvent)                       (uint64 serverConnectionHandlerID, anyID clientID, uint64 oldChannelID, uint64 newChannelID, int visibility, const char* moveMessage);
	void (*onClientMoveSubscriptionEvent)           (uint64 serverConnectionHandlerID, anyID clientID, uint64 oldChannelID, uint64 newChannelID, int visibility);
	void (*onClientMoveTimeoutEvent)                (uint64 serverConnectionHandlerID, anyID clientID, uint64 oldChannelID, uint64 newChannelID, int visibility, const char* timeoutMessage);
	void (*onClientMoveMovedEvent)                  (uint64 serverConnectionHandlerID, anyID clientID, uint64 oldChannelID, uint64 newChannelID, int visibility, anyID moverID, const char* moverName, const char* moverUniqueIdentifier, const char* moveMessage);
	void (*onClientKickFromChannelEvent)            (uint64 serverConnectionHandlerID, anyID clientID, uint64 oldChannelID, uint64 newChannelID, int visibility, anyID kickerID, const char* kickerName, const char* kickerUniqueIdentifier, const char* kickMessage);
	void (*onClientKickFromServerEvent)             (uint64 serverConnectionHandlerID, anyID clientID, uint64 oldChannelID, uint64 newChannelID, int visibility, anyID kickerID, const char* kickerName, const char* kickerUniqueIdentifier, const char* kickMessage);
	void (*onClientIDsEvent)                        (uint64 serverConnectionHandlerID, const char* uniqueClientIdentifier, anyID clientID, const char* clientName);
	void (*onClientIDsFinishedEvent)                (uint64 serverConnectionHandlerID);
	void (*onServerEditedEvent)                     (uint64 serverConnectionHandlerID, anyID editerID, const char* editerName, const char* editerUniqueIdentifier);
	void (*onServerUpdatedEvent)                    (uint64 serverConnectionHandlerID);
	void (*onServerErrorEvent)                      (uint64 serverConnectionHandlerID, const char* errorMessage, unsigned int error, const char* returnCode, const char* extraMessage);
	void (*onServerStopEvent)                       (uint64 serverConnectionHandlerID, const char* shutdownMessage);
	void (*onTextMessageEvent)                      (uint64 serverConnectionHandlerID, anyID targetMode, anyID toID, anyID fromID, const char* fromName, const char* fromUniqueIdentifier, const char* message);
	void (*onTalkStatusChangeEvent)                 (uint64 serverConnectionHandlerID, int status, int isReceivedWhisper, anyID clientID);
	void (*onIgnoredWhisperEvent)                   (uint64 serverConnectionHandlerID, anyID clientID);
	void (*onConnectionInfoEvent)                   (uint64 serverConnectionHandlerID, anyID clientID);
	void (*onServerConnectionInfoEvent)             (uint64 serverConnectionHandlerID);
	void (*onChannelSubscribeEvent)                 (uint64 serverConnectionHandlerID, uint64 channelID);
	void (*onChannelSubscribeFinishedEvent)         (uint64 serverConnectionHandlerID);
	void (*onChannelUnsubscribeEvent)               (uint64 serverConnectionHandlerID, uint64 channelID);
	void (*onChannelUnsubscribeFinishedEvent)       (uint64 serverConnectionHandlerID);
	void (*onChannelDescriptionUpdateEvent)         (uint64 serverConnectionHandlerID, uint64 channelID);
	void (*onChannelPasswordChangedEvent)           (uint64 serverConnectionHandlerID, uint64 channelID);
	void (*onPlaybackShutdownCompleteEvent)         (uint64 serverConnectionHandlerID);
	void (*onSoundDeviceListChangedEvent)           (const char* modeID, int playOrCap);
	void (*onEditPlaybackVoiceDataEvent)            (uint64 serverConnectionHandlerID, anyID clientID, short* samples, int sampleCount, int channels);
	void (*onEditPostProcessVoiceDataEvent)         (uint64 serverConnectionHandlerID, anyID clientID, short* samples, int sampleCount, int channels, const unsigned int* channelSpeakerArray, unsigned int* channelFillMask);
	void (*onEditMixedPlaybackVoiceDataEvent)       (uint64 serverConnectionHandlerID, short* samples, int sampleCount, int channels, const unsigned int* channelSpeakerArray, unsigned int* channelFillMask);
	void (*onEditCapturedVoiceDataEvent)            (uint64 serverConnectionHandlerID, short* samples, int sampleCount, int channels, int* edited);
	void (*onCustom3dRolloffCalculationClientEvent) (uint64 serverConnectionHandlerID, anyID clientID, float distance, float* volume);
	void (*onCustom3dRolloffCalculationWaveEvent)   (uint64 serverConnectionHandlerID, uint64 waveHandle, float distance, float* volume);
	void (*onUserLoggingMessageEvent)               (const char* logmessage, int logLevel, const char* logChannel, uint64 logID, const char* logTime, const char* completeLogString);
	void (*onCustomPacketEncryptEvent)              (char** dataToSend, unsigned int* sizeOfData);
	void (*onCustomPacketDecryptEvent)              (char** dataReceived, unsigned int* dataReceivedSize);
	void (*onProvisioningSlotRequestResultEvent)    (unsigned int error, uint64 requestHandle, const char* connectionKey);

}; //END OF ClientUIFunctions*/
    #endregion
    #endregion
}