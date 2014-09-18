/*
 * TeamSpeak 3 client minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using teamspeak.definitions;

namespace teamspeak
{
    public class CustomTS3Client : TS3SDKConsumerBase
    {
        //Ferry: Below are our own Delegates
        public delegate void CustomServerMessageEvent(string message);

        //Ferry: [WARNING] _mapper must be public or AccessViolation will throw
        public ClientEventCallbackMapper _mapper;

        #region Event Declarations

        public event ChannelCreatedEvent ChannelCreated;

        public event ChannelDeletedEvent ChannelDeleted;

        public event ChannelDescriptionUpdateEvent ChannelDescriptionUpdate;

        public event ChannelEditedEvent ChannelEdited;

        public event ChannelMoveEvent ChannelMove;

        public event ChannelPasswordChangedEvent ChannelPasswordChanged;

        public event ChannelSubscribeEvent ChannelSubscribe;

        public event ChannelSubscribeFinishedEvent ChannelSubscribeFinished;

        public event ChannelTextMessageEvent ChannelTextMessage;

        public event ChannelUnsubscribeEvent ChannelUnsubscribe;

        public event ChannelUnsubscribeFinishedEvent ChannelUnsubscribeFinished;

        public event ClientConnectedEvent ClientConnected;

        public event ClientDisconnectedEvent ClientDisconnected;

        public event ClientIDsEvent ClientIDs;

        public event ClientIDsFinishedEvent ClientIDsFinished;

        public event ClientKickFromChannelEvent ClientKickFromChannel;

        public event ClientKickFromServerEvent ClientKickFromServer;

        public event ClientMovedByOtherEvent ClientMovedByOther;

        public event ClientMovedEvent ClientMoved;

        public event ClientMoveEvent ClientMove;

        public event ClientMoveSubscriptionEvent ClientMoveSubscription;

        public event ClientMoveTimeoutEvent ClientMoveTimeout;

        public event ClientStartTalkingEvent ClientStartTalking;

        public event ClientStopTalkingEvent ClientStopTalking;

        public event ConnectionInfoEvent ConnectionInfo;

        public event ConnectStatusChangeEvent ConnectStatusChange;

        public event ConsoleCancelEventHandler ConsoleCancelEventHandler;

        public event Custom3dRolloffCalculationClientEvent Custom3dRolloffCalculationClient;

        public event Custom3dRolloffCalculationWaveEvent Custom3dRolloffCalculationWave;

        public event CustomPacketDecryptEvent CustomPacketDecrypt;

        public event CustomPacketEncryptEvent CustomPacketEncrypt;

        public event CustomServerMessageEvent CustomServerMessage;

        public event DelChannelEvent DelChannel;

        public event EditCapturedVoiceDataEvent EditCapturedVoiceData;

        public event EditMixedPlaybackVoiceDataEvent EditMixedPlaybackVoiceData;

        public event EditPlaybackVoiceDataEvent EditPlaybackVoiceData;

        public event EditPostProcessVoiceDataEvent EditPostProcessVoiceData;

        public event IgnoredWhisperEvent IgnoredWhisper;

        public event NewChannelCreatedEvent NewChannelCreated;

        public event NewChannelEvent NewChannel;

        public event PlaybackShutdownCompleteEvent PlaybackShutdownComplete;

        public event ProvisioningSlotRequestResultEvent ProvisioningSlotRequestResult;

        public event ServerConnectionInfoEvent ServerConnectionInfo;

        public event ServerEditedEvent ServerEdited;

        public event ServerErrorEvent ServerError;

        public event ServerProtocolVersionEvent ServerProtocolVersion;

        public event ServerStopEvent ServerStop;

        public event ServerTextMessageEvent ServerTextMessage;

        public event ServerUpdatedEvent ServerUpdated;

        public event SoundDeviceListChangedEvent SoundDeviceListChanged;

        public event TalkStatusChangeEvent TalkStatusChange;

        public event TextMessageEvent TextMessage;

        public event UpdateChannelEditedEvent UpdateChannelEdited;

        public event UpdateChannelEvent UpdateChannel;

        public event UpdateClientEvent UpdateClient;

        public event UserLoggingMessageEvent UserLoggingMessage;

        public event VoiceDataEvent VoiceData;

        public event VoiceRecordDataEvent VoiceRecordData;

        #endregion Event Declarations

        public void initMapper()
        {
            _mapper = new ClientEventCallbackMapper();
            _mapper.onConnectStatusChange = onConnectStatusChange;
            _mapper.onConnectStatusChange = onConnectStatusChange;
            _mapper.onServerProtocolVersion = onServerProtocolVersion;
            _mapper.onNewChannel = onNewChannel;
            _mapper.onNewChannelCreated = onNewChannelCreated;
            _mapper.onDelChannel = onDelChannel;
            _mapper.onClientMove = onClientMove;
            _mapper.onClientMoveSubscription = onClientMoveSubscription;
            _mapper.onClientMoveTimeout = onClientMoveTimeout;
            _mapper.onTalkStatusChange = onTalkStatusChange;
            _mapper.onIgnoredWhisper = onIgnoredWhisper;
            _mapper.onServerError = onServerError;
            _mapper.onServerStop = onServerStop;
        }

        #region Event Handlers

        void onConnectStatusChange(ulong serverID, int newStatus, uint errorNumber)
        {
            Console.WriteLine("Connect status changed: {0} {1} {2}", serverID, newStatus, errorNumber);
            /* Failed to connect ? */
            if (newStatus == (int)ConnectStatus.STATUS_DISCONNECTED && errorNumber == Error.failed_connection_initialisation)
            {
                Console.WriteLine("Looks like there is no server running, terminate!");
                Console.ReadLine();
                System.Environment.Exit(-1);
            }
            if (ConnectStatusChange != null)
                ConnectStatusChange(serverID, newStatus, errorNumber);
        }

        void onServerProtocolVersion(ulong serverID, int protocolVersion)
        {
            Console.WriteLine("Server protocol version: {0} {1}", serverID, protocolVersion);
            if (ServerProtocolVersion != null)
                ServerProtocolVersion(serverID, protocolVersion);
        }

        void onNewChannel(ulong serverID, ulong channelID, ulong channelParentID)
        {
            string name;
            string errormsg;
            uint error;
            IntPtr namePtr = IntPtr.Zero;
            Console.WriteLine("onNewChannelEvent: {0} {1} {2}", serverID, channelID, channelParentID);
            error = GetChannelVariableAsString(serverID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr);
            if (error == Error.ok)
            {
                name = Marshal.PtrToStringAnsi(namePtr);
                Console.WriteLine("New channel: {0} {1}", channelID, name);
                FreeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
            }
            else
            {
                IntPtr errormsgPtr = IntPtr.Zero;
                if (GetErrorMessage(error, errormsgPtr) == Error.ok)
                {
                    errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
                    Console.WriteLine("Error getting channel name in onNewChannelEvent: {0}", errormsg);
                    FreeMemory(errormsgPtr);
                }
            }
            if (NewChannel != null)
                NewChannel(serverID, channelID, channelParentID);
        }

        void onNewChannelCreated(ulong serverID, ulong channelID, ulong channelParentID, ushort invokerID, string invokerName, string invokerUniqueIdentifier)
        {
            string name;
            IntPtr namePtr = IntPtr.Zero;
            /* Query channel name from channel ID */
            uint error = GetChannelVariableAsString(serverID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr);
            if (error != Error.ok)
            {
                return;
            }
            name = Marshal.PtrToStringAnsi(namePtr);
            Console.WriteLine("New channel created: {0}", name);
            FreeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
            if (NewChannelCreated != null)
                NewChannelCreated(serverID, channelID, channelParentID, invokerID, invokerName, invokerUniqueIdentifier);
        }

        void onDelChannel(ulong serverID, ulong channelID, ushort invokerID, string invokerName, string invokerUniqueIdentifier)
        {
            Console.WriteLine("Channel ID {0} deleted by {1} ({2})", channelID, invokerName, invokerID);
            if (DelChannel != null)
                DelChannel(serverID, channelID, invokerID, invokerName, invokerUniqueIdentifier);
        }

        void onClientMove(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string moveMessage)
        {
            Console.WriteLine("ClientID {0} moves from channel {1} to {2} with message {3}", clientID, oldChannelID, newChannelID, moveMessage);
            if (ClientMove != null)
                ClientMove(serverID, clientID, oldChannelID, newChannelID, visibility, moveMessage);
        }

        void onClientMoveSubscription(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility)
        {
            string name;
            IntPtr namePtr = IntPtr.Zero;
            /* Query client nickname from ID */
            uint error = GetClientVariableAsString(serverID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
            if (error != Error.ok)
            {
                return;
            }
            name = Marshal.PtrToStringAnsi(namePtr);
            Console.WriteLine("New client: {0}", name);
            FreeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */

            if (ClientMoveSubscription != null)
                ClientMoveSubscription(serverID, clientID, oldChannelID, newChannelID, visibility);
        }

        void onClientMoveTimeout(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string timeoutMessage)
        {
            Console.WriteLine("ClientID {0} timeouts with message {1}", clientID, timeoutMessage);

            if (ClientMoveTimeout != null)
                ClientMoveTimeout(serverID, clientID, oldChannelID, newChannelID, visibility, timeoutMessage);
        }

        void onTalkStatusChange(ulong serverID, int status, int isReceivedWhisper, ushort clientID)
        {
            string name;
            IntPtr namePtr = IntPtr.Zero;
            /* Query client nickname from ID */
            uint error = GetClientVariableAsString(serverID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
            if (error != Error.ok)
            {
                return;
            }
            name = Marshal.PtrToStringAnsi(namePtr);
            //if (status == (int)TalkStatus.STATUS_TALKING)
            //{
            //    Console.WriteLine("Client \"{0}\" {1} starts talking.", name, status == (int)TalkStatus.STATUS_TALKING ? "start" : "stop");
            //}
            //else
            //{
            //    Console.WriteLine("Client \"{0}\" stops talking.", name);
            //}
            Console.WriteLine("Client \"{0}\" {1} talking.", name, status == (int)TalkStatus.STATUS_TALKING ? "start" : "stop");
            FreeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */

            if (TalkStatusChange != null)
                TalkStatusChange(serverID, status, isReceivedWhisper, clientID);
        }

        void onIgnoredWhisper(ulong serverID, ushort clientID)
        {
            Console.WriteLine("Ignored whisper: {0} {1}", serverID, clientID);
            if (IgnoredWhisper != null)
                IgnoredWhisper(serverID, clientID);
        }

        void onServerError(ulong serverID, string errorMessage, uint error, string returnCode, string extraMessage)
        {
            Console.WriteLine("Error for server {0}: {1}", serverID, errorMessage);
            if (ServerError != null)
                ServerError(serverID, errorMessage, error, returnCode, extraMessage);
        }

        void onServerStop(ulong serverID, string shutdownMessage)
        {
            Console.WriteLine("Server {0} stopping: {1}", serverID, shutdownMessage);
            if (ServerStop != null)
                ServerStop(serverID, shutdownMessage);
        }

        #endregion Event Handlers

        #region DLL Import

#if x64
        private const string DLL_FILE_NAME = "ts3client_win64.dll";
#else
        string DLL_FILE_NAME = "ts3client_win64.dll";
#endif
        //All copied from clientlib.h so no need to paste source here
        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_freeMemory")]
        static extern uint FreeMemory(IntPtr pointer);

        /*Construction and Destruction*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_initClientLib")]
        static extern uint InitClientLib(ref ClientEventCallbackMapper functionPointers, ref ClientEventCallbackMapper functionRarePointers, LogTypes usedLogTypes, string logFileFolder, string resourcesFolder);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_destroyClientLib")]
        static extern uint DestroyClientLib();

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientLibVersion")]
        static extern uint GetClientLibVersion(out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientLibVersionNumber")]
        static extern uint GetClientLibVersionNumber(out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_spawnNewServerConnectionHandler")]
        static extern uint SpawnNewServerConnectionHandler(int port, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_destroyServerConnectionHandler")]
        static extern uint DestroyServerConnectionHandler(ulong serverID);

        /*Identity management*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_createIdentity")]
        static extern uint CreateIdentity(out IntPtr result);

        /*sound*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPlaybackDeviceList")]
        static extern uint GetPlaybackDeviceList(string modeID, out string[][] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCaptureDeviceList")]
        static extern uint GetCaptureDeviceList(string modeID, out string[][] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPlaybackModeList")]
        static extern uint GetPlaybackModeList(out string[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCaptureModeList")]
        static extern uint GetCaptureModeList(out string[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultPlaybackDevice")]
        static extern uint GetDefaultPlaybackDevice(string modeID, out string[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultCaptureDevice")]
        static extern uint GetDefaultCaptureDevice(string modeID, out string[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultPlayBackMode")]
        static extern uint GetDefaultPlayBackMode(out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultCaptureMode")]
        static extern uint GetDefaultCaptureMode(out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_openPlaybackDevice")]
        static extern uint OpenPlaybackDevice(ulong serverID, string modeID, string playbackDevice);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_openCaptureDevice")]
        static extern uint OpenCaptureDevice(ulong serverID, string modeID, string captureDevice);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentPlaybackDeviceName")]
        static extern uint GetCurrentPlaybackDeviceName(ulong serverID, out string result, out int isDefault);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentPlayBackMode")]
        static extern uint GetCurrentPlayBackMode(ulong serverID, out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentCaptureDeviceName")]
        static extern uint GetCurrentCaptureDeviceName(ulong serverID, out string result, out int isDefault);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentCaptureMode")]
        static extern uint GetCurrentCaptureMode(ulong serverID, out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_initiateGracefulPlaybackShutdown")]
        static extern uint InitiateGracefulPlaybackShutdown(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_closePlaybackDevice")]
        static extern uint ClosePlaybackDevice(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_closeCaptureDevice")]
        static extern uint CloseCaptureDevice(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_activateCaptureDevice")]
        static extern uint ActivateCaptureDevice(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_playWaveFile")]
        static extern uint PlayWaveFile(ulong serverID, string path);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_playWaveFileHandle")]
        static extern uint PlayWaveFileHandle(ulong serverID, string path, int loop, out ulong waveHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_pauseWaveFileHandle")]
        static extern uint PauseWaveFileHandle(ulong serverID, ulong waveHandle, int pause);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_closeWaveFileHandle")]
        static extern uint CloseWaveFileHandle(ulong serverID, ulong waveHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_registerCustomDevice")]
        static extern uint RegisterCustomDevice(string deviceID, string deviceDisplayName, int capFrequency, int capChannels, int playFrequency, int playChannels);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_unregisterCustomDevice")]
        static extern uint UnregisterCustomDevice(string deviceID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_processCustomCaptureData")]
        static extern uint ProcessCustomCaptureData(string deviceName, short[] buffer, int samples);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_acquireCustomPlaybackData")]
        static extern uint AcquireCustomPlaybackData(string deviceName, short[] buffer, int samples);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setLocalTestMode")]
        static extern uint SetLocalTestMode(ulong serverID, int status);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startVoiceRecording")]
        static extern uint StartVoiceRecording(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_stopVoiceRecording")]
        static extern uint StopVoiceRecording(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_allowWhispersFrom")]
        static extern uint AllowWhispersFrom(ulong serverID, ushort clID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_removeFromAllowedWhispersFrom")]
        static extern uint RemoveFromAllowedWhispersFrom(ulong serverID, ushort clID);

        /* 3d sound positioning */

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_systemset3DListenerAttributes")]
        static extern uint Systemset3DListenerAttributes(ulong serverID, TS3_VECTOR position, TS3_VECTOR forward, TS3_VECTOR up);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_set3DWaveAttributes")]
        static extern uint Set3DWaveAttributes(ulong serverID, ulong waveHandle, TS3_VECTOR position);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_systemset3DSettings")]
        static extern uint Systemset3DSettings(ulong serverID, float distanceFactor, float rolloffScale);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_channelset3DAttributes")]
        static extern uint Channelset3DAttributes(ulong serverID, ushort clientID, TS3_VECTOR position);

        /*preprocessor*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPreProcessorInfoValueFloat")]
        static extern uint GetPreProcessorInfoValueFloat(ulong serverID, string ident, out float result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPreProcessorConfigValue")]
        static extern uint GetPreProcessorConfigValue(ulong serverID, string ident, out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setPreProcessorConfigValue")]
        static extern uint SetPreProcessorConfigValue(ulong serverID, string ident, string value);

        /*encoder*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getEncodeConfigValue")]
        static extern uint GetEncodeConfigValue(ulong serverID, string ident, out string result);

        /*playback*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPlaybackConfigValueAsFloat")]
        static extern uint GetPlaybackConfigValueAsFloat(ulong serverID, string ident, out float result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setPlaybackConfigValue")]
        static extern uint SetPlaybackConfigValue(ulong serverID, string ident, string value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setClientVolumeModifier")]
        static extern uint SetClientVolumeModifier(ulong serverID, ushort clientID, float value);

        /*logging*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_logMessage")]
        static extern uint LogMessage(string logMessage, LogLevel severity, string channel, ulong logID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setLogVerbosity")]
        static extern uint SetLogVerbosity(LogLevel logVerbosity);

        /*error handling*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getErrorMessage")]
        static extern uint GetErrorMessage(uint errorCode, IntPtr error);

        /*Interacting with the server*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startConnection", CharSet = CharSet.Ansi)]
        static extern uint StartConnection(ulong serverID, string identity, string ip, uint port, string nickname, ref string defaultChannelArray, string defaultChannelPassword, string serverPassword);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_stopConnection")]
        static extern uint StopConnection(ulong serverID, string quitMessage);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientMove")]
        static extern uint RequestClientMove(ulong serverID, ushort clientID, ulong newChannelID, string password, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientVariables")]
        static extern uint RequestClientVariables(ulong serverID, ushort clientID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientKickFromChannel")]
        static extern uint RequestClientKickFromChannel(ulong serverID, ushort clientID, string kickReason, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientKickFromServer")]
        static extern uint RequestClientKickFromServer(ulong serverID, ushort clientID, string kickReason, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelDelete")]
        static extern uint RequestChannelDelete(ulong serverID, ulong channelID, int force, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelMove")]
        static extern uint RequestChannelMove(ulong serverID, ulong channelID, ulong newChannelParentID, ulong newChannelOrder, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSendPrivateTextMsg")]
        static extern uint RequestSendPrivateTextMsg(ulong serverID, string message, ushort targetClientID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSendChannelTextMsg")]
        static extern uint RequestSendChannelTextMsg(ulong serverID, string message, ulong targetChannelID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSendServerTextMsg")]
        static extern uint RequestSendServerTextMsg(ulong serverID, string message, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestConnectionInfo")]
        static extern uint RequestConnectionInfo(ulong serverID, ushort clientID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientSetWhisperList")]
        static extern uint RequestClientSetWhisperList(ulong serverID, ushort clientID, ulong[] targetChannelIDArray, ushort[] targetClientIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelSubscribe")]
        static extern uint RequestChannelSubscribe(ulong serverID, ulong[] channelIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelSubscribeAll")]
        static extern uint RequestChannelSubscribeAll(ulong serverID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelUnsubscribe")]
        static extern uint RequestChannelUnsubscribe(ulong serverID, ulong[] channelIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelUnsubscribeAll")]
        static extern uint RequestChannelUnsubscribeAll(ulong serverID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelDescription")]
        static extern uint RequestChannelDescription(ulong serverID, ulong channelID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestMuteClients")]
        static extern uint RequestMuteClients(ulong serverID, ushort[] clientIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestUnmuteClients")]
        static extern uint RequestUnmuteClients(ulong serverID, ushort[] clientIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientIDs")]
        static extern uint RequestClientIDs(ulong serverID, string clientUniqueIdentifier, string returnCode);

        /*provisioning server calls*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSlotsFromProvisioningServer")]
        static extern uint RequestSlotsFromProvisioningServer(string ip, ushort port, string serverPassword, ushort slots, string identity, string region, out ulong requestHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_cancelRequestSlotsFromProvisioningServer")]
        static extern uint CancelRequestSlotsFromProvisioningServer(ulong requestHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startConnectionWithProvisioningKey")]
        static extern uint StartConnectionWithProvisioningKey(ulong serverID, string identity, string nickname, string connectionKey);

        /*retrieve information ClientLib has stored*/

        /*general info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientID")]
        static extern uint GetClientID(ulong serverID, ushort* result);

        /*client connection info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionStatus")]
        static extern uint GetConnectionStatus(ulong serverID, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionVariableAsUInt64")]
        static extern uint GetConnectionVariableAsulong(ulong serverID, ushort clientID, ConnectionProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionVariableAsDouble")]
        static extern uint GetConnectionVariableAsDouble(ulong serverID, ushort clientID, ConnectionProperties flag, out double result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionVariableAsString")]
        static extern uint GetConnectionVariableAsString(ulong serverID, ushort clientID, ConnectionProperties flag, out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_cleanUpConnectionInfo")]
        static extern uint CleanUpConnectionInfo(ulong serverID, ushort clientID);

        /*server connection info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestServerConnectionInfo")]
        static extern uint RequestServerConnectionInfo(ulong serverID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerConnectionVariableAsUInt64")]
        static extern uint GetServerConnectionVariableAsulong(ulong serverID, ConnectionProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerConnectionVariableAsFloat")]
        static extern uint GetServerConnectionVariableAsFloat(ulong serverID, ConnectionProperties flag, out float result);

        /*client info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientSelfVariableAsInt")]
        static extern uint GetClientSelfVariableAsInt(ulong serverID, ClientProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientSelfVariableAsString")]
        static extern uint GetClientSelfVariableAsString(ulong serverID, ClientProperties flag, out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setClientSelfVariableAsInt")]
        static extern uint SetClientSelfVariableAsInt(ulong serverID, ClientProperties flag, int value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setClientSelfVariableAsString")]
        static extern uint SetClientSelfVariableAsString(ulong serverID, ClientProperties flag, string value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_flushClientSelfUpdates")]
        static extern uint FlushClientSelfUpdates(ulong serverID, string returnCode);

        //Ferry: ClientVariables => other clients, ClientSelfVariables => self client, so no setter and flush for other clients
        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsInt")]
        static extern uint GetClientVariableAsInt(ulong serverID, ushort clientID, ClientProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsUInt64")]
        static extern uint GetClientVariableAsulong(ulong serverID, ushort clientID, ClientProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsString")]
        static extern uint GetClientVariableAsString(ulong serverID, ushort clientID, ClientProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientList")]
        static extern uint GetClientList(ulong serverID, out ushort[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelOfClient")]
        static extern uint GetChannelOfClient(ulong serverID, ushort clientID, out ulong result);

        /*channel info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsInt")]
        static extern uint GetChannelVariableAsInt(ulong serverID, ulong channelID, ChannelProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsUInt64")]
        static extern uint GetChannelVariableAsulong(ulong serverID, ulong channelID, ChannelProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsString")]
        static extern uint GetChannelVariableAsString(ulong serverID, ulong channelID, ChannelProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelIDFromChannelNames")]
        static extern uint GetChannelIDFromChannelNames(ulong serverID, out string channelNameArray, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setChannelVariableAsInt")]
        static extern uint SetChannelVariableAsInt(ulong serverID, ulong channelID, ChannelProperties flag, int value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setChannelVariableAsUInt64")]
        static extern uint SetChannelVariableAsulong(ulong serverID, ulong channelID, ChannelProperties flag, ulong value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setChannelVariableAsString")]
        static extern uint SetChannelVariableAsString(ulong serverID, ulong channelID, ChannelProperties flag, string value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_flushChannelUpdates")]
        static extern uint FlushChannelUpdates(ulong serverID, ulong channelID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_flushChannelCreation")]
        static extern uint FlushChannelCreation(ulong serverID, ulong channelParentID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelList")]
        static extern uint GetChannelList(ulong serverID, out ulong[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelClientList")]
        static extern uint GetChannelClientList(ulong serverID, ulong channelID, out ushort[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getParentChannelOfChannel")]
        static extern uint GetParentChannelOfChannel(ulong serverID, ulong channelID, out ulong result);

        /*server info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerConnectionHandlerList")]
        static extern uint GetServerConnectionHandlerList(out ulong[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerVariableAsInt")]
        static extern uint GetServerVariableAsInt(ulong serverID, VirtualServerProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerVariableAsUInt64")]
        static extern uint GetServerVariableAsulong(ulong serverID, VirtualServerProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerVariableAsString")]
        static extern uint GetServerVariableAsString(ulong serverID, VirtualServerProperties flag, out string result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestServerVariables")]
        static extern uint RequestServerVariables(ulong serverID);

        #endregion DLL Import
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            /* Create struct for callback function pointers */
            ClientEventCallbackMapper cbs = new ClientEventCallbackMapper();
            client_callbackrare_struct cbs_rare = new client_callbackrare_struct(); // dummy

            cbs.onConnectStatusChangeEvent_delegate = new onConnectStatusChangeEvent_type(callback.onConnectStatusChangeEvent);
            cbs.onServerProtocolVersionEvent_delegate = new onServerProtocolVersionEvent_type(callback.onServerProtocolVersionEvent);
            cbs.onNewChannelEvent_delegate = new onNewChannelEvent_type(callback.onNewChannelEvent);
            cbs.onNewChannelCreatedEvent_delegate = new onNewChannelCreatedEvent_type(callback.onNewChannelCreatedEvent);
            cbs.onDelChannelEvent_delegate = new onDelChannelEvent_type(callback.onDelChannelEvent);
            cbs.onClientMoveEvent_delegate = new onClientMoveEvent_type(callback.onClientMoveEvent);
            cbs.onClientMoveSubscriptionEvent_delegate = new onClientMoveSubscriptionEvent_type(callback.onClientMoveSubscriptionEvent);
            cbs.onClientMoveTimeoutEvent_delegate = new onClientMoveTimeoutEvent_type(callback.onClientMoveTimeoutEvent);
            cbs.onTalkStatusChangeEvent_delegate = new onTalkStatusChangeEvent_type(callback.onTalkStatusChangeEvent);
            cbs.onIgnoredWhisperEvent_delegate = new onIgnoredWhisperEvent_type(callback.onIgnoredWhisperEvent);
            cbs.onServerErrorEvent_delegate = new onServerErrorEvent_type(callback.onServerErrorEvent);

            /* Initialize client lib with callbacks */
            /* Resource path points to the SDK\bin directory to locate the soundbackends folder when running from Visual Studio. */
            /* If you want to run directly from the SDK\bin directory, use an empty string instead to locate the soundbackends folder in the current directory. */
            uint error = CustomTS3Client.ts3client_initClientLib(ref cbs, ref cbs_rare, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE, null, "");
            if (error != Error.ok)
            {
                Console.WriteLine("Failed to init clientlib: {0}.", error);
                return;
            }

            ulong scHandlerID = 0;
            /* Spawn a new server connection handler using the default port and store the server ID */
            error = CustomTS3Client.ts3client_spawnNewServerConnectionHandler(0, out scHandlerID);
            if (error != Error.ok)
            {
                Console.WriteLine("Error spawning server connection handler: {0}", error);
                return;
            }

            /* Open default capture device */
            /* Passing empty string for mode and null or empty string for device will open the default device */
            error = CustomTS3Client.ts3client_openCaptureDevice(scHandlerID, "", null);
            if (error != Error.ok)
            {
                Console.WriteLine("Error opening capture device: {0}", error);
            }

            /* Open default playback device */
            /* Passing empty string for mode and NULL or empty string for device will open the default device */
            error = CustomTS3Client.ts3client_openPlaybackDevice(scHandlerID, "", null);
            if (error != Error.ok)
            {
                Console.WriteLine("Error opening playback device: {0}", error);
            }

            /* Create a new client identity */
            /* In your real application you should do this only once, store the assigned identity locally and then reuse it. */
            IntPtr identityPtr = IntPtr.Zero;
            error = CustomTS3Client.ts3client_createIdentity(out identityPtr);
            if (error != Error.ok)
            {
                Console.WriteLine("Error creating identity: {0}", error);
                return;
            }
            string identity = Marshal.PtrToStringAnsi(identityPtr);

            string defaultarray = "";
            /* Connect to server on localhost:9987 with nickname "client", no default channel, no default channel password and server password "secret" */
            //error = ts3client.ts3client_startConnection(scHandlerID, identity, "54.68.20.34", 9987, "win7", ref defaultarray, "", "secret");
            error = CustomTS3Client.ts3client_startConnection(scHandlerID, identity, "localhost", 9987, "win08r2", ref defaultarray, "", "secret");
            if (error != Error.ok)
            {
                Console.WriteLine("Error connecting to server: 0x{0:X4}", error);
                Console.ReadLine();
                return;
            }
            CustomTS3Client.ts3client_freeMemory(identityPtr);  /* Release dynamically allocated memory */

            Console.WriteLine("Client lib initialized and running");

            /* Query and print client lib version */
            IntPtr versionPtr = IntPtr.Zero;
            error = CustomTS3Client.ts3client_getClientLibVersion(out versionPtr);
            if (error != Error.ok)
            {
                Console.WriteLine("Failed to get clientlib version: {0}.", error);
                return;
            }
            string version = Marshal.PtrToStringAnsi(versionPtr);
            Console.WriteLine(version);
            CustomTS3Client.ts3client_freeMemory(versionPtr); /* Release dynamically allocated memory */

            Thread.Sleep(500);

            /* Wait for user input */
            Console.WriteLine("\n--- Press Return to disconnect from server and exit ---");
            Console.ReadLine();

            /* Disconnect from server */
            error = CustomTS3Client.ts3client_stopConnection(scHandlerID, "leaving");
            if (error != Error.ok)
            {
                Console.WriteLine("Error stopping connection: {0}", error);
                return;
            }

            Thread.Sleep(200);

            /* Destroy server connection handler */
            error = CustomTS3Client.ts3client_destroyServerConnectionHandler(scHandlerID);
            if (error != Error.ok)
            {
                Console.WriteLine("Error destroying clientlib: {0}", error);
                return;
            }

            /* Shutdown client lib */
            error = CustomTS3Client.ts3client_destroyClientLib();
            if (error != Error.ok)
            {
                Console.WriteLine("Failed to destroy clientlib: {0}", error);
                return;
            }

            Console.ReadLine();
        }
    }
}