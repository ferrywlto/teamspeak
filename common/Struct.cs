/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

namespace teamspeak.definitions
{
    //"C#, Visual Basic, and C++ compilers apply the Sequential layout value to structures by default.
    //They must be struct, cannot be class due to SDK constraint.
    //[StructLayout(LayoutKind.Sequential)]
    public struct ServerEventCallbackMapper
    {
        public VoiceDataEvent onVoiceData;
        public ClientStartTalkingEvent onClientStartTalking;
        public ClientStopTalkingEvent onClientStopTalking;
        public ClientConnectedEvent onClientConnected;
        public ClientDisconnectedEvent onClientDisconnected;
        public ClientMovedEvent onClientMoved;
        public ChannelCreatedEvent onChannelCreated;
        public ChannelEditedEvent onChannelEdited;
        public ChannelDeletedEvent onChannelDeleted;
        public ServerTextMessageEvent onServerTextMessage;
        public ChannelTextMessageEvent onChannelTextMessage;
        public UserLoggingMessageEvent onUserLoggingMessage;
        public AccountingErrorEvent onAccountingError;

        //[WARING] Ferry: Do NOT remove these two lines, they are required for the SDK DLL,
        //remove or comment out them will make the program crash when client connects!
        /* in serverlib.h: they should be in CustomPacketEncrypt/DecryptEvent type,
        * but since the example not implementing them so they are sustitued by Dummy Event,
        * also since the serverlib declared 15 function pointers in the struct ServerLibFunctions,
        * the struct used in C# have to be in exact 15 delegates as well,
        * otherwise runtime memory access violation will occur.
        void (*onCustomPacketEncryptEvent) (char** dataToSend, unsigned int* sizeOfData);
	    void (*onCustomPacketDecryptEvent) (char** dataReceived, unsigned int* dataReceivedSize);
        */
        public CustomPacketEncryptEvent onCustomPacketEncrypt;
        public CustomPacketDecryptEvent onCustomPacketDecrypt;
    }

    #region Original Struct Definition in C

    /*
struct ServerLibFunctions {
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
	void (*onCustomPacketDecryptEvent) (char** dataReceived, unsigned int* dataReceivedSize);
}; //END OF ServerLibFunctions */

    #endregion Original Struct Definition in C

    public struct ClientEventCallbackMapper
    {
        public ConnectStatusChangeEvent onConnectStatusChange;
        public ServerProtocolVersionEvent onServerProtocolVersion;
        public NewChannelEvent onNewChannel;
        public NewChannelCreatedEvent onNewChannelCreated;
        public DelChannelEvent onDelChannel;
        public ChannelMoveEvent onChannelMove;
        public UpdateChannelEvent onUpdateChannel;
        public UpdateChannelEditedEvent onUpdateChannelEdited;
        public UpdateClientEvent onUpdateClient;
        public ClientMoveEvent onClientMove;
        public ClientMoveSubscriptionEvent onClientMoveSubscription;
        public ClientMoveTimeoutEvent onClientMoveTimeout;
        public ClientMovedByOtherEvent onClientMovedByOther;
        public ClientKickFromChannelEvent onClientKickFromChannel;
        public ClientKickFromServerEvent onClientKickFromServer;
        public ClientIDsEvent onClientIDs;
        public ClientIDsFinishedEvent onClientIDsFinished;
        public ServerEditedEvent onServerEdited;
        public ServerUpdatedEvent onServerUpdated;
        public ServerErrorEvent onServerError;
        public ServerStopEvent onServerStop;
        public TextMessageEvent onTextMessage;
        public TalkStatusChangeEvent onTalkStatusChange;
        public IgnoredWhisperEvent onIgnoredWhisper;
        public ConnectionInfoEvent onConnectionInfo;
        public ServerConnectionInfoEvent onServerConnectionInfo;
        public ChannelSubscribeEvent onChannelSubscribe;
        public ChannelSubscribeFinishedEvent onChannelSubscribeFinished;
        public ChannelUnsubscribeEvent onChannelUnsubscribe;
        public ChannelUnsubscribeFinishedEvent onChannelUnsubscribeFinished;
        public ChannelDescriptionUpdateEvent onChannelDescriptionUpdate;
        public ChannelPasswordChangedEvent onChannelPasswordChanged;
        public PlaybackShutdownCompleteEvent onPlaybackShutdownComplete;

        // Have to be exactly 44 delegates as well.
        public SoundDeviceListChangedEvent onSoundDeviceListChanged;

        public EditPlaybackVoiceDataEvent onEditPlaybackVoiceData;
        public EditPostProcessVoiceDataEvent onEditPostProcessVoiceData;
        public EditMixedPlaybackVoiceDataEvent onEditMixedPlaybackVoiceData;
        public EditCapturedVoiceDataEvent onEditCapturedVoiceData;
        public Custom3dRolloffCalculationClientEvent onCustom3dRolloffCalculationClient;
        public Custom3dRolloffCalculationWaveEvent onCustom3dRolloffCalculationWave;
        public UserLoggingMessageEvent onUserLoggingMessage;
        public CustomPacketEncryptEvent onCustomPacketEncrypt;
        public CustomPacketDecryptEvent onCustomPacketDecrypt;

        //seems missing this one?
        public ProvisioningSlotRequestResultEvent onProvisioningSlotRequestResult;

        #region Original Struct Definition in C

        /*
        struct ClientUIFunctions {
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
        }; //END OF ClientUIFunctions */

        #endregion Original Struct Definition in C
    }
    public struct TS3_VECTOR
    {
        float x;        /* X coordinate in 3D space. */
        float y;        /* Y coordinate in 3D space. */
        float z;        /* Z coordinate in 3D space. */
    } ;

    #region Original Struct Definition in C

    /*
    typedef struct {
        float x;        // X coordinate in 3D space. /
        float y;        // Y coordinate in 3D space. /
        float z;        // Z coordinate in 3D space. /
    } TS3_VECTOR;
        */

    #endregion Original Struct Definition in C
}